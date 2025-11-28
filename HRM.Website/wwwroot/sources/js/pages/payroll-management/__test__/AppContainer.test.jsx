import { render, waitFor } from '@testing-library/react';
import React from 'react';
import AppContainer from '../AppContainer';

const mockSetSalaryData = jest.fn();
jest.spyOn(React, 'useState').mockReturnValue([[], mockSetSalaryData]);

jest.mock('../service', () => ({
    getSalaryData: jest.fn(),
}));

jest.mock('toastr', () => ({
    error: jest.fn(),
}));

jest.mock('../../../shared/common.functions', () => ({
    toastOptions: jest.fn(() => ({})),
}));

let mockOnSearchCallback = null;

jest.mock('../components/Filter', () => {
    return function Filter({ onSearch }) {
        const React = require('react');
        mockOnSearchCallback = onSearch;
        return React.createElement(
            'div',
            { className: 'filter' },
            'Filter Component'
        );
    };
});

describe('AppContainer Component', () => {
    const mockPayrollService = require('../service');
    const mockToastr = require('toastr');

    beforeEach(() => {
        jest.clearAllMocks();
        mockOnSearchCallback = null;
        mockSetSalaryData.mockClear();
    });

    it('should render components and pass onSearch callback', () => {
        const { container } = render(<AppContainer />);

        expect(
            container.querySelector('.salary-management-container')
        ).toBeInTheDocument();
        expect(container.querySelector('.filter')).toBeInTheDocument();
        expect(container.querySelector('.salary-table')).toBeInTheDocument();
        expect(mockOnSearchCallback).toBeDefined();
        expect(typeof mockOnSearchCallback).toBe('function');
    });

    it('should fetch salary data successfully', async () => {
        const mockData = [
            {
                id: 1,
                code: 'EMP001',
                fullName: 'Test Employee',
                basicSalary: 5000,
                grossSalary: 6000,
            },
        ];
        mockPayrollService.getSalaryData.mockResolvedValue(mockData);

        render(<AppContainer />);

        await mockOnSearchCallback({ month: '2024-01-01', isLocal: true });
        expect(mockPayrollService.getSalaryData).toHaveBeenCalledWith({
            month: '2024-01-01',
            isLocal: true,
        });
        expect(mockSetSalaryData).toHaveBeenCalledWith({data: mockData});

        await mockOnSearchCallback();
        expect(mockPayrollService.getSalaryData).toHaveBeenCalledWith({});
    });
});
