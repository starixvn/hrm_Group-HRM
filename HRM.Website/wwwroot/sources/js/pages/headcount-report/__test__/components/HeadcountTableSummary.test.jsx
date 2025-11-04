import { render } from '@testing-library/react';
import React from 'react';
import HeadcountTableSummary from '../../components/HeadcountTableSummary';

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

const mockGroupByLabel1 = jest.fn();
const mockGetUniqueColumnsByLabel1 = jest.fn();

jest.mock('../../utils/tableUtils', () => ({
    groupByLabel1: (columns) => mockGroupByLabel1(columns),
    getUniqueColumnsByLabel1: (data) => mockGetUniqueColumnsByLabel1(data),
}));

describe('HeadcountTableSummary Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
        
        const mockUniqueColumns = [
            { 
                label: 'Unit 1', 
                label2: 'Company 1',
                label3: 'Category 1',
                key: 'Unit 1',
                originalIndices: [0, 1],
                isUnitCategory: false,
                isGrandTotal: false
            },
            { 
                label: 'Unit 2', 
                label2: 'Company 2',
                label3: 'Category 2',
                key: 'Unit 2',
                originalIndices: [2],
                isUnitCategory: true,
                isGrandTotal: false
            }
        ];
        
        mockGetUniqueColumnsByLabel1.mockReturnValue({
            columns: mockUniqueColumns
        });
        
        mockGroupByLabel1.mockReturnValue([
            { label: 'Unit 1', colspan: 1 },
            { label: 'Unit 2', colspan: 1 }
        ]);
    });

    it('should render table with data', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: [{ month: 1, currentHeadCount: 10, currentHeadCountEmpIds: [1, 2, 3] }]
            },
            {
                label1: 'Unit 1',
                label2: 'Company 2',
                label3: 'Category 2',
                details: [{ month: 1, currentHeadCount: 5, currentHeadCountEmpIds: [4, 5] }]
            }
        ];

        const { container, getByText } = render(
            <HeadcountTableSummary data={data} />
        );

        expect(container.querySelector('.headcount-table')).toBeInTheDocument();
        expect(getByText('Data: 2, Columns: 2, useLabel3ForCategory: true')).toBeInTheDocument();
    });

    it('should call getUniqueColumnsByLabel1 with data', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            },
            {
                label1: 'Unit 1',
                label2: 'Company 2',
                label3: 'Category 2',
                details: []
            }
        ];

        render(<HeadcountTableSummary data={data} />);

        expect(mockGetUniqueColumnsByLabel1).toHaveBeenCalledWith(data);
        expect(mockGetUniqueColumnsByLabel1).toHaveBeenCalledTimes(1);
    });

    it('should use unique columns from getUniqueColumnsByLabel1', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            },
            {
                label1: 'Unit 1',
                label2: 'Company 2',
                label3: 'Category 2',
                details: []
            },
            {
                label1: 'Unit 1',
                label2: 'Company 3',
                label3: 'Category 3',
                details: []
            }
        ];

        const mockUniqueColumns = [
            { 
                label: 'Unit 1', 
                key: 'Unit 1',
                originalIndices: [0, 1, 2]
            }
        ];
        
        mockGetUniqueColumnsByLabel1.mockReturnValue({
            columns: mockUniqueColumns
        });

        render(<HeadcountTableSummary data={data} />);

        expect(mockGetUniqueColumnsByLabel1).toHaveBeenCalledWith(data);
        expect(mockGroupByLabel1).toHaveBeenCalledWith(mockUniqueColumns);
    });

    it('should render single header row', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            }
        ];

        const { container } = render(
            <HeadcountTableSummary data={data} />
        );

        const thead = container.querySelector('thead');
        expect(thead).toBeInTheDocument();
        const rows = thead.querySelectorAll('tr');
        expect(rows.length).toBe(1);
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

        render(<HeadcountTableSummary data={data} />);

        expect(mockRenderFixedHeaders).toHaveBeenCalledWith(3);
        expect(mockRenderFixedHeaders).toHaveBeenCalledTimes(1);
    });

    it('should call renderGroupedHeaders with groupByLabel1 result', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            }
        ];

        const mockUniqueColumns = [
            { 
                label: 'Unit 1', 
                key: 'Unit 1',
                originalIndices: [0]
            }
        ];
        
        mockGetUniqueColumnsByLabel1.mockReturnValue({
            columns: mockUniqueColumns
        });
        
        mockGroupByLabel1.mockReturnValue([
            { label: 'Unit 1', colspan: 1 }
        ]);

        render(<HeadcountTableSummary data={data} />);

        expect(mockGroupByLabel1).toHaveBeenCalledWith(mockUniqueColumns);
        expect(mockRenderGroupedHeaders).toHaveBeenCalledTimes(1);
        expect(mockRenderGroupedHeaders).toHaveBeenCalledWith([{ label: 'Unit 1', colspan: 1 }], 1);
    });

    it('should pass useLabel3ForCategory=true to TableBody', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            }
        ];

        const { getByText } = render(
            <HeadcountTableSummary data={data} />
        );

        expect(getByText('Data: 1, Columns: 2, useLabel3ForCategory: true')).toBeInTheDocument();
    });

    it('should handle empty data', () => {
        const mockEmptyColumns = [];
        
        mockGetUniqueColumnsByLabel1.mockReturnValue({
            columns: mockEmptyColumns
        });
        
        mockGroupByLabel1.mockReturnValue([]);

        const { container, getByText } = render(
            <HeadcountTableSummary data={[]} />
        );

        expect(container.querySelector('.headcount-table')).toBeInTheDocument();
        expect(getByText('Data: 0, Columns: 0, useLabel3ForCategory: true')).toBeInTheDocument();
    });

    it('should handle multiple unique columns', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            },
            {
                label1: 'Unit 1',
                label2: 'Company 2',
                label3: 'Category 2',
                details: []
            },
            {
                label1: 'Unit 2',
                label2: 'Company 3',
                label3: 'Category 3',
                details: []
            }
        ];

        const mockUniqueColumns = [
            { 
                label: 'Unit 1', 
                key: 'Unit 1',
                originalIndices: [0, 1]
            },
            { 
                label: 'Unit 2', 
                key: 'Unit 2',
                originalIndices: [2]
            }
        ];
        
        mockGetUniqueColumnsByLabel1.mockReturnValue({
            columns: mockUniqueColumns
        });
        
        mockGroupByLabel1.mockReturnValue([
            { label: 'Unit 1', colspan: 1 },
            { label: 'Unit 2', colspan: 1 }
        ]);

        const { getByText } = render(
            <HeadcountTableSummary data={data} />
        );

        expect(getByText('Data: 3, Columns: 2, useLabel3ForCategory: true')).toBeInTheDocument();
        expect(mockGetUniqueColumnsByLabel1).toHaveBeenCalledWith(data);
        expect(mockGroupByLabel1).toHaveBeenCalledWith(mockUniqueColumns);
    });

    it('should pass unique columns to TableBody', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category 1',
                details: []
            },
            {
                label1: 'Unit 1',
                label2: 'Company 2',
                label3: 'Category 2',
                details: []
            }
        ];

        const mockUniqueColumns = [
            { 
                label: 'Unit 1', 
                key: 'Unit 1',
                originalIndices: [0, 1]
            }
        ];
        
        mockGetUniqueColumnsByLabel1.mockReturnValue({
            columns: mockUniqueColumns
        });

        const { getByText } = render(
            <HeadcountTableSummary data={data} />
        );

        // Should pass unique columns (1) not original data length (2)
        expect(getByText('Data: 2, Columns: 1, useLabel3ForCategory: true')).toBeInTheDocument();
    });
});

