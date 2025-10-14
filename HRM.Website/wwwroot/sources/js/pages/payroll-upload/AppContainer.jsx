import React from 'react';
import toastr from 'toastr';
import SalaryTable from './components/SalaryTable';
import { toastOptions } from '../../shared/common.functions';
import Filter from './components/Filter';
import PayrollService from './service';

const AppContainer = () => {
    const [salaryData, setSalaryData] = React.useState([]);
    const fetchSalaryData = (filters = {}) => {
        PayrollService.getSalaryData(filters)
            .then(data => {
                setSalaryData(data);
            })
            .catch(_ => {
                toastr.error('Failed to fetch salary data', '', toastOptions());
            });
    };

    const handleSearch = filters => {
        fetchSalaryData(filters);
    };

    return (
        <div className="salary-management-container">
            <div className="filter">
                <Filter onSearch={handleSearch} />
            </div>
            <div className="salary-table">
                <SalaryTable data={salaryData} />
            </div>
        </div>
    );
};

export default AppContainer;
