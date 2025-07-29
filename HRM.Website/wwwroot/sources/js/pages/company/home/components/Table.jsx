import React from "react";
import Row from "./Row";

const Table = ({ data }) => {
  const colSpan = 9;

  const columns = [
    { label: "#", key: null, width: "3%" },
    { label: "Company", key: "name", width: "15%" },
    { label: "BIO", key: "hasBIO", width: "4%" },
    { label: "Letstalk", key: "enableLetstalk", width: "4%" },
    { label: "Currency", key: "currency", width: "10%" },
    { label: "Whitelist IPs", key: "whitelistIPs", width: "25%" },
    { label: "URLs", key: "urls", width: "25%" },
    { label: "Sub Company", key: "subCompany", width: "10%" },
    { label: "Edit", key: null, width: "4%" }
  ];

  return (
    <table className="table table-custom">
      <thead>
        <tr>
          {columns.map(({ label, width }) => (
            <th key={label} style={{ width }}>
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
