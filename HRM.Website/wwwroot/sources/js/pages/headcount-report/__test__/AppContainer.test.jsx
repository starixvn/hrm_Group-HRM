import { render, waitFor, fireEvent, act } from '@testing-library/react';
import React from 'react';
import AppContainer from '../components/AppContainer';

jest.mock('axios', () => ({
    get: jest.fn(),
}));

jest.mock('../../../shared/common.functions', () => ({
    blockUI: jest.fn(),
    unblockUI: jest.fn(),
    resolveClientUrl: jest.fn((url) => url),
}));

let mockOnYearChangeCallback = null;
let mockOnSearchCallback = null;

jest.mock('../components/Filter', () => {
    return function Filter({ selectedYear, onYearChange, onSearch }) {
        const React = require('react');
        mockOnYearChangeCallback = onYearChange;
        mockOnSearchCallback = onSearch;
        return React.createElement(
            'div',
            { className: 'filter', 'data-selected-year': selectedYear },
            'Filter Component'
        );
    };
});

jest.mock('../components/HeadcountTableByUnit', () => {
    const React = require('react');
    return function HeadcountTableByUnit({ data }) {
        return React.createElement('div', { className: 'table-by-unit' }, `Unit Table: ${data.length} items`);
    };
});

jest.mock('../components/HeadcountTableByCompany', () => {
    const React = require('react');
    return function HeadcountTableByCompany({ data }) {
        return React.createElement('div', { className: 'table-by-company' }, `Company Table: ${data.length} items`);
    };
});

jest.mock('../components/HeadcountTableByDetails', () => {
    const React = require('react');
    return function HeadcountTableByDetails({ data }) {
        return React.createElement('div', { className: 'table-by-details' }, `Details Table: ${data.length} items`);
    };
});

jest.mock('../components/HeadcountTableSummary', () => {
    const React = require('react');
    return function HeadcountTableSummary({ data }) {
        return React.createElement('div', { className: 'table-summary' }, `Summary Table: ${data.length} items`);
    };
});

describe('AppContainer Component', () => {
    const mockAxios = require('axios');
    const mockCommonFunctions = require('../../../shared/common.functions');

    beforeEach(() => {
        jest.clearAllMocks();
        mockOnYearChangeCallback = null;
        mockOnSearchCallback = null;
        
        // Setup default axios mock for mount
        const defaultMockData = {
            reportByUnit: [],
            reportByCompany: [],
            reportByDetails: [],
            reportByUnitCountry: [],
            reportByUnitCountryLocal: [],
            reportByUnitCountryExpat: []
        };
        mockAxios.get.mockResolvedValue({
            data: defaultMockData
        });
    });

    it('should render components and tabs', async () => {
        const { container } = render(<AppContainer />);

        await waitFor(() => {
            expect(container.querySelector('.filter')).toBeInTheDocument();
            expect(container.querySelector('.nav-tabs')).toBeInTheDocument();
        });

        expect(mockOnYearChangeCallback).toBeDefined();
        expect(mockOnSearchCallback).toBeDefined();
    });

    it('should fetch report data on mount', async () => {
        const mockData = {
            reportByUnit: [{ label1: 'Unit 1' }],
            reportByCompany: [{ label1: 'Company 1' }],
            reportByDetails: [{ label1: 'Detail 1' }],
            reportByUnitCountry: [{ label1: 'Country 1' }],
            reportByUnitCountryLocal: [{ label1: 'Local 1' }],
            reportByUnitCountryExpat: [{ label1: 'Expat 1' }]
        };

        mockAxios.get.mockResolvedValueOnce({
            data: mockData
        });

        render(<AppContainer />);

        await waitFor(() => {
            expect(mockAxios.get).toHaveBeenCalled();
            expect(mockCommonFunctions.blockUI).toHaveBeenCalled();
            expect(mockCommonFunctions.unblockUI).toHaveBeenCalled();
        });
    });

    it('should handle search and fetch data', async () => {
        const mockData = {
            reportByUnit: [{ label1: 'Unit 1' }],
            reportByCompany: [],
            reportByDetails: [],
            reportByUnitCountry: [],
            reportByUnitCountryLocal: [],
            reportByUnitCountryExpat: []
        };

        // First call is for mount, second call is for search
        mockAxios.get
            .mockResolvedValueOnce({ data: { reportByUnit: [], reportByCompany: [], reportByDetails: [], reportByUnitCountry: [], reportByUnitCountryLocal: [], reportByUnitCountryExpat: [] } }) // for mount
            .mockResolvedValueOnce({ data: mockData }); // for search

        const { container } = render(<AppContainer />);

        // Wait for mount to complete
        await waitFor(() => {
            expect(mockAxios.get).toHaveBeenCalledTimes(1);
        });

        // Wait for callbacks to be set
        await waitFor(() => {
            expect(mockOnSearchCallback).toBeDefined();
        });

        // Call search callback within act
        await act(async () => {
            mockOnSearchCallback();
        });

        await waitFor(() => {
            expect(mockAxios.get).toHaveBeenCalledTimes(2);
            // Verify the table updates with new data
            const unitTable = container.querySelector('.table-by-unit');
            expect(unitTable).toBeInTheDocument();
            expect(unitTable.textContent).toContain('Unit Table: 1 items');
        });
    });

    it('should switch tabs when clicking tab', async () => {
        const { container } = render(<AppContainer />);

        // Wait for initial render
        await waitFor(() => {
            expect(container.querySelector('.nav-tabs')).toBeInTheDocument();
        });

        const tabs = container.querySelectorAll('.nav-tabs li');
        expect(tabs.length).toBe(6);
        
        // Initially, first tab should be active
        expect(tabs[0].className).toContain('active');
        expect(container.querySelector('.table-by-unit')).toBeInTheDocument();
        
        // Click on the second tab
        await act(async () => {
            fireEvent.click(tabs[1]);
        });
        
        // Wait for tab switch to complete
        await waitFor(() => {
            // Second tab should now be active
            expect(tabs[1].className).toContain('active');
            // Company table should be rendered
            expect(container.querySelector('.table-by-company')).toBeInTheDocument();
        });
    });

    it('should render correct table based on active tab', async () => {
        const mockReportData = {
            reportByUnit: [],
            reportByCompany: [{ label1: 'Company 1' }],
            reportByDetails: [],
            reportByUnitCountry: [],
            reportByUnitCountryLocal: [],
            reportByUnitCountryExpat: []
        };
        
        mockAxios.get.mockResolvedValueOnce({
            data: mockReportData
        });

        const { container } = render(<AppContainer />);
        
        // Wait for initial render
        await waitFor(() => {
            expect(container.querySelector('.nav-tabs')).toBeInTheDocument();
        });

        // Click on the Company tab (second tab, id = 1)
        const tabs = container.querySelectorAll('.nav-tabs li');
        await act(async () => {
            fireEvent.click(tabs[1]);
        });
        
        // Wait for component to render with the correct tab and data
        await waitFor(() => {
            const companyTable = container.querySelector('.table-by-company');
            expect(companyTable).toBeInTheDocument();
            expect(companyTable.textContent).toContain('Company Table: 1 items');
        }, { timeout: 3000 });

        // Verify the correct table is rendered
        expect(container.querySelector('.table-by-company')).toBeInTheDocument();
        expect(container.querySelector('.table-by-unit')).not.toBeInTheDocument();
    });
});
