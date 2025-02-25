import React, { Fragment } from 'react';

import Row from './Row';

const Table = ({ data, onDelete, onEdit }) => {
    const colSpan = window.hasPermission === 'True' ? 8 : 6;

    return (
        <Fragment>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Job Title</th>
                        <th>Category</th>
                        <th>Total Expat</th>
                        <th>Total Local</th>
                        <th>Total(s)</th>
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
                            ? data.map((item, index) => <Row key={item.id} rowIndex={index + 1} item={item} onEdit={onEdit} onDelete={onDelete} />)
                            : <tr>
                                <td colSpan={colSpan}>No information available</td>
                            </tr>
                    }
                </tbody>
            </table>
        </Fragment>
    );
};

export default Table;