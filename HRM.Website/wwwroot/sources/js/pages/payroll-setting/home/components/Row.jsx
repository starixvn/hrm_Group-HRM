import React, { Fragment, useState } from "react";

import SweetModalDialog from "../../../../shared/components/SweetModalDialog";
import AddEditAllowanceItemContent from "./AddEditAllowanceItemContent";

const Row = ({ rowIndex, item, saveEditData, onDelete }) => {
  const [editModalDialog, setEditModalDialog] = useState(false);
  const [editData, setEditData] = useState({
    payrollItemSettingId: "",
    allowanceItemSettingId: "",
    type: "",
    itemName: "",
    amount: "",
    isPercent: false,
    isActive: true,
  });

  const handleEdit = (item, allowance) => {
    setEditData({
      payrollItemSettingId: item.payrollItemSettingId,
      allowanceItemSettingId: allowance.allowanceItemSettingId,
      type: item.type,
      itemName: allowance.itemName,
      amount: allowance.amount,
      isPercent: allowance.isPercent,
      isActive: allowance.isActive,
    });
    setEditModalDialog(true);
  };

  const handleDelete = allowance => onDelete(allowance);

  const formatSalary = value => {
    if (value === null || value === undefined || value === "") {
      return "";
    }

    const number = Number(value);
    if (isNaN(number)) {
      return "";
    }

    return number.toLocaleString("en-US");
  };

  const handleMouseEnter = () => {
    document
      .querySelectorAll(`.row-group-${item.payrollItemSettingId}`)
      .forEach(tr => {
        tr.classList.add("hovered-row");
      });
  };

  const handleMouseLeave = () => {
    document
      .querySelectorAll(`.row-group-${item.payrollItemSettingId}`)
      .forEach(tr => {
        tr.classList.remove("hovered-row");
      });
  };

  const editAllowanceItem = async () => {
    const result = await saveEditData(editData);
    if (result) {
      setEditModalDialog(false);
    }
  };

  const getRows = () => {
    if (item.payrollAllowanceItems.length === 0) {
      return (
        <tr
          id={`row_${item.payrollItemSettingId}`}
          className={`row-group-${item.payrollItemSettingId} ${
            rowIndex % 2 === 0 ? "odd-row" : "even-row"
          }`}
          onMouseEnter={handleMouseEnter}
          onMouseLeave={handleMouseLeave}
        >
          <td>{rowIndex}</td>
          <td className="text-left">{item.type}</td>
          <td></td>
          <td></td>
          <td></td>
          {window.hasPermission === "True" && (
            <Fragment>
              <td></td>
              <td></td>
            </Fragment>
          )}
        </tr>
      );
    }

    const rows = [];
    const rowspan =
      item.payrollAllowanceItems.length >= 2
        ? item.payrollAllowanceItems.length
        : 1;

    item.payrollAllowanceItems.forEach((allowance, index) => {
      rows.push(
        <tr
          id={`row_${item.payrollItemSettingId}_${index}`}
          className={`row-group-${item.payrollItemSettingId} ${
            rowIndex % 2 === 0 ? "odd-row" : "even-row"
          }`}
          onMouseEnter={handleMouseEnter}
          onMouseLeave={handleMouseLeave}
        >
          {index === 0 && (
            <Fragment>
              <td rowSpan={rowspan}>{rowIndex}</td>
              <td className="text-left" rowSpan={rowspan}>
                {item.type}
              </td>
            </Fragment>
          )}
          <td>{allowance.itemName}</td>
          <td className="text-right">{formatSalary(allowance.amount)}</td>
          <td>{allowance.isActive ? 'Yes' : 'No'}</td>
          {window.hasPermission === "True" && (
            <Fragment>
              <td>
                <a
                  onClick={() => handleEdit(item, allowance)}
                  className="fa fa-pencil fa-lg icon-edit"
                />
              </td>
              <td>
                <a
                  onClick={() => handleDelete(allowance)}
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

  return (
    <>
      {getRows()}

      <SweetModalDialog
        show={editModalDialog}
        contents={<AddEditAllowanceItemContent editData={editData} setEditData={setEditData} />}
        onSubmit={editAllowanceItem}
        onHide={() => setEditModalDialog(false)}
        title="Edit Allowance Item"
        submitText="Save"
      />
    </>
  );
};

export default Row;
