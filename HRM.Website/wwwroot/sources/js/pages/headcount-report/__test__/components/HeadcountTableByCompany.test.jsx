import { render } from '@testing-library/react';
import React from 'react';
import HeadcountTableByCompany from '../../components/HeadcountTableByCompany';

jest.mock('../../components/shared/TableWrapper', () => {
    const React = require('react');
    return function TableWrapper({ children }) {
        return React.createElement('table', { className: 'headcount-table' }, children);
    };
});

jest.mock('../../components/shared/TableBody', () => {
    const React = require('react');
    return function TableBody({ data, columns }) {
        return React.createElement('tbody', null, 
            React.createElement('tr', null, 
                React.createElement('td', null, `Data: ${data.length}, Columns: ${columns.length}`)
            )
        );
    };
});

jest.mock('../../components/shared/TableHeaders', () => {
    const React = require('react');
    return {
        renderFixedHeaders: jest.fn(() => React.createElement(React.Fragment, null,
            React.createElement('th', { key: 'month' }, 'Month'),
            React.createElement('th', { key: 'category' }, 'Category')
        )),
        renderGroupedHeaders: jest.fn((groups, rowKey) => 
            groups.map((cell, idx) => 
                React.createElement('th', { key: `row${rowKey}-${idx}` }, cell.label)
            )
        ),
    };
});

jest.mock('../../utils/tableUtils', () => ({
    processColumns: jest.fn((data) => data.map(item => ({ 
        label: item.label1 || '', 
        label2: item.label2 || '',
        key: item.label1 || '' 
    }))),
    groupByLabel1: jest.fn(() => [{ label: 'Unit 1', colspan: 1 }]),
    groupByLabel2: jest.fn(() => [{ label: 'Company 1', colspan: 1 }]),
}));

describe('HeadcountTableByCompany Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('should render table with data', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                details: [{ month: 1, newHire: 5, newHireEmpIds: [1, 2, 3, 4, 5] }]
            }
        ];

        const { container, getByText } = render(
            <HeadcountTableByCompany data={data} />
        );

        expect(container.querySelector('.headcount-table')).toBeInTheDocument();
        expect(getByText('Data: 1, Columns: 1')).toBeInTheDocument();
    });

    it('should render multiple header rows', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                details: []
            },
            {
                label1: 'Unit 1',
                label2: 'Company 2',
                details: []
            }
        ];

        const { container } = render(
            <HeadcountTableByCompany data={data} />
        );

        const thead = container.querySelector('thead');
        expect(thead).toBeInTheDocument();
        const rows = thead.querySelectorAll('tr');
        expect(rows.length).toBe(2);
    });

    it('should handle empty data', () => {
        const { container, getByText } = render(
            <HeadcountTableByCompany data={[]} />
        );

        expect(container.querySelector('.headcount-table')).toBeInTheDocument();
        expect(getByText('Data: 0, Columns: 0')).toBeInTheDocument();
    });

    it('should process columns with label2', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                details: []
            },
            {
                label1: 'Unit 1',
                label2: 'Company 2',
                details: []
            }
        ];

        const { getByText } = render(
            <HeadcountTableByCompany data={data} />
        );

        expect(getByText('Data: 2, Columns: 2')).toBeInTheDocument();
    });
});

