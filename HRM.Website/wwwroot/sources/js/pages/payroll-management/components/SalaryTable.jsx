import React from "react";
import { getPeriodTypeLabel } from "../constants/periodType";

const SalaryTable = { data } => {
    const formatNumber = number => {
        return new Intl.NumberFormat("en-US").format(number || 0);
    };

    const totals = data.reduce(
        (acc, item) => ({
            personalCost: (acc.personalCost || 0) + (item.personalCost || 0),
            extraPayment: (acc.extraPayment || 0) + (item.extraPayment || 0),
        }),
        { personalCost: 0, extraPayment: 0 }
    );

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
                    {data.length > 0 &&
                        data.map((item, index) => (
                            <tr key={index}>
                                <td>{index + 1}</td>
                                <td>{item.monthFormatted}</td>
                                <td>{getPeriodTypeLabel(item.periodType)}</td>
                                <td>{item.code}</td>
                                <td>{item.fullName}</td>
                                <td>{item.nickname}</td>
                                <td>{item.departmentName}</td>
                                <td>{item.teamName}</td>
                                <td>{formatNumber(item.personalCost)}</td>
                                <td>{formatNumber(item.extraPayment)}</td>
                            </tr>
                        ))}
                </tbody>
                <tfoot>
                    <tr className="summary-row">
                        <td colSpan="8" className="text-end">
                            <strong>Total</strong>
                        </td>
                        <td>
                            <strong>{formatNumber(totals.personalCost)}</strong>
                        </td>
                        <td>
                            <strong>{formatNumber(totals.extraPayment)}</strong>
                        </td>
                    </tr>
                </tfoot>
            </table>
        </div>
    );
};

export default SalaryTable;