import React, { useState } from "react";
import moment from "moment";
import Row from "./Row";

const Table = ({ data, onDelete }) => {
    const [sortConfig, setSortConfig] = useState({
        key: null,
        direction: null,
    });
    const colSpan = window.hasPermission === "True" ? 10 : 8;

    const columns = [
        { label: "#", key: null, sortable: false },
        { label: "Department ID", key: "groupDeptId", sortable: true },
        { label: "Department Code", key: "code", sortable: true },
        { label: "BU Code", key: "buCode", sortable: true },
        { label: "Department Name", key: "name", sortable: true },
        { label: "Accountable Area", key: "accountableArea", sortable: true },
        { label: "Personal In Charge", key: "picEmployeeName", sortable: true },
        { label: "Active", key: "isActive", sortable: false },
        { label: "Total Members", key: "totalMembers", sortable: true },
    ];

    if (window.hasPermission === "True") {
        columns.push({ label: "Edit", key: null, sortable: false });
        columns.push({ label: "Delete", key: null, sortable: false });
    }

    const getCellValue = (item, key) => {
        if (!key) {
            return '';
        }
        let value = item[key];

        if (key === "buCode") {
            value = item.unit ? item.unit.code : '';
        }

        if (value === null || value === undefined) {
            return '';
        }

        if (typeof value === "string") {
            return value.trim().toLowerCase();
        }

        if (typeof value === "boolean") {
            return value ? "true" : "false";
        }

        return value;
    };

    const sortedData = React.useMemo(() => {
        if (!sortConfig.key) {
            return data;
        }

        const compareValues = (valA, valB) => {
            if (valA === '' || valB === '') {
                if (valA === '' && valB === '') {
                    return 0;
                }
                return valA === '' ? 1 : -1;
            }

            if (!isNaN(valA) && !isNaN(valB)) {
                return Number(valA) - Number(valB);
            }

            if (moment(valA).isValid() && moment(valB).isValid()) {
                return new Date(valA).getTime() - new Date(valB).getTime();
            }

            return String(valA).localeCompare(String(valB), "vi", { sensitivity: "base" });
        };

        const sorted = [...data].sort((a, b) =>
            compareValues(getCellValue(a, sortConfig.key), getCellValue(b, sortConfig.key))
        );

        return sortConfig.direction === "desc" ? sorted.reverse() : sorted;
    }, [data, sortConfig]);

    const onSort = key => {
        if (!key) {
            return;
        }

        let direction = "asc";
        if (sortConfig.key === key && sortConfig.direction === "asc") {
            direction = "desc";
        }

        setSortConfig({ key, direction });
    };

    const renderSortIcon = key => {
        if (sortConfig.key !== key) {
            return "fa fa-sort";
        }

        return sortConfig.direction === "asc"
            ? "fa fa-sort-asc"
            : "fa fa-sort-desc";
    };

    return (
        <table className="table table-custom">
            <thead>
                <tr>
                    {columns.map(({ label, key, sortable }) => 
                        <th
                            key={label}
                            className={sortable ? "col-sorter" : ''}
                            onClick={() => onSort(key)}
                            style={{
                                cursor: sortable ? "pointer" : "default",
                            }}>
                            <div className="cell-control-wrapper">
                                <span style={{ textAlign: "center" }}>
                                    {label}
                                </span>
                                {sortable && (
                                    <div className={renderSortIcon(key)}></div>
                                )}
                            </div>
                        </th>
                    )}
                </tr>
            </thead>
            <tbody>
                {sortedData.length > 0 ? (
                    sortedData.map((item, index) => (
                        <Row
                            key={item.id}
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

