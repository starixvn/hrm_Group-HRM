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
                        <th>Full Name</th>
                        <th>Nickname</th>
                        <th>Department</th>
                        <th>Team</th>
                        <th>Personal Cost</th>
                        <th>Extra Payment</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr key={index}>
                            <td>{index + 1}</td>
                            <td>{item.monthFormatted}</td>
                            <td>{getPeriodTypeLabel(item.periodType)}</td>
                            <td>{item.empId}</td>
                            <td>{item.fullName}</td>
                            <td>{item.nickname}</td>
                            <td>{item.departmentName}</td>
                            <td>{item.teamName}</td>
                            <td>{item.personalCost}</td>
                            <td>{item.extraPayment}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default SalaryTable;
