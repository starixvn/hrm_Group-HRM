import React from 'react';

const Row = ({ rowIndex, item, onDelete, onEdit }) => {
    const handleDelete = () => {
        if (item.totalMember <= 0) {
            onDelete(item.teamId);
        }
    };

    const handleEdit = () => {
        onEdit(item.teamId, item.teamName)
    };

    return (
        <tr>
            <td>{rowIndex}</td>
            <td className="align-left">{item.teamName}</td>
            <td>
                <a
                    className="fa fa-pencil fa-lg icon-edit"
                    onClick={handleEdit}
                />
            </td>
            <td>
                <a
                    className={`fa fa-times fa-lg ${item.totalMember > 0 ? 'fa-disabled' : ''} icon-delete`}
                    onClick={handleDelete}
                />
            </td>
        </tr>
    );
};

export default Row;