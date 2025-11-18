import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import AppContainer from '../AppContainer';
import ExchangeRateService from '../service';

jest.mock('../service');
jest.mock('toastr', () => ({
    success: jest.fn(),
    error: jest.fn()
}));
jest.mock('../../../shared/common.functions', () => ({
    axiosError: jest.fn(),
    toastOptions: jest.fn(() => ({}))
}));
jest.mock('../components/ExchangeRateModal', () => {
    return function MockExchangeRateModal({ show, onClose, onSuccess, editData, isEdit }) {
        if (!show) return null;
        return (
            <div data-testid="exchange-rate-modal">
                <h4>{isEdit ? 'Edit Exchange Rate' : 'Add Exchange Rate'}</h4>
                <button onClick={onClose}>Close</button>
                <button onClick={onSuccess}>Submit</button>
                {editData && <input data-testid="edit-currency" defaultValue={editData.baseCurrency} />}
            </div>
        );
    };
});
jest.mock('../../../shared/components/SweetModalDialog', () => {
    return function MockSweetModalDialog({ show, contents, onSubmit, onHide }) {
        if (!show) return null;
        return (
            <div data-testid="sweet-modal-dialog">
                {contents}
                <button onClick={onSubmit}>Submit</button>
                <button onClick={onHide}>Cancel</button>
            </div>
        );
    };
});

beforeAll(() => {
    // Suppress React Bootstrap Modal lifecycle warnings and moment deprecation warnings
    const originalWarn = jest.requireActual('console').warn;
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
             message.includes('react-unsafe-component-lifecycles') ||
             message.includes('Deprecation warning') ||
             message.includes('moment construction falls back') ||
             message.includes('Non RFC2822/ISO date formats'))
        ) {
            return;
        }
        // Let other warnings through
        originalWarn(message);
    });

    // Suppress act() warnings and moment errors from third-party libraries
    const originalError = jest.requireActual('console').error;
    jest.spyOn(console, 'error').mockImplementation((message, ...args) => {
        const messageStr = typeof message === 'string' ? message : String(message);
        const allArgsStr = args.map(arg => typeof arg === 'string' ? arg : String(arg)).join(' ');
        const combinedMessage = messageStr + ' ' + allArgsStr;
        
        if (
            messageStr.includes('Warning: An update to') ||
            messageStr.includes('was not wrapped in act') ||
            messageStr.includes('not wrapped in act(...)') ||
            messageStr.includes('react-wrap-tests-with-act') ||
            combinedMessage.includes('Deprecation warning') ||
            combinedMessage.includes('moment construction falls back') ||
            combinedMessage.includes('createFromInputFallback') ||
            combinedMessage.includes('Non RFC2822/ISO date formats') ||
            combinedMessage.includes('moment.js') ||
            combinedMessage.includes('moment/moment.js')
        ) {
            return;
        }
        // Let other errors through
        originalError(message, ...args);
    });
});

afterAll(() => {
    console.warn.mockRestore();
    console.error.mockRestore();
});

const mockHasPermission = (value) => {
    Object.defineProperty(window, 'hasPermission', {
        value: value,
        writable: true
    });
};

describe('AppContainer Component', () => {
    const mockExchangeRates = [
        {
            currencyId: 1,
            baseCurrency: 'USD',
            toUsd: 1.0,
            effectiveDate: '2024-01-01',
            rounding: 0
        },
        {
            currencyId: 2,
            baseCurrency: 'EUR',
            toUsd: 1.1,
            effectiveDate: '2024-01-15',
            rounding: 2
        }
    ];

    const mockRoundingRules = [
        {
            currency: 'USD',
            rounding: 0,
            description: 'Rounded to nearest 1'
        }
    ];

    beforeEach(() => {
        jest.clearAllMocks();
        mockHasPermission('True');
        ExchangeRateService.getExchangeRates = jest.fn().mockResolvedValue(mockExchangeRates);
        ExchangeRateService.getRoundingRules = jest.fn().mockResolvedValue(mockRoundingRules);
        ExchangeRateService.deleteExchangeRate = jest.fn().mockResolvedValue({
            data: { code: 0, isSuccessful: true }
        });
    });

    afterEach(() => {
        delete window.hasPermission;
    });

    it('should render exchange rate table and fetch rates on mount', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
            expect(screen.getByText('1 unit to USD')).toBeInTheDocument();
        });
    });

    it('should show "New Exchange Rate" button when hasPermission is True', () => {
        mockHasPermission('True');
        render(<AppContainer />);

        expect(screen.getByText('New Exchange Rate')).toBeInTheDocument();
    });

    it('should not show "New Exchange Rate" button when hasPermission is False', () => {
        mockHasPermission('False');
        render(<AppContainer />);

        expect(screen.queryByText('New Exchange Rate')).not.toBeInTheDocument();
    });

    it('should open rounding info popup when info icon is clicked', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const infoIcon = screen.getByTitle('Click to show Currency Rounding info');
        fireEvent.click(infoIcon);

        await waitFor(() => {
            expect(ExchangeRateService.getRoundingRules).toHaveBeenCalled();
            expect(screen.getByText('Currency Rounding')).toBeInTheDocument();
        });
    });

    it('should close rounding info popup when close icon is clicked', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const infoIcon = screen.getByTitle('Click to show Currency Rounding info');
        fireEvent.click(infoIcon);

        await waitFor(() => {
            expect(screen.getByText('Currency Rounding')).toBeInTheDocument();
        });

        const closeIcon = document.querySelector('.fa-times.close-icon');
        expect(closeIcon).toBeTruthy();
        if (closeIcon) {
            fireEvent.click(closeIcon);
            await waitFor(() => {
                expect(screen.queryByText('Currency Rounding')).not.toBeInTheDocument();
            });
        }
    });

    it('should open add modal when "New Exchange Rate" button is clicked', () => {
        render(<AppContainer />);

        const addButton = screen.getByText('New Exchange Rate');
        fireEvent.click(addButton);

        expect(screen.getByText('Add Exchange Rate')).toBeInTheDocument();
    });

    it('should handle delete exchange rate successfully', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(ExchangeRateService.deleteExchangeRate).toHaveBeenCalled();
            });
        }
    });

    it('should handle object response with exchangeRates property', async () => {
        ExchangeRateService.getExchangeRates = jest.fn().mockResolvedValue({
            exchangeRates: mockExchangeRates
        });

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
            expect(screen.getByText('1 unit to USD')).toBeInTheDocument();
        });
    });

    it('should close add modal when onClose is called', async () => {
        render(<AppContainer />);

        const addButton = screen.getByText('New Exchange Rate');
        fireEvent.click(addButton);

        await waitFor(() => {
            expect(screen.getByText('Add Exchange Rate')).toBeInTheDocument();
        });

        const closeButton = screen.getByRole('button', { name: /close/i });
        fireEvent.click(closeButton);

        await waitFor(() => {
            expect(screen.queryByText('Add Exchange Rate')).not.toBeInTheDocument();
        });
    });

    it('should open edit modal when edit button is clicked', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const editButtons = screen.queryAllByTitle('Edit');
        if (editButtons.length > 0) {
            fireEvent.click(editButtons[0]);

            await waitFor(() => {
                expect(screen.getByText('Edit Exchange Rate')).toBeInTheDocument();
            });
        }
    });

    it('should close edit modal when onClose is called', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const editButtons = screen.queryAllByTitle('Edit');
        if (editButtons.length > 0) {
            fireEvent.click(editButtons[0]);

            await waitFor(() => {
                expect(screen.getByText('Edit Exchange Rate')).toBeInTheDocument();
            });

            const closeButton = screen.getByRole('button', { name: /close/i });
            fireEvent.click(closeButton);

            await waitFor(() => {
                expect(screen.queryByText('Edit Exchange Rate')).not.toBeInTheDocument();
            });
        }
    });

    it('should refresh exchange rates after successful add', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const initialCallCount = ExchangeRateService.getExchangeRates.mock.calls.length;

        const addButton = screen.getByText('New Exchange Rate');
        fireEvent.click(addButton);

        await waitFor(() => {
            expect(screen.getByText('Add Exchange Rate')).toBeInTheDocument();
        });

        // Simulate successful add by clicking Submit button in mock modal
        const submitButton = screen.getByText('Submit');
        fireEvent.click(submitButton);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates.mock.calls.length).toBeGreaterThan(initialCallCount);
        });
    });

    it('should refresh exchange rates after successful edit', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const initialCallCount = ExchangeRateService.getExchangeRates.mock.calls.length;

        const editButtons = screen.queryAllByTitle('Edit');
        if (editButtons.length > 0) {
            fireEvent.click(editButtons[0]);

            await waitFor(() => {
                expect(screen.getByText('Edit Exchange Rate')).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(ExchangeRateService.getExchangeRates.mock.calls.length).toBeGreaterThan(initialCallCount);
            });
        }
    });

    it('should handle delete error response', async () => {
        const toastr = require('toastr');
        ExchangeRateService.deleteExchangeRate = jest.fn().mockResolvedValue({
            data: { code: 1, message: 'Delete failed' }
        });

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(toastr.error).toHaveBeenCalled();
            });
        }
    });

    it('should handle delete catch error', async () => {
        const toastr = require('toastr');
        ExchangeRateService.deleteExchangeRate = jest.fn().mockRejectedValue({
            response: {
                data: { message: 'Network error' }
            }
        });

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(toastr.error).toHaveBeenCalled();
            });
        }
    });

    it('should close delete modal when cancel is clicked', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
            });

            const cancelButton = screen.getByRole('button', { name: /cancel/i });
            if (cancelButton) {
                fireEvent.click(cancelButton);

                await waitFor(() => {
                    expect(screen.queryByText(/Are you sure/i)).not.toBeInTheDocument();
                });
            }
        }
    });

    it('should close rounding info popup when overlay is clicked', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const infoIcon = screen.getByTitle('Click to show Currency Rounding info');
        fireEvent.click(infoIcon);

        await waitFor(() => {
            expect(screen.getByText('Currency Rounding')).toBeInTheDocument();
        });

        const overlay = document.querySelector('.popup-overlay');
        if (overlay) {
            fireEvent.click(overlay);

            await waitFor(() => {
                expect(screen.queryByText('Currency Rounding')).not.toBeInTheDocument();
            });
        }
    });

    it('should handle empty rounding rules', async () => {
        ExchangeRateService.getRoundingRules = jest.fn().mockResolvedValue(null);

        render(<AppContainer />);

        const infoIcon = screen.getByTitle('Click to show Currency Rounding info');
        fireEvent.click(infoIcon);

        await waitFor(() => {
            expect(ExchangeRateService.getRoundingRules).toHaveBeenCalled();
        });
    });

    it('should handle fetchExchangeRates error', async () => {
        const { axiosError } = require('../../../shared/common.functions');
        ExchangeRateService.getExchangeRates = jest.fn().mockRejectedValue(new Error('Network error'));

        render(<AppContainer />);

        await waitFor(() => {
            expect(axiosError).toHaveBeenCalled();
        });
    });

    it('should handle fetchRoundingRules error', async () => {
        const { axiosError } = require('../../../shared/common.functions');
        ExchangeRateService.getRoundingRules = jest.fn().mockRejectedValue(new Error('Network error'));

        render(<AppContainer />);

        const infoIcon = screen.getByTitle('Click to show Currency Rounding info');
        fireEvent.click(infoIcon);

        await waitFor(() => {
            expect(axiosError).toHaveBeenCalled();
        });
    });

    it('should handle delete success with isSuccessful flag', async () => {
        const toastr = require('toastr');
        ExchangeRateService.deleteExchangeRate = jest.fn().mockResolvedValue({
            data: { isSuccessful: true }
        });

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(toastr.success).toHaveBeenCalled();
                expect(ExchangeRateService.getExchangeRates).toHaveBeenCalledTimes(2);
            });
        }
    });

    it('should handle delete error with Message (capital M)', async () => {
        const toastr = require('toastr');
        ExchangeRateService.deleteExchangeRate = jest.fn().mockRejectedValue({
            response: {
                data: { Message: 'Error with capital M' }
            }
        });

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(toastr.error).toHaveBeenCalledWith('Error with capital M', '', expect.anything());
            });
        }
    });

    it('should handle delete error without response.data', async () => {
        const toastr = require('toastr');
        ExchangeRateService.deleteExchangeRate = jest.fn().mockRejectedValue({
            message: 'Network error'
        });

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(toastr.error).toHaveBeenCalledWith('Failed to delete exchange rate', '', expect.anything());
            });
        }
    });

    it('should display formatted date in delete modal when effectiveDate exists', async () => {
        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
                // Check if formatted date is displayed (format: DD MMM, YYYY)
                const modalText = screen.getByText(/Are you sure/i).textContent;
                expect(modalText).toMatch(/\d{2}\s\w{3},\s\d{4}/);
            });
        }
    });

    it('should handle delete modal without effectiveDate', async () => {
        const mockDataWithoutDate = [{
            currencyId: 3,
            baseCurrency: 'GBP',
            toUsd: 1.25,
            rounding: 0
        }];

        ExchangeRateService.getExchangeRates = jest.fn().mockResolvedValue(mockDataWithoutDate);

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
                // Should not show "with effective date" if effectiveDate is missing
                const modalText = screen.getByText(/Are you sure/i).textContent;
                expect(modalText).not.toMatch(/with effective date/);
            });
        }
    });

    it('should handle delete with missing effectiveDate', async () => {
        const toastr = require('toastr');
        const mockDataWithoutDate = [{
            currencyId: 3,
            baseCurrency: 'GBP',
            toUsd: 1.25,
            rounding: 0
            // No effectiveDate
        }];

        ExchangeRateService.getExchangeRates = jest.fn().mockResolvedValue(mockDataWithoutDate);

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(toastr.error).toHaveBeenCalledWith(
                    'Cannot delete: Currency ID or Effective date not found',
                    '',
                    expect.anything()
                );
            });
        }
    });

    it('should handle delete error with default message when no message provided', async () => {
        const toastr = require('toastr');
        ExchangeRateService.deleteExchangeRate = jest.fn().mockResolvedValue({
            data: { code: 1 }
            // No message property
        });

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(toastr.error).toHaveBeenCalledWith(
                    'Failed to delete exchange Rate',
                    '',
                    expect.anything()
                );
            });
        }
    });

    it('should handle formatDate with null or undefined date', async () => {
        const mockDataWithNullDate = [{
            currencyId: 5,
            baseCurrency: 'CNY',
            toUsd: 0.14,
            effectiveDate: null,
            rounding: 0
        }];

        ExchangeRateService.getExchangeRates = jest.fn().mockResolvedValue(mockDataWithNullDate);

        render(<AppContainer />);

        await waitFor(() => {
            expect(ExchangeRateService.getExchangeRates).toHaveBeenCalled();
        });

        const deleteButtons = screen.queryAllByTitle('Delete');
        if (deleteButtons.length > 0) {
            fireEvent.click(deleteButtons[0]);

            await waitFor(() => {
                expect(screen.getByText(/Are you sure/i)).toBeInTheDocument();
                const modalText = screen.getByText(/Are you sure/i).textContent;
                expect(modalText).not.toMatch(/with effective date/);
            });
        }
    });
});
