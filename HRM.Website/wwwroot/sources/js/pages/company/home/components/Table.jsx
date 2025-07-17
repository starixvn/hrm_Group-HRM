import React from "react";
import Row from "./Row";

const Table = ({ data }) => {
  const colSpan = 7;

  const columns = [
    { label: "#", key: null },
    { label: "Company", key: "name" },
    { label: "BIO", key: "hasBIO" },
    { label: "Let's Talk", key: "enableLetstalk" },
    { label: "Whitelist IPs", key: "whitelistIPs" },
    { label: "Currency", key: "currency" },
    { label: "Edit", key: null }
  ];

  return (
    <table className="table table-custom">
      <thead>
        <tr>
          {columns.map(({ label }) => (
            <th key={label}>
              <div className="cell-control-wrapper">
                {label}
              </div>
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {data && data.length > 0 ? (
          data.map((item, index) => (
            <Row
              key={`${item.companyId}-${index}`}
              rowIndex={index + 1}
              item={item}
            />
          ))
        ) : (
          <tr>
            <td colSpan={colSpan}>No information available</td>
          </tr>
        )}
      </tbody>
    </table>
  );
};

export default Table;
