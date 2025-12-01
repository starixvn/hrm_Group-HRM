import React, { Fragment } from 'react';

import Row from './Row';

const Table = ({ data, onDelete }) => {
    return (
        <table className="table table-custom">
            <thead>
                <tr>
                    <th>#</th>
                    <th>BU ID</th>
                    <th>BU Code</th>
                    <th>BU Category</th>
                    <th>BU Type</th>
                    <th>Accountable Area</th>
                    <th>Personal In Charge</th>
                    <th>Active</th>
                    <th>Total Members</th>
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