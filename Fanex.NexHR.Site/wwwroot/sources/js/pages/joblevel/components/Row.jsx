import React, { Fragment } from 'react';

const Row = ({ rowIndex, item, onDelete, onEdit }) => {
    const handleDelete = () => {
        onDelete(item);
    };

    const handleEdit = () => {
        onEdit(item);
    };

    return (
        <tr id={`row_${item.id}`}>
            <td>{rowIndex}</td>
            <td className="align-left">{item.name}</td>
            {
                window.hasPermission === 'True' &&
                <Fragment>
                    <td>
                        <a onClick={handleEdit} className="fa fa-pencil fa-lg icon-edit" />
                    </td>
                    <td>
                        <a onClick={handleDelete} className="fa fa-times fa-lg icon-delete" />
                    </td>
                </Fragment>
            }
        </tr>
    );
};

export default Row;