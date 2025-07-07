import React, { Fragment } from "react";

import { resolveClientUrl } from "../../../../shared/common.functions";

const Row = ({ rowIndex, item, onDelete }) => {
    const handleEdit = () => {
        window.location = resolveClientUrl(`salarystructure/edit/${item.salaryStructureId}`);
    };

    const handleDelete = () => {
        onDelete(item);
    };

    return (
        <tr id={`row_${item.salaryStructureId}`}>
            <td>{rowIndex}</td>
            <td>{item.name}</td>
            <td>{item.jobLevelName}</td>
            <td>{item.localSalary}</td>
            <td>{item.expatSalary}</td>
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
