import React, { Fragment } from 'react';

import Row from './Row';

const Table = ({ data, onDelete }) => {
    return (
        <table className="table table-custom">
            <thead>
                <tr>
                    <th>#</th>
                    <th>Unit ID</th>
                    <th>Unit Code</th>
                    <th>Unit Category</th>
                    <th>Unit Type</th>
                    <th>Accountable Area</th>
                    <th>Personal In Charge</th>
                    <th>Active</th>
                    {
                        window.hasPermission === 'True' &&
                        <Fragment>
                            <th>Edit</th>
                            <th>Delete</th>
                        </Fragment>
                    }
                </tr>
            </thead>
            <tbody>
                {
                    data.length > 0
                        ? data.map((item, index) => <Row key={item.id} rowIndex={index + 1} item={item} onDelete={onDelete} />)
                        : <tr>
                            <td colSpan={window.hasPermission === 'True' ? '6' : '4'}>
                                No information available
                            </td>
                        </tr>
                }
            </tbody>
        </table>
    );
};

export default Table;