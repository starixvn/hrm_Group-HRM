import React, { useEffect, useState } from 'react';
import { blockUI, resolveClientUrl, unblockUI } from '../../../shared/common.functions';
import axios from 'axios';
import KPICard from './KPICard';
import HeadcountByBUChart from './HeadcountByBUChart';
import HeadcountByCountryChart from './HeadcountByCountryChart';
import BUCategoryBreakdown from './BUCategoryBreakdown';
import SalaryExpenseByBUCategoryChart from './SalaryExpenseByBUCategoryChart';
import SalaryExpenseByBUChart from './SalaryExpenseByBUChart';
import EmployeeByAgeChart from './EmployeeByAgeChart';
import SalaryExpenseByCountryByBU from './SalaryExpenseByCountryByBU';

const AppContainer = () => {
    const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());
    const [dashboardData, setDashboardData] = useState({
        kpis: {
            currentHeadcount: 0,
            recruitment: 0,
            probationResign: 0,
            confirmedResign: 0,
            companies: 0,
            avgAge: 0,
            avgYrsTenure: 0,
            countries: 0,
            businessUnits: 0
        },
        headcountByBU: [],
        headcountByCountry: [],
        buCategory: [],
        salaryExpenseByBUCategory: [],
        salaryExpenseByBU: [],
        employeeByAge: [],
        salaryExpenseByCountryByBU: []
    });

    const fetchDashboardData = async year => {
        blockUI();
        const response = await axios.get(
            resolveClientUrl(`Dashboard/GetDashboardData?year=${year}`),
        );
        const data = await response.data;
        unblockUI();

        setDashboardData({
            kpis: {
                currentHeadcount: data.currentHeadcount,
                recruitment: data.recruitment,
                probationResign: data.probationResign,
                confirmedResign: data.confirmedResign,
                companies: data.companies,
                avgAge: data.avgAge,
                avgYrsTenure: data.avgYrsTenure,
                countries: data.countries,
                businessUnits: data.businessUnits,
            },
            headcountByBU: data.headcountByBU,
            headcountByCountry: data.headcountByCountry,
            buCategory: data.buCategory,
            salaryExpenseByBUCategory: data.salaryExpenseByBUCategory,
            salaryExpenseByBU: data.salaryExpenseByBU,
            employeeByAge: data.employeeByAge,
            salaryExpenseByCountryByBU: data.salaryExpenseByCountryByBU
        });
    };

    useEffect(() => {
        fetchDashboardData(selectedYear);
    }, []);

    return (
        <div className="dashboard-container">
            <div className="dashboard-header-kpi-row">
                <div className="dashboard-header-right">
                    <KPICard label="Current Headcount" value={dashboardData.kpis.currentHeadcount} icon="users" />
                    <KPICard label={`Recruitment ${selectedYear}`} value={dashboardData.kpis.recruitment} icon="users" />
                    <KPICard label={`Probation Resign ${selectedYear}`} value={dashboardData.kpis.probationResign || 0} icon="users" />
                    <KPICard label={`Confirmed Resign ${selectedYear}`} value={dashboardData.kpis.confirmedResign || 0} icon="users" />
                    <KPICard label="Companies" value={dashboardData.kpis.companies} icon="users" />
                    <KPICard label="Avg. Age" value={dashboardData.kpis.avgAge} icon="user" />
                    <KPICard label="Avg. Yrs Tenure" value={dashboardData.kpis.avgYrsTenure} icon="clock-o" />
                    <KPICard label="Countries" value={dashboardData.kpis.countries} icon="building" />
                    <KPICard label="Business Units" value={dashboardData.kpis.businessUnits} icon="briefcase" />
                </div>
            </div>

            <div className="dashboard-content">
                <div className="dashboard-column">
                    <div className="chart-card">
                        <h3>Business Unit Category</h3>
                        <BUCategoryBreakdown data={dashboardData.buCategory} />
                    </div>

                    <div className="chart-card">
                        <h3>Headcount By Country</h3>
                        <HeadcountByCountryChart data={dashboardData.headcountByCountry} />
                    </div>
                </div>

                <div className="dashboard-column">
                    <div className="chart-card">
                        <h3>Headcount By Business Unit</h3>
                        <HeadcountByBUChart data={dashboardData.headcountByBU} />
                    </div>

                    <div className="chart-card">
                        <h3>Salary Expense By Business Unit</h3>
                        <SalaryExpenseByBUChart data={dashboardData.salaryExpenseByBU} />
                    </div>
                </div>

                <div className="dashboard-column">
                    <div className="chart-card">
                        <h3>Salary Expense By Business Unit Category</h3>
                        <SalaryExpenseByBUCategoryChart data={dashboardData.salaryExpenseByBUCategory} />
                    </div>

                    <div className="chart-card">
                        <h3>Number Of Employees By Age</h3>
                        <EmployeeByAgeChart data={dashboardData.employeeByAge} />
                    </div>

                    {/* <div className="chart-card">
                        <h3>Salary Expense By Country By Business Unit</h3>
                        <SalaryExpenseByCountryByBU data={dashboardData.salaryExpenseByCountryByBU} />
                    </div> */}
                </div>
            </div>
        </div>
    );
};

export default AppContainer;

