import React, { useState, useMemo } from 'react';

const ExchangeRateTable = ({ data, onEdit, onDelete }) => {
    const [expandedRows, setExpandedRows] = useState(new Set());

    const formatRate = (rate) => {
        if (rate === null || rate === undefined || isNaN(rate) || rate === 0) {
            return <span>N/A</span>;
        }
        return new Intl.NumberFormat('en-US', {
            minimumFractionDigits: 8,
            maximumFractionDigits: 8
        }).format(rate);
    };

    const isDatePassed = (date) => {
        if (!date) return false;
        try {
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            const dateObj = new Date(date);
            dateObj.setHours(0, 0, 0, 0);

            return dateObj < today;
        } catch (error) {
            return false;
        }
    };

    const formatDate = (date) => {
        if (!date) return '';

        try {
            const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

            if (typeof date === 'string') {
                let dateStr = date.split('T')[0].split(' ')[0].trim();
                const parts = dateStr.split('-');
                if (parts.length === 3) {
                    const year = parseInt(parts[0], 10);
                    const month = parseInt(parts[1], 10);
                    const day = parseInt(parts[2], 10);

                    if (!isNaN(year) && !isNaN(month) && !isNaN(day) && month >= 1 && month <= 12 && day >= 1 && day <= 31) {
                        return `${day} ${monthNames[month - 1]}, ${year}`;
                    }
                }

                const dateObj = new Date(date);
                if (!isNaN(dateObj.getTime())) {
                    const d = dateObj.getDate();
                    const m = dateObj.getMonth();
                    const y = dateObj.getFullYear();
                    return `${d} ${monthNames[m]}, ${y}`;
                }

                return dateStr;
            }

            if (date instanceof Date) {
                if (isNaN(date.getTime())) {
                    return '';
                }
                const day = date.getDate();
                const month = date.getMonth();
                const year = date.getFullYear();
                return `${day} ${monthNames[month]}, ${year}`;
            }

            if (typeof date === 'number') {
                const dateObj = new Date(date);
                if (!isNaN(dateObj.getTime())) {
                    const day = dateObj.getDate();
                    const month = dateObj.getMonth();
                    const year = dateObj.getFullYear();
                    return `${day} ${monthNames[month]}, ${year}`;
                }
            }

            return '';
        } catch (error) {
            return '';
        }
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

        return Object.entries(groups).map(([key, items]) => {
            const itemsWithDates = items.map(item => {
                const dateStr = item.effectiveDate;
                const date = new Date(dateStr);
                date.setHours(0, 0, 0, 0);
                return { ...item, _parsedDate: date };
            });

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

            const subItems = itemsWithDates
                .filter((_, index) => index !== mainIndex)
                .map(({ _parsedDate, ...item }) => item);

            let finalMainItem = null;
            if (mainItem) {
                const { _parsedDate, ...cleanMainItem } = mainItem;
                finalMainItem = cleanMainItem;
            }

            return {
                key,
                mainItem: finalMainItem,
                subItems,
            };
        });
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
                        groupedData.map((group, index) => {
                            const isExpanded = expandedRows.has(group.key);
                            const hasSubItems = group.subItems.length > 0;
                            const mainItem = group.mainItem;
                            const baseCurrency = mainItem.baseCurrency;
                            const toUsd = mainItem.toUsd;
                            const effectiveDate = mainItem.effectiveDate;
                            const itemId = mainItem.currencyId;

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
                                        {window.hasPermission === 'True' && !isDatePassed(effectiveDate) && (
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
                                        {window.hasPermission === 'True' && isDatePassed(effectiveDate) && (
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
                                                {window.hasPermission === 'True' && !isDatePassed(subItem.effectiveDate) && (
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
                                                {window.hasPermission === 'True' && isDatePassed(subItem.effectiveDate) && (
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

