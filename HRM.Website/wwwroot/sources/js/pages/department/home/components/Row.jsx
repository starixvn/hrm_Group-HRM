import React, { Fragment } from 'react';

import { resolveClientUrl } from '../../../../shared/common.functions';

const Row = ({ rowIndex, item, onDelete }) => {
    const handleEdit = () => {
        window.location = resolveClientUrl(`department/edit/${item.id}`);
    };

    const handleDelete = () => {
        onDelete(item);
    };

    return (
        <tr id={`row_${item.id}`}>
            <td>{rowIndex}</td>
            <td>{item.code}</td>
            <td>{item.name}</td>
            <td>{item.unit ? item.unit.code : ""}</td>
            <td>
                {item.isActive && <i className="fa fa-check fa-lg icon-check" />}
            </td>
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