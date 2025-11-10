import { render } from '@testing-library/react';
import React from 'react';
import EmployeeByAgeChart from '../../components/EmployeeByAgeChart';

jest.mock('react-chartjs-2', () => ({
    Bar: ({ data, options }) => {
        const React = require('react');
        return React.createElement('div', { 
            'data-testid': 'bar-chart',
            'data-labels': JSON.stringify(data.labels),
            'data-values': JSON.stringify(data.datasets[0].data)
        });
    }
}));

describe('EmployeeByAgeChart Component', () => {
    it('should render chart with valid data', () => {
        const data = [
            { label: '20-24', value: 10 },
            { label: '25-29', value: 20 }
        ];

        const { getByTestId } = render(<EmployeeByAgeChart data={data} />);

        const chart = getByTestId('bar-chart');
        expect(chart).toBeInTheDocument();
        expect(JSON.parse(chart.getAttribute('data-labels'))).toEqual(['20-24', '25-29']);
        expect(JSON.parse(chart.getAttribute('data-values'))).toEqual([10, 20]);
    });

    it('should handle null or missing labels and values', () => {
        const data = [
            { label: null, value: 10 },
            { value: 20 },
            { label: '25-29', value: null }
        ];

        const { getByTestId } = render(<EmployeeByAgeChart data={data} />);

        const chart = getByTestId('bar-chart');
        expect(JSON.parse(chart.getAttribute('data-labels'))).toEqual(['', '', '25-29']);
        expect(JSON.parse(chart.getAttribute('data-values'))).toEqual([10, 20, 0]);
    });

    it('should render placeholder when data is empty', () => {
        const { getByText } = render(<EmployeeByAgeChart data={[]} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });

    it('should render placeholder when data is null', () => {
        const { getByText } = render(<EmployeeByAgeChart data={null} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });
});

