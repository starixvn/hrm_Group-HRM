import React from "react";
import { getPeriodTypeLabel } from "../constants/periodType";

const SalaryTable = ({ data }) => {
    return (
        <div className="salary-table-section">
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Month</th>
                        <th>Period Type</th>
                        <th>Staff ID</th>
                        <th>Total Personal Cost</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr key={index}>
                            <td>{index + 1}</td>
                            <td>{item.month}</td>
                            <td>{getPeriodTypeLabel(item.periodType)}</td>
                            <td>{item.staffId}</td>
                            <td>{item.totalPersonalCost}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default SalaryTable;
