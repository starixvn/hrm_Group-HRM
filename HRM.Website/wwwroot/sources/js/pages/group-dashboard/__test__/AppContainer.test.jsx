import { render, waitFor } from '@testing-library/react';
import React from 'react';
import axios from 'axios';
import AppContainer from '../components/AppContainer';

jest.mock('axios');
jest.mock('../../../shared/common.functions', () => ({
    blockUI: jest.fn(),
    unblockUI: jest.fn(),
    resolveClientUrl: jest.fn((url) => url),
}));

jest.mock('../components/KPICard', () => {
    const React = require('react');
    return function KPICard({ label, value }) {
        return React.createElement('div', { className: 'kpi-card' }, `${label}: ${value}`);
    };
});

jest.mock('../components/HeadcountByBUChart', () => {
    const React = require('react');
    return function HeadcountByBUChart({ data }) {
        return React.createElement('div', { className: 'headcount-by-bu' }, `BU Chart: ${data.length} items`);
    };
});

jest.mock('../components/HeadcountByCountryChart', () => {
    const React = require('react');
    return function HeadcountByCountryChart({ data }) {
        return React.createElement('div', { className: 'headcount-by-country' }, `Country Chart: ${data.length} items`);
    };
});

jest.mock('../components/BUCategoryBreakdown', () => {
    const React = require('react');
    return function BUCategoryBreakdown({ data }) {
        return React.createElement('div', { className: 'bu-category' }, `Category: ${data.length} items`);
    };
});

jest.mock('../components/SalaryExpenseByBUCategoryChart', () => {
    const React = require('react');
    return function SalaryExpenseByBUCategoryChart({ data }) {
        return React.createElement('div', { className: 'salary-by-category' }, `Salary Category: ${data.length} items`);
    };
});

jest.mock('../components/SalaryExpenseByBUChart', () => {
    const React = require('react');
    return function SalaryExpenseByBUChart({ data }) {
        return React.createElement('div', { className: 'salary-by-bu' }, `Salary BU: ${data.length} items`);
    };
});

jest.mock('../components/EmployeeByAgeChart', () => {
    const React = require('react');
    return function EmployeeByAgeChart({ data }) {
        return React.createElement('div', { className: 'employee-by-age' }, `Age Chart: ${data.length} items`);
    };
});

describe('AppContainer Component', () => {
    const mockCommonFunctions = require('../../../shared/common.functions');

    beforeEach(() => {
        jest.clearAllMocks();
        const mockData = {
            currentHeadcount: 100,
            recruitment: 10,
            probationResign: 2,
            confirmedResign: 3,
            companies: 5,
            avgAge: 30,
            avgYrsTenure: 5,
            countries: 3,
            businessUnits: 10,
            headcountByBU: [{ label: 'IT', value: 50 }],
            headcountByCountry: [{ label: 'Vietnam', value: 60 }],
            buCategory: [{ category: 'Technology', employeeCount: 40, buCount: 3 }],
            salaryExpenseByBUCategory: [{ label: 'Technology', value: 100000 }],
            salaryExpenseByBU: [{ label: 'IT', value: 50000 }],
            employeeByAge: [{ label: '20-24', value: 10 }],
        };
        axios.get.mockResolvedValue({ data: mockData });
    });

    it('should render and fetch dashboard data on mount', async () => {
        const { container } = render(<AppContainer />);

        await waitFor(() => {
            expect(container.querySelector('.dashboard-container')).toBeInTheDocument();
            expect(mockCommonFunctions.blockUI).toHaveBeenCalled();
            expect(mockCommonFunctions.unblockUI).toHaveBeenCalled();
        });

        expect(axios.get).toHaveBeenCalledWith(
            expect.stringContaining('Dashboard/GetDashboardData')
        );
    });

    it('should render all KPI cards and charts with data', async () => {
        const { container } = render(<AppContainer />);

        await waitFor(() => {
            expect(container.querySelector('.headcount-by-bu')).toBeInTheDocument();
            expect(container.querySelector('.headcount-by-country')).toBeInTheDocument();
            expect(container.querySelector('.bu-category')).toBeInTheDocument();
            expect(container.querySelector('.salary-by-category')).toBeInTheDocument();
            expect(container.querySelector('.salary-by-bu')).toBeInTheDocument();
            expect(container.querySelector('.employee-by-age')).toBeInTheDocument();
        });
    });

    it('should handle empty data gracefully', async () => {
        axios.get.mockResolvedValue({
            data: {
                currentHeadcount: 0,
                recruitment: 0,
                probationResign: 0,
                confirmedResign: 0,
                companies: 0,
                avgAge: 0,
                avgYrsTenure: 0,
                countries: 0,
                businessUnits: 0,
                headcountByBU: [],
                headcountByCountry: [],
                buCategory: [],
                salaryExpenseByBUCategory: [],
                salaryExpenseByBU: [],
                employeeByAge: [],
            }
        });

        const { container } = render(<AppContainer />);

        await waitFor(() => {
            expect(container.querySelector('.dashboard-container')).toBeInTheDocument();
        });
    });
});

