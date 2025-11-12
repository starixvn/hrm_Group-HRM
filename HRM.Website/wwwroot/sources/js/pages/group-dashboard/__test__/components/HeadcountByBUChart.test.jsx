import { render } from '@testing-library/react';
import React from 'react';
import HeadcountByBUChart from '../../components/HeadcountByBUChart';

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

describe('HeadcountByBUChart Component', () => {
    it('should render chart with valid data', () => {
        const data = [
            { label: 'IT', value: 50 },
            { label: 'HR', value: 30 }
        ];

        const { getByTestId } = render(<HeadcountByBUChart data={data} />);

        const chart = getByTestId('bar-chart');
        expect(chart).toBeInTheDocument();
        expect(JSON.parse(chart.getAttribute('data-labels'))).toEqual(['IT', 'HR']);
        expect(JSON.parse(chart.getAttribute('data-values'))).toEqual([50, 30]);
    });

    it('should render placeholder when data is empty', () => {
        const { getByText } = render(<HeadcountByBUChart data={[]} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });

    it('should render placeholder when data is null', () => {
        const { getByText } = render(<HeadcountByBUChart data={null} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });

    it('should render placeholder when data is undefined', () => {
        const { getByText } = render(<HeadcountByBUChart />);

        expect(getByText('No data available')).toBeInTheDocument();
    });
});

