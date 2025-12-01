import React, { useState } from 'react';
import Row from './Row';

const Table = ({ data, onDelete }) => {
    const [sortConfig, setSortConfig] = useState({ key: null, direction: null });
    const colSpan = window.hasPermission === 'True' ? 8 : 6;

    const columns = [
        { label: '#', key: null, sortable: false },
        { label: 'Salary Structure', key: 'name', sortable: true },
        { label: 'Job Level', key: 'jobLevelName', sortable: false },
        { label: 'Basic Salary', key: 'basicSalary', sortable: false },
        { label: 'Job Level Add-On', key: 'jobLevelAddOn', sortable: false },
        { label: 'Basic Salary (USD)', key: 'expatSalary', sortable: false },
        { label: 'Job Level Add-On (USD)', key: 'expatJobLevelAddOn', sortable: false },
    ];

    if (window.hasPermission === 'True') {
        columns.push({ label: 'Edit', key: null, sortable: false });
        columns.push({ label: 'Delete', key: null, sortable: false });
    }

    const getCellValue = (item, key) => {
        if (!key) {
            return '';
        }
        const value = item[key];
        if (value === null || value === undefined) {
            return '';
        }
        if (typeof value === 'string') {
            return value.trim().toLowerCase();
        }
        if (typeof value === 'boolean') {
            return value ? 'true' : 'false';
        }

        return value;
    };

    const sortedData = React.useMemo(() => {
        if (!sortConfig.key) {
            return data;
        }

        const sorted = [...data].sort((a, b) => {
            const valA = getCellValue(a, sortConfig.key);
            const valB = getCellValue(b, sortConfig.key);

            if (valA === '' && valB !== '') {
                return 1;
            }
            if (valB === '' && valA !== '') {
                return -1;
            }
            if (valA === '' && valB === '') {
                return 0;
            }

            if (!isNaN(valA) && !isNaN(valB)) {
                return valA - valB;
            }

            return valA.localeCompare(valB, 'vi', { sensitivity: 'base' });
        });

        if (sortConfig.direction === 'desc') {
            sorted.reverse();
        }

        return sorted;
    }, [data, sortConfig]);

    const onSort = key => {
        if (!key) {
            return;
        }

        let direction = 'asc';
        if (sortConfig.key === key && sortConfig.direction === 'asc') {
            direction = 'desc';
        }

        setSortConfig({ key, direction });
    };

    const renderSortIcon = key => {
        if (sortConfig.key !== key) {
            return 'fa fa-sort';
        }

        return sortConfig.direction === 'asc'
            ? 'fa fa-sort-asc'
            : 'fa fa-sort-desc';
    };

    return (
        <table className='table table-custom'>
            <thead>
                <tr>
                    {columns.map(({ label, key, sortable }) => 
                        <th
                            key={label}
                            className={sortable ? 'col-sorter' : ''}
                            onClick={() => {onSort(key)}}
                            style={{ cursor: sortable ? 'pointer' : 'default' }}
                        >
                            <div className='cell-control-wrapper'>
                                {label}
                                {sortable && <div className={renderSortIcon(key)}></div>}
                            </div>
                        </th>
                    )}
                </tr>
            </thead>
            <tbody>
                {sortedData.length > 0 ? (
                    sortedData.map((item, index) => (
                        <Row
                            key={item.salaryStructureId}
                            rowIndex={index + 1}
                            item={item}
                            onDelete={onDelete}
                        />
                    ))
                ) : (
                    <tr>
                        <td colSpan={colSpan}>No information available</td>
                    </tr>
                )}
            </tbody>
        </table>
    );
};

export default Table;
