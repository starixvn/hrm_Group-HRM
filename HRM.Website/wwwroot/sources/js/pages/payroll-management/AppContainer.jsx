import React from 'react';
import toastr from 'toastr';
import SalaryTable from './components/SalaryTable';
import { toastOptions } from '../../shared/common.functions';
import Filter from './components/Filter';
import PayrollService from './Service';

const AppContainer = () => {
    const [salaryData, setSalaryData] = React.useState([]);

    const fetchSalaryData = async (filters = {}) => {
        const data = await PayrollService.getSalaryData(filters);
        setSalaryData(data);
    };

    const handleSearch = async (filters) => {
        await fetchSalaryData(filters);
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
