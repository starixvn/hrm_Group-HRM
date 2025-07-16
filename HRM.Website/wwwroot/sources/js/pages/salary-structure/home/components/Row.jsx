import React, { Fragment } from "react";

import { resolveClientUrl } from "../../../../shared/common.functions";

const Row = ({ rowIndex, item, onDelete }) => {
    const handleEdit = () => {
        window.location = resolveClientUrl(`salarystructure/edit/${item.salaryStructureId}`);
    };

    const handleDelete = () => onDelete(item);;

    const formatSalary = (value) => {
        if (value === null || value === undefined || value === '') {
            return '';
        }

        const number = Number(value);
        if (isNaN(number)) {
            return '';
        }

        return number.toLocaleString('en-US');
    };

    const handleMouseEnter = () => {
        document.querySelectorAll(`.row-group-${item.salaryStructureId}`).forEach(tr => {
          tr.classList.add('hovered-row');
        });
    };

    const handleMouseLeave = () => {
        document.querySelectorAll(`.row-group-${item.salaryStructureId}`).forEach(tr => {
          tr.classList.remove('hovered-row');
        });
    };

    const getRows = () => {
        if (item.details.length === 0){
            return (
                <tr id={`row_${item.salaryStructureId}`}
                    className={`row-group-${item.salaryStructureId} ${rowIndex % 2 === 0 ? 'odd-row' : 'even-row'}`}
                    onMouseEnter={handleMouseEnter}
                    onMouseLeave={handleMouseLeave}
                >
                    <td>{rowIndex}</td>
                    <td className="text-left">{item.name}</td>
                    <td></td>
                    <td className="text-right"></td>
                    <td className="text-right"></td>
                    {
                        window.hasPermission === 'True' && (
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
        }

        const rows = [];
        const rowspan = item.details.length >= 2 ? item.details.length : 1;

        item.details.forEach((detail, index) => {
            rows.push(
                <tr id={`row_${item.salaryStructureId}_${index}`}
                    className={`row-group-${item.salaryStructureId} ${rowIndex % 2 === 0 ? 'odd-row' : 'even-row'}`}
                    onMouseEnter={handleMouseEnter}
                    onMouseLeave={handleMouseLeave}
                >
                    {index === 0 && (
                        <Fragment>
                            <td rowSpan={rowspan}>{rowIndex}</td>
                            <td className="text-left" rowSpan={rowspan}>{item.name}</td>
                        </Fragment>
                    )}
                    <td>{detail.name}</td>
                    <td className="text-right">{formatSalary(detail.localSalary)}</td>
                    <td className="text-right">{formatSalary(detail.expatSalary)}</td>
                    {
                        index === 0 &&
                        window.hasPermission === 'True' && (
                        <Fragment>
                            <td rowSpan={rowspan}>
                                <a
                                    onClick={handleEdit}
                                    className="fa fa-pencil fa-lg icon-edit"
                                />
                            </td>
                            <td rowSpan={rowspan}>
                                <a
                                    onClick={handleDelete}
                                    className="fa fa-times fa-lg icon-delete"
                                />
                            </td>
                        </Fragment>
                    )}
                </tr>
            );
        });

        return rows;
    };

    return getRows();
};

export default Row;