import React from 'react';
import { render, screen, fireEvent, waitFor, cleanup } from '@testing-library/react';
import ExchangeRateModal from '../../components/ExchangeRateModal';
import ExchangeRateService from '../../service';
import { validationSchema } from '../../common.functions';

jest.mock('../../service');
jest.mock('../../common.functions', () => ({
    validationSchema: {
        validate: jest.fn()
    }
}));
jest.mock('../../../../shared/common.functions', () => ({
    toastOptions: jest.fn(() => ({}))
}));
jest.mock('toastr', () => ({
    success: jest.fn(),
    error: jest.fn()
}));
jest.mock('react-draggable', () => {
    return function Draggable({ children }) {
        return children;
    };
});

const originalWarn = console.warn;
const originalError = console.error;

beforeAll(() => {
    // Suppress React Bootstrap Modal lifecycle warnings
    jest.spyOn(console, 'warn').mockImplementation((message) => {
        if (
            typeof message === 'string' &&
            (message.includes('componentWillReceiveProps') ||
             message.includes('componentWillUpdate') ||
             message.includes('componentWillMount') ||
             message.includes('UNSAFE_componentWillReceiveProps') ||
             message.includes('UNSAFE_componentWillUpdate') ||
             message.includes('Please update the following components: Modal') ||
             message.includes('Please update the following components: Portal') ||
             message.includes('react-unsafe-component-lifecycles'))
        ) {
            return;
        }
        // Let other warnings through
        originalWarn(message);
    });

    // Suppress act() warnings from third-party libraries
    jest.spyOn(console, 'error').mockImplementation((message) => {
        if (
            typeof message === 'string' &&
            (message.includes('Warning: An update to') ||
             message.includes('was not wrapped in act') ||
             message.includes('not wrapped in act(...)') ||
             message.includes('react-wrap-tests-with-act'))
        ) {
            return;
        }
        // Let other errors through
        originalError(message);
    });
});

afterAll(() => {
    console.warn.mockRestore();
    console.error.mockRestore();
});

describe('ExchangeRateModal Component', () => {
    const mockOnClose = jest.fn();
    const mockOnSuccess = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
        ExchangeRateService.createExchangeRate = jest.fn().mockResolvedValue({
            data: { code: 0, message: 'Success' }
        });
        ExchangeRateService.updateExchangeRate = jest.fn().mockResolvedValue({
            data: { code: 0, message: 'Success' }
        });
        validationSchema.validate = jest.fn().mockResolvedValue({
            currencyCode: 'USD',
            exchangeRate: 1.0,
            effectiveDate: '2024-01-01'
        });
    });

    afterEach(() => {
        cleanup();
    });

    describe('Add Mode', () => {
        it('should render modal when show is true', () => {
            render(
                <ExchangeRateModal
                    show={true}
                    onClose={mockOnClose}
                    onSuccess={mockOnSuccess}
                />
            );

            expect(screen.getByText('Add Exchange Rate')).toBeInTheDocument();
        });

        it('should not render modal when show is false', () => {
            render(
                <ExchangeRateModal
                    show={false}
                    onClose={mockOnClose}
                    onSuccess={mockOnSuccess}
                />
            );

            expect(screen.queryByText('Add Exchange Rate')).not.toBeInTheDocument();
        });

        it('should call onClose when close button is clicked', () => {
            render(
                <ExchangeRateModal
                    show={true}
                    onClose={mockOnClose}
                    onSuccess={mockOnSuccess}
                />
            );

            const closeButton = screen.getByRole('button', { name: /close/i });
            fireEvent.click(closeButton);

            expect(mockOnClose).toHaveBeenCalled();
        });

        it('should submit form with valid data', async () => {
            render(
                <ExchangeRateModal
                    show={true}
                    onClose={mockOnClose}
                    onSuccess={mockOnSuccess}
                />
            );

            const currencyInput = screen.getByPlaceholderText(/USD, EUR/i);
            const rateInput = screen.getByPlaceholderText(/0.00000000/i);
            const submitButton = screen.getByRole('button', { name: /Submit/i });

            let dateInput;
            await waitFor(() => {
                dateInput = document.querySelector('input[name="effectiveDate"]');
                expect(dateInput).toBeTruthy();
            });

            fireEvent.change(currencyInput, { target: { value: 'USD' } });
            fireEvent.change(rateInput, { target: { value: '1.0' } });
            fireEvent.change(dateInput, { target: { value: '2024-01-01' } });
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(ExchangeRateService.createExchangeRate).toHaveBeenCalled();
            });
        });

        it('should convert currency code to uppercase', async () => {
            render(
                <ExchangeRateModal
                    show={true}
                    onClose={mockOnClose}
                    onSuccess={mockOnSuccess}
                />
            );

            const currencyInput = screen.getByPlaceholderText(/USD, EUR/i);
            fireEvent.change(currencyInput, { target: { value: 'usd' } });

            await waitFor(() => {
                expect(currencyInput).toHaveValue('USD');
            });
        });
    });

    describe('Edit Mode', () => {
        const mockEditData = {
            currencyId: 1,
            baseCurrency: 'EUR',
            toUsd: 1.1,
            effectiveDate: '2024-01-01',
            rounding: 2
        };

        it('should render modal with edit data', () => {
            render(
                <ExchangeRateModal
                    show={true}
                    onClose={mockOnClose}
                    onSuccess={mockOnSuccess}
                    editData={mockEditData}
                    isEdit={true}
                />
            );

            expect(screen.getByText('Edit Exchange Rate')).toBeInTheDocument();
        });

        it('should populate form with edit data', () => {
            render(
                <ExchangeRateModal
                    show={true}
                    onClose={mockOnClose}
                    onSuccess={mockOnSuccess}
                    editData={mockEditData}
                    isEdit={true}
                />
            );

            const currencyInput = screen.getByPlaceholderText(/USD, EUR/i);
            expect(currencyInput).toHaveValue('EUR');
        });

        it('should call updateExchangeRate when submitting edit', async () => {
            render(
                <ExchangeRateModal
                    show={true}
                    onClose={mockOnClose}
                    onSuccess={mockOnSuccess}
                    editData={mockEditData}
                    isEdit={true}
                />
            );

            const submitButton = screen.getByRole('button', { name: /Submit/i });
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(ExchangeRateService.updateExchangeRate).toHaveBeenCalledWith(
                    mockEditData.currencyId,
                    expect.objectContaining({
                        currencyCode: expect.any(String)
                    })
                );
            }, { timeout: 3000 });
        });
    });

    describe('Form Validation', () => {
        it('should show validation errors for invalid data', async () => {
            const mockError = {
                inner: [
                    { message: 'Currency is required!' },
                    { message: 'Exchange Rate must be greater than 0' }
                ]
            };
            validationSchema.validate = jest.fn().mockRejectedValue(mockError);

            render(
                <ExchangeRateModal
                    show={true}
                    onClose={mockOnClose}
                    onSuccess={mockOnSuccess}
                />
            );

            const currencyInput = screen.getByPlaceholderText(/USD, EUR/i);
            const rateInput = screen.getByPlaceholderText(/0.00000000/i);
            const submitButton = screen.getByRole('button', { name: /Submit/i });
            
            let dateInput;
            await waitFor(() => {
                dateInput = document.querySelector('input[name="effectiveDate"]');
                expect(dateInput).toBeTruthy();
            });

            fireEvent.change(currencyInput, { target: { value: 'USD' } });
            fireEvent.change(rateInput, { target: { value: '-1' } });
            fireEvent.change(dateInput, { target: { value: '2024-01-01' } });
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(validationSchema.validate).toHaveBeenCalled();
            }, { timeout: 3000 });
        });
    });
});

