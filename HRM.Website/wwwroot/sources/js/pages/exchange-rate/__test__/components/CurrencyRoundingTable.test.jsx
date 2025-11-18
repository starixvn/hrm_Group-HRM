import React from 'react';
import { render, screen } from '@testing-library/react';
import CurrencyRoundingTable from '../../components/CurrencyRoundingTable';

describe('CurrencyRoundingTable Component', () => {
    const mockData = [
        {
            currency: 'USD',
            rounding: 0,
            description: 'Rounded to nearest 1'
        },
        {
            currency: 'EUR',
            rounding: 2,
            description: 'Rounded to 2 decimal places'
        }
    ];

    it('should render table with rounding rules data', () => {
        render(<CurrencyRoundingTable data={mockData} />);

        expect(screen.getByText('Currency')).toBeInTheDocument();
        expect(screen.getByText('Rounding')).toBeInTheDocument();
        expect(screen.getByText('USD')).toBeInTheDocument();
        expect(screen.getByText('EUR')).toBeInTheDocument();
    });

    it('should display "No data available" when data is empty', () => {
        render(<CurrencyRoundingTable data={[]} />);

        expect(screen.getByText('No data available')).toBeInTheDocument();
    });

    it('should display rounding values correctly', () => {
        render(<CurrencyRoundingTable data={mockData} />);

        expect(screen.getByText('0')).toBeInTheDocument();
        expect(screen.getByText('2')).toBeInTheDocument();
    });

    it('should handle null or undefined data', () => {
        render(<CurrencyRoundingTable data={null} />);

        expect(screen.getByText('No data available')).toBeInTheDocument();
    });
});


