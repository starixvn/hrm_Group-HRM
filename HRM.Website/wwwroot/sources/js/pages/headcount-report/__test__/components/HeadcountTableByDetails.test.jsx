import { render } from '@testing-library/react';
import React from 'react';
import HeadcountTableByDetails from '../../components/HeadcountTableByDetails';

jest.mock('../../components/shared/TableWrapper', () => {
    const React = require('react');
    return function TableWrapper({ children }) {
        return React.createElement('table', { className: 'headcount-table' }, children);
    };
});

jest.mock('../../components/shared/TableBody', () => {
    const React = require('react');
    return function TableBody({ data, columns, useLabel3ForCategory }) {
        return React.createElement('tbody', null, 
            React.createElement('tr', null, 
                React.createElement('td', null, `Data: ${data.length}, Columns: ${columns.length}, useLabel3ForCategory: ${useLabel3ForCategory || false}`)
            )
        );
    };
});

const mockRenderFixedHeaders = jest.fn();
const mockRenderGroupedHeaders = jest.fn();

jest.mock('../../components/shared/TableHeaders', () => {
    const React = require('react');
    return {
        renderFixedHeaders: (rowspan) => {
            mockRenderFixedHeaders(rowspan);
            return React.createElement(React.Fragment, null,
                React.createElement('th', { key: 'month' }, 'Month'),
                React.createElement('th', { key: 'category' }, 'Category')
            );
        },
        renderGroupedHeaders: (groups, rowKey) => {
            mockRenderGroupedHeaders(groups, rowKey);
            return groups.map((cell, idx) => 
                React.createElement('th', { key: `row${rowKey}-${idx}` }, cell.label)
            );
        },
    };
});

const mockProcessColumns = jest.fn();
const mockGroupByLabel1 = jest.fn();
const mockGroupByLabel2 = jest.fn();
const mockGroupByLabel3 = jest.fn();

jest.mock('../../utils/tableUtils', () => ({
    processColumns: (data) => mockProcessColumns(data),
    groupByLabel1: (columns) => mockGroupByLabel1(columns),
    groupByLabel2: (columns) => mockGroupByLabel2(columns),
    groupByLabel3: (columns) => mockGroupByLabel3(columns),
}));

describe('HeadcountTableByDetails Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
        
        mockProcessColumns.mockImplementation((data) => 
            data.map(item => ({ 
                label: item.label1 || '', 
                label2: item.label2 || '',
                label3: item.label3 || '',
                key: item.label1 || '',
                isUnitCategory: item.isUnitCategory,
                isGrandTotal: item.isGrandTotal
            }))
        );
        
        mockGroupByLabel1.mockReturnValue([{ label: 'Unit 1', colspan: 1 }]);
        mockGroupByLabel2.mockReturnValue([{ label: 'Company 1', colspan: 1 }]);
        mockGroupByLabel3.mockReturnValue([{ label: 'Category 1', colspan: 1 }]);
    });

    it('should render table with data', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: [{ month: 1, newHire: 5, newHireEmpIds: [1, 2, 3, 4, 5] }]
            }
        ];

        const { container, getByText } = render(
            <HeadcountTableByDetails data={data} />
        );

        expect(container.querySelector('.headcount-table')).toBeInTheDocument();
        expect(getByText('Data: 1, Columns: 1, useLabel3ForCategory: false')).toBeInTheDocument();
    });

    it('should process columns correctly', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                isUnitCategory: false,
                isGrandTotal: false,
                details: []
            },
            {
                label1: 'Unit 2',
                label2: 'Company 2',
                label3: 'Category 2',
                isUnitCategory: true,
                isGrandTotal: true,
                details: []
            }
        ];

        render(<HeadcountTableByDetails data={data} />);

        expect(mockProcessColumns).toHaveBeenCalledWith(data);
        expect(mockProcessColumns).toHaveBeenCalledTimes(1);
    });

    it('should render three header rows', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            }
        ];

        const { container } = render(
            <HeadcountTableByDetails data={data} />
        );

        const thead = container.querySelector('thead');
        expect(thead).toBeInTheDocument();
        const rows = thead.querySelectorAll('tr');
        expect(rows.length).toBe(3);
    });

    it('should call renderFixedHeaders with rowspan 3', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            }
        ];

        render(<HeadcountTableByDetails data={data} />);

        expect(mockRenderFixedHeaders).toHaveBeenCalledWith(3);
        expect(mockRenderFixedHeaders).toHaveBeenCalledTimes(1);
    });

    it('should call renderGroupedHeaders for all three levels', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            }
        ];

        const processedColumns = mockProcessColumns(data);

        render(<HeadcountTableByDetails data={data} />);

        expect(mockGroupByLabel1).toHaveBeenCalledWith(processedColumns);
        expect(mockGroupByLabel2).toHaveBeenCalledWith(processedColumns);
        expect(mockGroupByLabel3).toHaveBeenCalledWith(processedColumns);

        expect(mockRenderGroupedHeaders).toHaveBeenCalledTimes(3);
        expect(mockRenderGroupedHeaders).toHaveBeenNthCalledWith(1, [{ label: 'Unit 1', colspan: 1 }], 1);
        expect(mockRenderGroupedHeaders).toHaveBeenNthCalledWith(2, [{ label: 'Company 1', colspan: 1 }], 2);
        expect(mockRenderGroupedHeaders).toHaveBeenNthCalledWith(3, [{ label: 'Category 1', colspan: 1 }], 3);
    });

    it('should pass correct props to TableBody', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            }
        ];

        const { getByText } = render(
            <HeadcountTableByDetails data={data} />
        );

        // TableBody should receive data and columns, but NOT useLabel3ForCategory
        expect(getByText('Data: 1, Columns: 1, useLabel3ForCategory: false')).toBeInTheDocument();
    });

    it('should handle empty data', () => {
        const { container, getByText } = render(
            <HeadcountTableByDetails data={[]} />
        );

        expect(container.querySelector('.headcount-table')).toBeInTheDocument();
        expect(getByText('Data: 0, Columns: 0, useLabel3ForCategory: false')).toBeInTheDocument();
    });

    it('should handle multiple columns with different groupings', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category A',
                details: []
            },
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category B',
                details: []
            },
            {
                label1: 'Unit 2',
                label2: 'Company 2',
                label3: 'Category A',
                details: []
            }
        ];

        mockGroupByLabel1.mockReturnValue([
            { label: 'Unit 1', colspan: 2 },
            { label: 'Unit 2', colspan: 1 }
        ]);
        mockGroupByLabel2.mockReturnValue([
            { label: 'Company 1', colspan: 2 },
            { label: 'Company 2', colspan: 1 }
        ]);
        mockGroupByLabel3.mockReturnValue([
            { label: 'Category A', colspan: 2 },
            { label: 'Category B', colspan: 1 }
        ]);

        const { getByText } = render(
            <HeadcountTableByDetails data={data} />
        );

        expect(getByText('Data: 3, Columns: 3, useLabel3ForCategory: false')).toBeInTheDocument();
        expect(mockGroupByLabel1).toHaveBeenCalled();
        expect(mockGroupByLabel2).toHaveBeenCalled();
        expect(mockGroupByLabel3).toHaveBeenCalled();
    });
});

