import { render } from '@testing-library/react';
import React from 'react';
import SalaryExpenseByBUChart from '../../components/SalaryExpenseByBUChart';

jest.mock('react-chartjs-2', () => ({
    Bar: ({ data, _ }) => {
        const React = require('react');
        return React.createElement('div', { 
            'data-testid': 'bar-chart',
            'data-labels': JSON.stringify(data.labels),
            'data-values': JSON.stringify(data.datasets[0].data)
        });
    }
}));

jest.mock('chartjs-plugin-datalabels', () => ({}));

describe('SalaryExpenseByBUChart Component', () => {
    it('should render chart with valid data', () => {
        const data = [
            { label: 'IT', value: 50000 },
            { label: 'HR', value: 30000 }
        ];

        const { getByTestId } = render(<SalaryExpenseByBUChart data={data} />);

        const chart = getByTestId('bar-chart');
        expect(chart).toBeInTheDocument();
        expect(JSON.parse(chart.getAttribute('data-labels'))).toEqual(['IT', 'HR']);
        expect(JSON.parse(chart.getAttribute('data-values'))).toEqual([50000, 30000]);
    });

    it('should handle null or missing labels and values', () => {
        const data = [
            { label: null, value: 50000 },
            { value: 30000 },
            { label: 'Finance', value: null }
        ];

        const { getByTestId } = render(<SalaryExpenseByBUChart data={data} />);

        const chart = getByTestId('bar-chart');
        expect(JSON.parse(chart.getAttribute('data-labels'))).toEqual(['', '', 'Finance']);
        expect(JSON.parse(chart.getAttribute('data-values'))).toEqual([50000, 30000, 0]);
    });

    it('should render placeholder when data is empty', () => {
        const { getByText } = render(<SalaryExpenseByBUChart data={[]} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });

    it('should render placeholder when data is null', () => {
        const { getByText } = render(<SalaryExpenseByBUChart data={null} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });
});

