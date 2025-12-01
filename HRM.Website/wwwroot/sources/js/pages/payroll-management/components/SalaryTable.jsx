import React from 'react';
import { EmploymentStatusColor } from '../constants/constants';

const SalaryTable = ({ data }) => {
    const formatNumber = (number) => {
        var formattedNumber = new Intl.NumberFormat('en-US').format(number || 0);
        if(formattedNumber === '0') {
            return '-';
        }
        return formattedNumber;
    };

    const getRowStyle = employmentStatus => {
        const color = EmploymentStatusColor[employmentStatus];
        return color ? { color: color } : {};
    };

    const keys = [
        "basicSalary",
        "jobLevelAddOn",
        "specialSubsidy",
        "standardWorkingDay",
        "unpaidLeave",
        "paidDay",
        "grossSalary",
        "totalIncome",
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
                        <th rowSpan="2">No</th>
                        <th rowSpan="2">BU</th>
                        <th rowSpan="2">Dept</th>
                        <th rowSpan="2">Team</th>
                        <th rowSpan="2">ID Staff</th>
                        <th rowSpan="2">Full Name</th>
                        <th rowSpan="2">Nickname</th>
                        <th rowSpan="2">Commencement Date</th>
                        <th rowSpan="2">Confirmation Date</th>
                        <th rowSpan="2">Basic Salary</th>
                        <th rowSpan="2">Job Level Add-On</th>
                        <th rowSpan="2">Special Subsidy</th>
                        <th rowSpan="2">Standard Working day</th>
                        <th rowSpan="2">Unpaid Leave</th>
                        <th rowSpan="2">Actual Paid Day</th>
                        <th colSpan="1">Taxable Income</th>
                        <th rowSpan="2">Total Income</th>
                    </tr>
                    <tr>
                        <th style={{borderLeft: 'none'}}>Gross Salary</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr
                            key={index}
                            style={getRowStyle(item.employmentStatus)}>
                            <td>{index + 1}</td>
                            <td>{item.buCode}</td>
                            <td>{item.departmentCode}</td>
                            <td>{item.teamCode}</td>
                            <td>{item.code}</td>
                            <td>{item.fullName}</td>
                            <td>{item.nickName}</td>
                            <td>{item.displayJoinedDate}</td>
                            <td>{item.displayConfirmedDate}</td>
                            <td>{formatNumber(item.basicSalary)}</td>
                            <td>{formatNumber(item.jobLevelAddOn)}</td>
                            <td>{formatNumber(item.specialSubsidy)}</td>
                            <td>{item.standardWorkingDay}</td>
                            <td>{item.unpaidLeave}</td>
                            <td>{item.paidDay}</td>
                            <td>{formatNumber(item.grossSalary)}</td>
                            <td>{formatNumber(item.totalIncome)}</td>
                        </tr>
                    ))}
                </tbody>
                <tfoot>
                    <tr className="summary-row">
                        <td colSpan="9" className="text-end">
                            <strong>Total</strong>
                        </td>
                        <td>
                            <strong>{formatNumber(totals.basicSalary)}</strong>
                        </td>
                        <td>
                            <strong>{formatNumber(totals.jobLevelAddOn)}</strong>
                        </td>
                        <td>
                            <strong>{formatNumber(totals.specialSubsidy)}</strong>
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
                        <td>
                            <strong>{formatNumber(totals.totalIncome)}</strong>
                        </td>
                    </tr>
                </tfoot>
            </table>
        </div>
    );
};

export default SalaryTable;