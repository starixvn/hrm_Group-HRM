import React from "react";
import moment from "moment";

const Row = ({ rowIndex, item }) => {
  var transferredCompany = item.transferredByCompany ? `(${item.transferredByCompany})` : "";
  return (
    <tr
      key={`row_${item.ctrId}`}
      id={`row_${item.ctrId}`}
      className={`row-group-${item.ctrId} ${
        rowIndex % 2 === 0 ? "odd-row" : "even-row"
      }`}
    >
      <td>{rowIndex}</td>
      <td className="text-left">{item.ctrId}</td>
      <td className="text-left">{item.fullName}</td>
      <td className="text-left">{item.fromNickName}</td>
      <td className="text-left">{item.fromBUName}</td>
      <td className="text-left">{item.fromDeptName}</td>
      <td className="text-left">{item.fromTeamName}</td>
      <td className="text-left">{item.fromCompanyName}</td>
      <td className="text-left">{item.toNickName}</td>
      <td className="text-left">{item.toBUName}</td>
      <td className="text-left">{item.toDeptName}</td>
      <td className="text-left">{item.toTeamName}</td>
      <td className="text-left">{item.toCompanyName}</td>
      <td className="text-left">
        {item.remark ? (
          <div className="tooltip-container">
            <i className="fa fa-comment" />
            <span className="tooltip-text">{item.remark}</span>
          </div>
        ) : (
          <>-</>
        )}
      </td>
      <td className="text-left">{moment(item.transferredDatetime).format("MMM DD YYYY")}</td>
      <td className="text-left">{`${item.transferredByNickName} ${transferredCompany}`}</td>
    </tr>
  );
};

export default Row;
