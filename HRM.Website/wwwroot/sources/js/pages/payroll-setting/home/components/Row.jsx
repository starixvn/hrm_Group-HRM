import React, { Fragment, useState } from "react";

import { resolveClientUrl } from "../../../../shared/common.functions";
import SweetModalDialog from "../../../../shared/components/SweetModalDialog";
import { Formik, Form, Field } from "formik";

const Row = ({ rowIndex, item, onDelete }) => {
  const [editModalDialog, setEditModalDialog] = useState(false);
  const handleEdit = () => {
    setEditModalDialog(true);
    // window.location = resolveClientUrl(`salarystructure/edit/${item.salaryStructureId}`);
  };

  const handleDelete = (allowance) => onDelete(allowance);

  const formatSalary = (value) => {
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
      .forEach((tr) => {
        tr.classList.add("hovered-row");
      });
  };

  const handleMouseLeave = () => {
    document
      .querySelectorAll(`.row-group-${item.payrollItemSettingId}`)
      .forEach((tr) => {
        tr.classList.remove("hovered-row");
      });
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
          <td className="text-right">{allowance.amount}</td>
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

  const editContent = () => {
    return (
      <div style={{display: 'flex', flexDirection: 'column', gap: '10px'}}>
        <div
          style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            gap: "10px",
          }}
        >
          Type:
          <div style={{ flex: 1 }}>
            <input
              type="text"
              disabled
              value="Bui Van A"
              className="form-control"
            />
          </div>
        </div>
        <div
          style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            gap: "10px",
          }}
        >
          Item Name:
          <div style={{ flex: 1 }}>
            <input type="text" value="Bui Van A" className="form-control" />
          </div>
        </div>
      </div>
      // <div className='form-horizontal'>
      //     <div className='form-group'>
      //         <div className='label-form'>
      //             <div className='control-label'>
      //                 Employee
      //                 <span className='require'>*</span>
      //             </div>
      //         </div>
      //         {/* <div className='select-form'>
      //             <input type='text' name='itemName' className='form-control' value={""} />
      //         </div> */}
      //     </div>
      // </div>
    );
  };

  return (
    <>
      {getRows()}

      <SweetModalDialog
        show={editModalDialog}
        contents={editContent()}
        onSubmit={() => {}}
        onHide={() => setEditModalDialog(false)}
        title="Edit Allowance Item"
      />
    </>
  );
};

export default Row;
