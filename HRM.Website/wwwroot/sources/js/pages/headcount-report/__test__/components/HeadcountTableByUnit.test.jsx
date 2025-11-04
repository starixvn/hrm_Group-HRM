import { render } from '@testing-library/react';
import React from 'react';
import HeadcountTableByUnit from '../../components/HeadcountTableByUnit';

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
    processColumns: jest.fn((data) => data.map(item => ({ label: item.label1 || '', key: item.label1 || '' }))),
    groupByLabel1: jest.fn(() => [{ label: 'Unit 1', colspan: 1 }]),
}));

describe('HeadcountTableByUnit Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('should render table with data', () => {
        const data = [
            {
                label1: 'Unit 1',
                details: [{ month: 1, newHire: 5, newHireEmpIds: [1, 2, 3, 4, 5] }]
            }
        ];

        const { container, getByText } = render(
            <HeadcountTableByUnit data={data} />
        );

        expect(container.querySelector('.headcount-table')).toBeInTheDocument();
        expect(getByText('Data: 1, Columns: 1')).toBeInTheDocument();
    });

    it('should return null when data is empty', () => {
        const { container } = render(<HeadcountTableByUnit data={[]} />);

        expect(container.firstChild).toBeNull();
    });

    it('should return null when data is null', () => {
        const { container } = render(<HeadcountTableByUnit data={null} />);

        expect(container.firstChild).toBeNull();
    });

    it('should process columns correctly', () => {
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                isUnitCategory: false,
                isGrandTotal: false,
                details: []
            },
            {
                label1: 'Unit 2',
                label2: 'Company 2',
                isUnitCategory: true,
                isGrandTotal: false,
                details: []
            }
        ];

        const { getByText } = render(
            <HeadcountTableByUnit data={data} />
        );

        expect(getByText('Data: 2, Columns: 2')).toBeInTheDocument();
    });

    it('should render headers correctly', () => {
        const data = [
            {
                label1: 'Unit 1',
                details: []
            }
        ];

        const { container } = render(
            <HeadcountTableByUnit data={data} />
        );

        const thead = container.querySelector('thead');
        expect(thead).toBeInTheDocument();
    });
});

