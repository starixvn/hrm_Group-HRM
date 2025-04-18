import React, { Fragment } from 'react';

import Row from './Row';

const Table = ({ data, onDelete }) => {
    const colSpan = window.hasPermission === 'True' ? 7 : 5;

    return (
        <table className="table table-custom">
            <thead>
                <tr>
                    <th>#</th>
                    <th>Department Id</th>
                    <th>Department Code</th>
                    <th>BU Code</th>
                    <th>Department Name</th>
                    <th>Accountable Area</th>
                    <th>PIC</th>
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
                            <td colSpan={colSpan}>No information available</td>
                        </tr>
                }
            </tbody>
        </table>
    );
};

export default Table;