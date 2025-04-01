import React from 'react';
import Row from './Row';

const Table = ({ data, onDelete, onEdit }) => {
    if (!data) {
        return null;
    }

    return (
        <table className="table table-custom" id="data" style={{ width: 'auto', minWidth: '600px' }}>
            <thead>
                <tr>
                    <th>#</th>
                    <th>Team Name</th>
                    <th>Edit</th>
                    <th>Delete</th>
                </tr>
            </thead>
            <tbody>
                {
                    data.length > 0
                        ? data.map((item, index) => <Row key={item.teamId} rowIndex={index + 1} item={item} onDelete={onDelete} onEdit={onEdit} />)
                        : <tr>
                            <td colSpan='4'>
                                No information available
                            </td>
                        </tr>
                }
            </tbody>
        </table>
    );
};

export default Table;