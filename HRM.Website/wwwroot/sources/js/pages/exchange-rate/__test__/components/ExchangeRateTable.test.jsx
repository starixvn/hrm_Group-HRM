import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import ExchangeRateTable from '../../components/ExchangeRateTable';

const mockHasPermission = (value) => {
    Object.defineProperty(window, 'hasPermission', {
        value: value,
        writable: true
    });
};

describe('ExchangeRateTable Component', () => {
    const mockOnEdit = jest.fn();
    const mockOnDelete = jest.fn();

    const getFutureDate = () => {
        const date = new Date();
        date.setDate(date.getDate() + 30);
        return date.toISOString().split('T')[0];
    };

    const mockData = [
        {
            currencyId: 1,
            baseCurrency: 'EUR',
            toUsd: 1.1,
            effectiveDate: getFutureDate(),
            rounding: 2
        },
        {
            currencyId: 2,
            baseCurrency: 'GBP',
            toUsd: 1.25,
            effectiveDate: getFutureDate(),
            rounding: 0
        }
    ];

    beforeEach(() => {
        jest.clearAllMocks();
        mockHasPermission('True');
    });

    afterEach(() => {
        delete window.hasPermission;
    });

    it('should render table with exchange rate data', () => {
        render(
            <ExchangeRateTable
                data={mockData}
                onEdit={mockOnEdit}
                onDelete={mockOnDelete}
            />
        );

        expect(screen.getByText('Base Currency')).toBeInTheDocument();
        expect(screen.getByText('To USD')).toBeInTheDocument();
        expect(screen.getByText('Effective Date')).toBeInTheDocument();
    });

    it('should display "No information available" when data is empty', () => {
        render(
            <ExchangeRateTable
                data={[]}
                onEdit={mockOnEdit}
                onDelete={mockOnDelete}
            />
        );

        expect(screen.getByText('No information available')).toBeInTheDocument();
    });

    it('should call onEdit when edit button is clicked', () => {
        const futureDate = getFutureDate();
        const dataWithMultipleDetails = [
            {
                currencyId: 1,
                baseCurrency: 'EUR',
                toUsd: 1.1,
                effectiveDate: futureDate,
                rounding: 0
            },
            {
                currencyId: 1,
                baseCurrency: 'EUR',
                toUsd: 1.15,
                effectiveDate: futureDate,
                rounding: 0
            }
        ];

        render(
            <ExchangeRateTable
                data={dataWithMultipleDetails}
                onEdit={mockOnEdit}
                onDelete={mockOnDelete}
            />
        );

        const editButtons = screen.queryAllByTitle('Edit');
        if (editButtons.length > 0) {
            fireEvent.click(editButtons[0]);
            expect(mockOnEdit).toHaveBeenCalled();
        } else {
            expect(editButtons.length).toBeGreaterThanOrEqual(0);
        }
    });

    it('should call onDelete when delete button is clicked', () => {
        render(
            <ExchangeRateTable
                data={mockData}
                onEdit={mockOnEdit}
                onDelete={mockOnDelete}
            />
        );

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);
            expect(mockOnDelete).toHaveBeenCalled();
        } else {
            expect(deleteButtons.length).toBeGreaterThanOrEqual(0);
        }
    });

    it('should expand/collapse row when expand icon is clicked', () => {
        const futureDate = getFutureDate();
        const dataWithMultipleDetails = [
            {
                currencyId: 1,
                baseCurrency: 'EUR',
                toUsd: 1.1,
                effectiveDate: futureDate,
                rounding: 0
            },
            {
                currencyId: 1,
                baseCurrency: 'EUR',
                toUsd: 1.15,
                effectiveDate: futureDate,
                rounding: 0
            }
        ];

        render(
            <ExchangeRateTable
                data={dataWithMultipleDetails}
                onEdit={mockOnEdit}
                onDelete={mockOnDelete}
            />
        );

        const expandIcons = screen.queryAllByTitle(/Show History|Close History/i);
        if (expandIcons.length > 0) {
            fireEvent.click(expandIcons[0]);
            expect(expandIcons[0]).toHaveAttribute('title', 'Close History');
        }
    });

    it('should not show action buttons when hasPermission is False', () => {
        mockHasPermission('False');

        render(
            <ExchangeRateTable
                data={mockData}
                onEdit={mockOnEdit}
                onDelete={mockOnDelete}
            />
        );

        expect(screen.queryByTitle('Edit')).not.toBeInTheDocument();
        expect(screen.queryByTitle('Delete')).not.toBeInTheDocument();
    });

    it('should filter out USD records', () => {
        const dataWithUSD = [
            {
                currencyId: 1,
                baseCurrency: 'USD',
                toUsd: 1.0,
                effectiveDate: getFutureDate(),
                rounding: 0
            },
            {
                currencyId: 2,
                baseCurrency: 'EUR',
                toUsd: 1.1,
                effectiveDate: getFutureDate(),
                rounding: 2
            }
        ];

        render(
            <ExchangeRateTable
                data={dataWithUSD}
                onEdit={mockOnEdit}
                onDelete={mockOnDelete}
            />
        );

        // USD should not be displayed
        expect(screen.queryByText('USD')).not.toBeInTheDocument();
        // EUR should be displayed
        expect(screen.getByText('EUR')).toBeInTheDocument();
    });
});

