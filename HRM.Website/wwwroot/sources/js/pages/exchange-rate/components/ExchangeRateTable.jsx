import React, { useState, useMemo } from 'react';
import moment from 'moment';

const ExchangeRateTable = ({ data, onEdit, onDelete }) => {
    const [expandedRows, setExpandedRows] = useState(new Set());

    const hasPermission = () => window.hasPermission === 'True';

    const validateAndParseDate = date => {
        if (!date) return null;
        const dateMoment = moment(date);
        return dateMoment.isValid() ? dateMoment : null;
    };

    const formatRate = rate => {
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

    const isFutureDate = date => {
        const dateMoment = validateAndParseDate(date);
        if (!dateMoment) return false;
        return dateMoment.isAfter(moment().startOf('day'));
    };

    const formatDate = date => {
        const dateMoment = validateAndParseDate(date);
        if (!dateMoment) return '';
        return dateMoment.format('DD MMM, YYYY');
    };

    const parseDate = dateStr => {
        const dateMoment = validateAndParseDate(dateStr);
        if (!dateMoment) return null;
        const date = dateMoment.toDate();
        date.setHours(0, 0, 0, 0);
        return date;
    };

    const groupByCurrency = items => {
        const groups = {};
        items.forEach(item => {
            const currency = item.baseCurrency;
            if (!groups[currency]) {
                groups[currency] = [];
            }
            groups[currency].push(item);
        });
        return groups;
    };

    const parseItemsWithDates = items => {
        return items
            .map(item => {
                const dateStr = item.effectiveDate || item.EffectiveDate;
                const parsedDate = parseDate(dateStr);
                return { ...item, _parsedDate: parsedDate };
            })
            .filter(item => item._parsedDate !== null && !isNaN(item._parsedDate.getTime()));
    };

    const findMainItem = (itemsWithDates, today) => {
        itemsWithDates.sort((a, b) => b._parsedDate - a._parsedDate);

        for (let i = 0; i < itemsWithDates.length; i++) {
            if (itemsWithDates[i]._parsedDate <= today) {
                return { item: itemsWithDates[i], index: i };
            }
        }

        if (itemsWithDates.length > 0) {
            const lastIndex = itemsWithDates.length - 1;
            return { item: itemsWithDates[lastIndex], index: lastIndex };
        }

        return null;
    };

    const processCurrencyGroup = (currency, items, today) => {
        const itemsWithDates = parseItemsWithDates(items);
        if (itemsWithDates.length === 0) {
            return null;
        }

        const mainResult = findMainItem(itemsWithDates, today);
        if (!mainResult) {
            return null;
        }

        const subItems = itemsWithDates
            .filter((_, index) => index !== mainResult.index)
            .map(({ _parsedDate, ...item }) => item);

        const { _parsedDate, ...cleanMainItem } = mainResult.item;

        return {
            key: currency,
            mainItem: cleanMainItem,
            subItems,
        };
    };

    const groupedData = useMemo(() => {
        if (!data || data.length === 0) {
            return [];
        }

        // Filter out USD records
        const filteredData = data.filter(item => {
            const currency = item.baseCurrency || item.BaseCurrency;
            return currency && currency.toUpperCase() !== 'USD';
        });

        const groupsByCurrency = groupByCurrency(filteredData);

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const result = Object.entries(groupsByCurrency)
            .map(([currency, items]) => processCurrencyGroup(currency, items, today))
            .filter(group => group !== null);

        return result;
    }, [data]);

    const toggleRow = key => {
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

    const canEditOrDelete = item => {
        const isUsed = item.isUsed === true;
        if (!isUsed) return true;
        return isFutureDate(item.effectiveDate);
    };

    const renderActionButtonsCell = (item, className = 'action-cell') => (
        <td className={className}>
            <button
                className="btn-action btn-edit"
                onClick={() => onEdit && onEdit(item)}
                title="Edit"
            >
                <i className="fa fa-pencil"></i>
            </button>
            <button
                className="btn-action btn-delete"
                onClick={() => onDelete && onDelete(item)}
                title="Delete"
            >
                <i className="fa fa-trash"></i>
            </button>
        </td>
    );

    const renderActionButtons = (item, className = 'action-cell') => {
        if (!hasPermission()) {
            return null;
        }
        if (!canEditOrDelete(item)) {
            return <td className={className}></td>;
        }
        return renderActionButtonsCell(item, className);
    };

    const renderSubItemRow = (subItem, groupKey, subIndex) => {
        return (
            <tr key={`${groupKey}-sub-${subIndex}`} className="sub-item-row">
                <td className="sub-item-currency">
                    <span className="sub-item-indicator"></span>
                    <span className="sub-item-text">{subItem.baseCurrency}</span>
                </td>
                <td className="sub-item-rate">{formatRate(subItem.toUsd)}</td>
                <td className="sub-item-date">{formatDate(subItem.effectiveDate)}</td>
                {renderActionButtons(subItem, 'sub-item-action')}
            </tr>
        );
    };

    const renderMainRow = (group, isExpanded, hasSubItems) => {
        const mainItem = group.mainItem;
        const { baseCurrency, toUsd, effectiveDate } = mainItem;

        return (
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
                {renderActionButtons(mainItem)}
            </tr>
        );
    };

    return (
        <div>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>Base Currency</th>
                        <th>To USD</th>
                        <th>Effective Date</th>
                        {hasPermission() && <th style={{ width: '100px', padding: '16px' }}>Actions</th>}
                    </tr>
                </thead>
                <tbody>
                    {groupedData.length > 0 ? (
                        groupedData.map(group => {
                            if (!group || !group.mainItem) {
                                return null;
                            }
                            const isExpanded = expandedRows.has(group.key);
                            const hasSubItems = group.subItems.length > 0;

                            return (
                                <React.Fragment key={group.key}>
                                    {renderMainRow(group, isExpanded, hasSubItems)}
                                    {isExpanded && hasSubItems && group.subItems.map((subItem, subIndex) =>
                                        renderSubItemRow(subItem, group.key, subIndex)
                                    )}
                                </React.Fragment>
                            );
                        })
                    ) : (
                        <tr>
                            <td colSpan={hasPermission() ? 4 : 3} className="empty-state">No information available</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ExchangeRateTable;
