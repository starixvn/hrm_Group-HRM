import React, { Fragment } from "react";

import { resolveClientUrl } from "../../../../shared/common.functions";

const Row = ({ rowIndex, item, onDelete }) => {
    const handleEdit = () => {
        window.location = resolveClientUrl(`secondmentcompany/edit/${item.empId}`);
    };

    const handleDelete = () => {
        onDelete(item);
    };

    return (
        <tr id={`row_${item.empId}`}>
            <td>{rowIndex}</td>
            <td>{item.empCode}</td>
            <td>{item.fullName}</td>
            <td>{item.nickName}</td>
            <td>{item.currentCompanyName}</td>
            <td>{item.secondCompanyName}</td>
            {window.hasPermission === "True" && (
                <Fragment>
                    <td>
                        <a
                            onClick={handleEdit}
                            className="fa fa-pencil fa-lg icon-edit"
                        />
                    </td>
                    <td>
                        <a
                            onClick={handleDelete}
                            className="fa fa-times fa-lg icon-delete"
                        />
                    </td>
                </Fragment>
            )}
        </tr>
    );
};

export default Row;
