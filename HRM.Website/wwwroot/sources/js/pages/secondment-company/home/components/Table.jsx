import React from "react";

import Row from "./Row";

const Table = ({ data, onDelete }) => {
    const colSpan = window.hasPermission === "True" ? 10 : 8;

    return (
        <table className="table table-custom">
            <thead>
                <tr>
                    <th key="index">#</th>
                    <th key="employeeCode">Employee Code</th>
                    <th key="fullName">Full Name</th>
                    <th key="nickName">Nick Name</th>
                    <th key="currentCompany">Current Company</th>
                    <th key="secondCompany">Second Company</th>
                    {window.hasPermission === "True" && [
                        <th key="edit">Edit</th>,
                        <th key="delete">Delete</th>
                    ]}
                </tr>
            </thead>
            <tbody>
                {data.length > 0 ? (
                    data.map((item, index) => (
                        <Row
                            key={item.id}
                            rowIndex={index + 1}
                            item={item}
                            onDelete={onDelete}
                        />
                    ))
                ) : (
                    <tr key="no-data">
                        <td colSpan={colSpan}>No information available</td>
                    </tr>
                )}
            </tbody>
        </table>
    );
};

export default Table;
