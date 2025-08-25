import React from "react";

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
            <td key="rowIndex">{rowIndex}</td>
            <td key="empCode" className="align-left">{item.empCode}</td>
            <td key="fullName" className="align-left">{item.fullName}</td>
            <td key="nickName">{item.nickName}</td>
            <td key="currentCompanyName">{item.currentCompanyName}</td>
            <td key="secondCompanyName">{item.secondCompanyName}</td>
            {window.hasPermission === "True" && [
                <td key="editCell">
                    <a
                        onClick={handleEdit}
                        className="fa fa-pencil fa-lg icon-edit"
                    />
                </td>,
                <td key="deleteCell">
                    <a
                        onClick={handleDelete}
                        className="fa fa-times fa-lg icon-delete"
                    />
                </td>
            ]}
        </tr>
    );
};

export default Row;
