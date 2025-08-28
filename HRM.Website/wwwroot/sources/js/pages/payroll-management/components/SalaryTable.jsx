import React from 'react';
import { EmploymentStatusColor } from '../constants/constants';

const SalaryTable = ({ data }) => {
    const formatNumber = number => new Intl.NumberFormat('en-US').format(number || 0);

    const getRowStyle = employmentStatus => {
        const color = EmploymentStatusColor[employmentStatus];
        return color ? { color: color } : {};
    };

    const keys = [
        "basicSalary",
        "standardWorkingDay",
        "unpaidLeave",
        "paidDay",
        "grossSalary",
    ];

    const totals = data.reduce((acc, item) => {
        keys.forEach(key => {
            acc[key] += item[key] || 0;
        });
        return acc;
    }, Object.fromEntries(keys.map(k => [k, 0])));

    return (
        <div className="salary-table-section">
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>No</th>
                        <th>ID Staff</th>
                        <th>Full Name</th>
                        <th>Nickname</th>
                        <th>Commencement Date</th>
                        <th>Confirmation Date</th>
                        <th>Basic Salary</th>
                        <th>Standard Working day</th>
                        <th>Unpaid Leave</th>
                        <th>Actual Paid Day</th>
                        <th>Gross salary</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr
                            key={index}
                            style={getRowStyle(item.employmentStatus)}>
                            <td>{index + 1}</td>
                            <td>{item.code}</td>
                            <td>{item.fullName}</td>
                            <td>{item.nickName}</td>
                            <td>{item.displayJoinedDate}</td>
                            <td>{item.displayConfirmedDate}</td>
                            <td>{formatNumber(item.basicSalary)}</td>
                            <td>{item.standardWorkingDay}</td>
                            <td>{item.unpaidLeave}</td>
                            <td>{item.paidDay}</td>
                            <td>{formatNumber(item.grossSalary)}</td>
                        </tr>
                    ))}
                </tbody>
                <tfoot>
                    <tr className="summary-row">
                        <td colSpan="6" className="text-end">
                            <strong>Total</strong>
                        </td>
                        <td>
                            <strong>{formatNumber(totals.basicSalary)}</strong>
                        </td>
                        <td>
                            <strong>{totals.standardWorkingDay}</strong>
                        </td>
                        <td>
                            <strong>{totals.unpaidLeave}</strong>
                        </td>
                        <td>
                            <strong>{totals.paidDay}</strong>
                        </td>
                        <td>
                            <strong>{formatNumber(totals.grossSalary)}</strong>
                        </td>
                    </tr>
                </tfoot>
            </table>
        </div>
    );
};

export default SalaryTable;