import { render } from '@testing-library/react';
import React from 'react';
import BUCategoryBreakdown from '../../components/BUCategoryBreakdown';

jest.mock('react-chartjs-2', () => ({
    Pie: ({ data, options }) => {
        const React = require('react');
        return React.createElement('div', { 
            'data-testid': 'pie-chart',
            'data-labels': JSON.stringify(data.labels),
            'data-values': JSON.stringify(data.datasets[0].data)
        });
    }
}));

jest.mock('chartjs-plugin-datalabels', () => ({}));

describe('BUCategoryBreakdown Component', () => {
    it('should render both charts and legend with valid data', () => {
        const data = [
            { category: 'Technology', employeeCount: 40, buCount: 3 },
            { category: 'Administration', employeeCount: 20, buCount: 2 }
        ];

        const { getByText, getAllByTestId } = render(<BUCategoryBreakdown data={data} />);

        expect(getByText('Employees')).toBeInTheDocument();
        expect(getByText('Business Units')).toBeInTheDocument();
        expect(getAllByTestId('pie-chart')).toHaveLength(2);
        expect(getByText('Technology')).toBeInTheDocument();
        expect(getByText('Administration')).toBeInTheDocument();
    });

    it('should handle missing category field using label1', () => {
        const data = [
            { label1: 'Technology', employeeCount: 40, buCount: 3 }
        ];

        const { getByText } = render(<BUCategoryBreakdown data={data} />);

        expect(getByText('Technology')).toBeInTheDocument();
    });

    it('should handle null or zero values', () => {
        const data = [
            { category: 'Technology', employeeCount: null, buCount: 0 },
            { category: 'Admin', employeeCount: 20, buCount: null }
        ];

        const { getAllByTestId } = render(<BUCategoryBreakdown data={data} />);

        const charts = getAllByTestId('pie-chart');
        expect(charts).toHaveLength(2);
    });

    it('should render placeholder when data is empty', () => {
        const { getByText } = render(<BUCategoryBreakdown data={[]} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });

    it('should render placeholder when data is null', () => {
        const { getByText } = render(<BUCategoryBreakdown data={null} />);

        expect(getByText('No data available')).toBeInTheDocument();
    });
});

