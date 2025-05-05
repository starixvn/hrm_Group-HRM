import React from "react";

const SalaryTable = ({ data }) => {
    return (
        <div className="salary-table-section">
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>Staff ID</th>
                        <th>Total Personal Cost</th>
                        <th>Month</th>
                        <th>Period Type</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr key={index}>
                            <td>{item.staffId}</td>
                            <td>{item.totalPersonalCost}</td>
                            <td>{item.month}</td>
                            <td>
                                {item.periodType === "1"
                                    ? "1st Half"
                                    : "2nd Half"}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default SalaryTable;
