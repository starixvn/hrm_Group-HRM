import React from "react";
import { PayrollType } from "../../../shared/constants";

const SalaryTable = ({ data }) => {
    return (
        <div className="salary-table-section">
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>Department</th>
                        <th>Team</th>
                        <th>Employee</th>
                        <th>Personal Cost</th>
                        <th>Extra Payment</th>
                        <th>Total</th>
                        <th>Month</th>
                        <th>Period Type</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr key={index}>
                            <td align="left">{item.departmentName}</td>
                            <td>{item.teamName}</td>
                            <td>{item.nickname}</td>
                            <td>{item.personalCost}</td>
                            <td>{item.extraPayment}</td>
                            <td>{item.personalCost + item.extraPayment}</td>
                            <td>{item.monthFormatted}</td>
                            <td>
                                {(() => {
                                    switch (item.periodType) {
                                        case PayrollType.FirstHalf:
                                            return "1st Half";
                                        case PayrollType.SecondHalf:
                                            return "2nd Half";
                                        default:
                                            return "Full Month";
                                    }
                                })()}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default SalaryTable;
