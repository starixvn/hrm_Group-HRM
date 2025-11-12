import { render } from '@testing-library/react';
import React from 'react';
import SalaryExpenseByBUCategoryChart from '../../components/SalaryExpenseByBUCategoryChart';

jest.mock('react-chartjs-2', () => ({
    Doughnut: ({ data, _ }) => {
        const React = require('react');
        return React.createElement('div', { 
            'data-testid': 'doughnut-chart',
            'data-labels': JSON.stringify(data.labels),
            'data-values': JSON.stringify(data.datasets[0].data)
        });
    }
}));

jest.mock('chartjs-plugin-datalabels', () => ({}));

describe('SalaryExpenseByBUCategoryChart Component', () => {
    it('should render chart with valid data', () => {
        const data = [
            { label: 'Technology', value: 100000 },
            { label: 'Administration', value: 50000 }
        ];

        const { getByTestId } = render(<SalaryExpenseByBUCategoryChart data={data} />);

        const chart = getByTestId('doughnut-chart');
        expect(chart).toBeInTheDocument();
        expect(JSON.parse(chart.getAttribute('data-labels'))).toEqual(['Technology', 'Administration']);
        expect(JSON.parse(chart.getAttribute('data-values'))).toEqual([100000, 50000]);
    });

    it('should handle null labels gracefully', () => {
        const data = [
            { label: null, value: 100000 },
            { label: 'Admin', value: 50000 }
        ];

        const { getByTestId } = render(<SalaryExpenseByBUCategoryChart data={data} />);

        const chart = getByTestId('doughnut-chart');
        expect(JSON.parse(chart.getAttribute('data-labels'))).toEqual([null, 'Admin']);
    });

    it('should render placeholder when data is empty', () => {
        const { getByText } = render(<SalaryExpenseByBUCategoryChart data={[]} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });

    it('should render placeholder when data is null', () => {
        const { getByText } = render(<SalaryExpenseByBUCategoryChart data={null} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });
});

