import { render } from '@testing-library/react';
import React from 'react';
import HeadcountByCountryChart from '../../components/HeadcountByCountryChart';

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

describe('HeadcountByCountryChart Component', () => {
    it('should render chart with valid data', () => {
        const data = [
            { label: 'Vietnam', value: 60 },
            { label: 'Singapore', value: 40 }
        ];

        const { getByTestId } = render(<HeadcountByCountryChart data={data} />);

        const chart = getByTestId('doughnut-chart');
        expect(chart).toBeInTheDocument();
        expect(JSON.parse(chart.getAttribute('data-labels'))).toEqual(['Vietnam', 'Singapore']);
        expect(JSON.parse(chart.getAttribute('data-values'))).toEqual([60, 40]);
    });

    it('should handle null values in data', () => {
        const data = [
            { label: 'Vietnam', value: null },
            { label: 'Singapore', value: 40 }
        ];

        const { getByTestId } = render(<HeadcountByCountryChart data={data} />);

        const chart = getByTestId('doughnut-chart');
        expect(JSON.parse(chart.getAttribute('data-values'))).toEqual([0, 40]);
    });

    it('should render placeholder when data is empty', () => {
        const { getByText } = render(<HeadcountByCountryChart data={[]} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });

    it('should render placeholder when data is null', () => {
        const { getByText } = render(<HeadcountByCountryChart data={null} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });
});

