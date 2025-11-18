import React, { useState, useMemo } from 'react';
import moment from 'moment';

const ExchangeRateTable = ({ data, onEdit, onDelete }) => {
    const [expandedRows, setExpandedRows] = useState(new Set());

    const formatRate = (rate) => {
        if (rate === null || rate === undefined || isNaN(rate) || rate === 0) {
            return <span>N/A</span>;
        }
        const num = parseFloat(rate);
        const formatted = num.toFixed(8);
        const decimalPart = formatted.split('.')[1];
        if (decimalPart && decimalPart === '00000000') {
            return num.toString();
        }

        return formatted;
    };

    const isExpired = (date) => {
        if (!date) return false;
        const dateMoment = moment(date);
        if (!dateMoment.isValid()) return false;
        return dateMoment.isSameOrBefore(moment().startOf('day'));
    };

    const formatDate = (date) => {
        if (!date) return '';
        const dateMoment = moment(date);
        if (!dateMoment.isValid()) return '';
        return dateMoment.format('DD MMM, YYYY');
    };

    const parseDate = (dateStr) => {
        if (!dateStr) return null;
        const dateMoment = moment(dateStr);
        if (!dateMoment.isValid()) return null;
        const date = dateMoment.toDate();
        date.setHours(0, 0, 0, 0);
        return date;
    };

    const groupedData = useMemo(() => {
        if (!data || data.length === 0) return [];

        const groups = {};
        data.forEach((item) => {
            const key = item.baseCurrency;
            if (!groups[key]) {
                groups[key] = [];
            }
            groups[key].push(item);
        });

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const result = Object.entries(groups).map(([key, items]) => {
            const itemsWithDates = items.map(item => {
                const dateStr = item.effectiveDate || item.EffectiveDate;
                const parsedDate = parseDate(dateStr);
                return { ...item, _parsedDate: parsedDate };
            }).filter(item => item._parsedDate !== null && !isNaN(item._parsedDate.getTime()));

            if (itemsWithDates.length === 0) {
                return null;
            }

            itemsWithDates.sort((a, b) => {
                return b._parsedDate - a._parsedDate;
            });

            let mainItem = null;
            let mainIndex = -1;

            for (let i = 0; i < itemsWithDates.length; i++) {
                if (itemsWithDates[i]._parsedDate <= today) {
                    mainItem = itemsWithDates[i];
                    mainIndex = i;
                    break;
                }
            }

            if (!mainItem && itemsWithDates.length > 0) {
                mainItem = itemsWithDates[itemsWithDates.length - 1];
                mainIndex = itemsWithDates.length - 1;
            }

            if (!mainItem) {
                return null;
            }

            const subItems = itemsWithDates
                .filter((_, index) => index !== mainIndex)
                .map(({ _parsedDate, ...item }) => item);

            const { _parsedDate, ...cleanMainItem } = mainItem;

            return {
                key,
                mainItem: cleanMainItem,
                subItems,
            };
        })
            .filter(group => group !== null);

        return result;
    }, [data]);

    const toggleRow = (key) => {
        setExpandedRows(prev => {
            const newSet = new Set(prev);
            if (newSet.has(key)) {
                newSet.delete(key);
            } else {
                newSet.add(key);
            }
            return newSet;
        });
    };

    return (
        <div>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>Base Currency</th>
                        <th>To USD</th>
                        <th>Effective Date</th>
                        {window.hasPermission === 'True' && <th style={{ width: '100px' }}>Actions</th>}
                    </tr>
                </thead>
                <tbody>
                    {groupedData.length > 0 ? (
                        groupedData.map((group) => {
                            if (!group || !group.mainItem) {
                                return null;
                            }
                            const isExpanded = expandedRows.has(group.key);
                            const hasSubItems = group.subItems.length > 0;
                            const mainItem = group.mainItem;
                            const baseCurrency = mainItem.baseCurrency;
                            const toUsd = mainItem.toUsd;
                            const effectiveDate = mainItem.effectiveDate;

                            return (
                                <React.Fragment key={group.key}>
                                    <tr className={hasSubItems ? 'expandable-row' : ''}>
                                        <td className={`currency-cell ${hasSubItems ? 'has-sub-items' : ''}`}>
                                            {hasSubItems ? (
                                                <span
                                                    className={`expand-icon ${isExpanded ? 'expanded' : ''}`}
                                                    onClick={() => toggleRow(group.key)}
                                                    title={isExpanded ? 'Close History' : 'Show History'}
                                                >
                                                    <i className={`fa ${isExpanded ? 'fa-angle-down' : 'fa-angle-right'}`}></i>
                                                </span>
                                            ) : (
                                                <span className="expand-icon-placeholder"></span>
                                            )}
                                            <span className="currency-text">{baseCurrency}</span>
                                        </td>
                                        <td className="rate-cell">{formatRate(toUsd)}</td>
                                        <td>{formatDate(effectiveDate)}</td>
                                        {window.hasPermission === 'True' && !isExpired(effectiveDate) && (
                                            <td className="action-cell">
                                                <button
                                                    className="btn-action btn-edit"
                                                    onClick={() => onEdit && onEdit(mainItem)}
                                                    title="Edit"
                                                >
                                                    <i className="fa fa-pencil"></i>
                                                </button>
                                                <button
                                                    className="btn-action btn-delete"
                                                    onClick={() => onDelete && onDelete(mainItem)}
                                                    title="Delete"
                                                >
                                                    <i className="fa fa-trash"></i>
                                                </button>
                                            </td>
                                        )}
                                        {window.hasPermission === 'True' && isExpired(effectiveDate) && (
                                            <td className="action-cell"></td>
                                        )}
                                    </tr>
                                    {isExpanded && hasSubItems && group.subItems.map((subItem, subIndex) => {
                                        return (
                                            <tr key={`${group.key}-sub-${subIndex}`} className="sub-item-row">
                                                <td className="sub-item-currency">
                                                    <span className="sub-item-indicator"></span>
                                                    <span className="sub-item-text">{subItem.baseCurrency}</span>
                                                </td>
                                                <td className="sub-item-rate">{formatRate(subItem.toUsd)}</td>
                                                <td className="sub-item-date">{formatDate(subItem.effectiveDate)}</td>
                                                {window.hasPermission === 'True' && !isExpired(subItem.effectiveDate) && (
                                                    <td className="sub-item-action">
                                                        <button
                                                            className="btn-action btn-edit"
                                                            onClick={() => onEdit && onEdit(subItem)}
                                                            title="Edit"
                                                        >
                                                            <i className="fa fa-pencil"></i>
                                                        </button>
                                                        <button
                                                            className="btn-action btn-delete"
                                                            onClick={() => onDelete && onDelete(subItem)}
                                                            title="Delete"
                                                        >
                                                            <i className="fa fa-trash"></i>
                                                        </button>
                                                    </td>
                                                )}
                                                {window.hasPermission === 'True' && isExpired(subItem.effectiveDate) && (
                                                    <td className="sub-item-action"></td>
                                                )}
                                            </tr>
                                        );
                                    })}
                                </React.Fragment>
                            );
                        })
                    ) : (
                        <tr>
                            <td colSpan={window.hasPermission === 'True' ? 4 : 3} className="empty-state">No information available</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ExchangeRateTable;

