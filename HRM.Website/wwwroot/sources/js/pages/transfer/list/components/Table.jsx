import React, { useState } from "react";
import Row from "./Row";

const Table = ({ data }) => {
  const [sortConfig, setSortConfig] = useState({ key: null, direction: null });

  const columns = [
    { label: "#", key: null, sortable: false, children: null },
    { label: "Ctr ID", key: "ctrId", sortable: true, children: null },
    { label: "Full Name", key: "fullName", sortable: true, children: null },
    {
      label: "From Company",
      key: "fromCompany",
      sortable: false,
      children: [
        { label: "Nickname", key: "fromNickName", sortable: true },
        { label: "BU", key: "fromBUName", sortable: true },
        { label: "Dept", key: "fromDeptName", sortable: true },
        { label: "Team", key: "fromTeamName", sortable: true },
        { label: "Company", key: "fromCompanyName", sortable: true },
      ],
    },
    {
      label: "To Company",
      key: "toCompany",
      sortable: false,
      children: [
        { label: "Nickname", key: "toNickName", sortable: true },
        { label: "BU", key: "toBUName", sortable: true },
        { label: "Dept", key: "toDeptName", sortable: true },
        { label: "Team", key: "toTeamName", sortable: true },
        { label: "Company", key: "toCompanyName", sortable: true },
      ],
    },
    { label: "Remark", key: "remark", sortable: false, children: null },
    {
      label: "Transferred Date",
      key: "transferredDate",
      sortable: true,
    },
    {
      label: "Transferred By",
      key: "transferredBy",
      sortable: true,
    },
  ];
  const childrenSpan = columns
    .flatMap(col => (col.children ? col.children.length : 0))
    .reduce((acc, curr) => acc + curr, 0);
  const colSpan = columns.length + childrenSpan;

  const getCellValue = (item, key) => {
    if (!key) {
      return "";
    }
    const value = item[key];
    if (value === null || value === undefined) {
      return "";
    }
    if (typeof value === "string") {
      return value.trim().toLowerCase();
    }
    if (typeof value === "boolean") {
      return value ? "true" : "false";
    }

    return value;
  };

  const sortedData = React.useMemo(() => {
    if (!sortConfig.key) {
      return data;
    }

    const sorted = [...data].sort((a, b) => {
      const valA = getCellValue(a, sortConfig.key);
      const valB = getCellValue(b, sortConfig.key);

      if (valA === "" && valB !== "") {
        return 1;
      }
      if (valB === "" && valA !== "") {
        return -1;
      }
      if (valA === "" && valB === "") {
        return 0;
      }

      if (!isNaN(valA) && !isNaN(valB)) {
        return valA - valB;
      }

      return valA.localeCompare(valB, "vi", { sensitivity: "base" });
    });

    if (sortConfig.direction === "desc") {
      sorted.reverse();
    }

    return sorted;
  }, [data, sortConfig]);

  const onSort = key => {
    if (!key) {
      return;
    }

    let direction = "asc";
    if (sortConfig.key === key && sortConfig.direction === "asc") {
      direction = "desc";
    }

    setSortConfig({ key, direction });
  };

  const renderSortIcon = key => {
    if (sortConfig.key !== key) {
      return "fa fa-sort";
    }

    return sortConfig.direction === "asc"
      ? "fa fa-sort-asc"
      : "fa fa-sort-desc";
  };

  return (
    <table className="table table-custom transfer-list-table">
      <thead>
        <tr>
          {columns.map(col => (
            <th
              key={col.key}
              colSpan={col.children ? col.children.length : 1}
              rowSpan={col.children ? 1 : 2}
              onClick={() => col.sortable && onSort(col.key)}
              style={{
                cursor: col.sortable ? "pointer" : "default",
                textAlign: "center",
              }}
            >
              {col.label}
              {col.sortable && (
                <i
                  className={renderSortIcon(col.key)}
                  style={{ marginLeft: 6 }}
                />
              )}
            </th>
          ))}
        </tr>

        <tr>
          {columns.flatMap(col =>
            col.children
              ? col.children.map(child => (
                  <th
                    key={child.key}
                    onClick={() => child.sortable && onSort(child.key)}
                    style={{
                      cursor: child.sortable ? "pointer" : "default",
                      textAlign: "center",
                    }}
                  >
                    {child.label}
                    {child.sortable && (
                      <i
                        className={renderSortIcon(child.key)}
                        style={{ marginLeft: 6 }}
                      />
                    )}
                  </th>
                ))
              : []
          )}
        </tr>
      </thead>
      <tbody>
        {sortedData.length > 0 ? (
          sortedData.map((item, index) => (
            <Row
              key={`transferred_${index}`}
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
