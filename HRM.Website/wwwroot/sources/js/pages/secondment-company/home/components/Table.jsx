import React, { Fragment } from "react";

import Row from "./Row";

const Table = ({ data, onDelete }) => {
    const colSpan = window.hasPermission === "True" ? 10 : 8;

    return (
        <table className="table table-custom">
            <thead>
                <tr>
                    <th>#</th>
                    <th>Employee Code</th>
                    <th>Full Name</th>
                    <th>Nick Name</th>
                    <th>Current Company</th>
                    <th>Second Company</th>
                    {window.hasPermission === "True" && (
                        <Fragment>
                            <th>Edit</th>
                            <th>Delete</th>
                        </Fragment>
                    )}
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
                    <tr>
                        <td colSpan={colSpan}>No information available</td>
                    </tr>
                )}
            </tbody>
        </table>
    );
};

export default Table;
