import { render, fireEvent, waitFor, screen } from '@testing-library/react';
import React from 'react';
import GenerateSalaryModal from '../../components/GenerateSalaryModal';
import toastr from 'toastr';
import salaryService from '../../service';

jest.spyOn(console, 'warn').mockImplementation(() => {});
jest.mock('toastr', () => ({
    success: jest.fn(),
    error: jest.fn(),
}));

jest.mock('../../../../shared/common.functions', () => ({
    toastOptions: jest.fn(() => ({})),
}));

jest.mock('../../service', () => ({
    generatePayroll: jest.fn(),
}));

jest.mock('../../../../shared/components/MonthPicker', () => {
    return function MockMonthPicker({ value, onChange, placeholder }) {
        return (
            <input
                data-testid="month-picker"
                value={value}
                onChange={(e) => onChange(e.target.value)}
                placeholder={placeholder}
            />
        );
    };
});

describe('GenerateSalaryModal', () => {
    const mockOnSuccess = jest.fn();
    const originalConsoleError = console.error;

    beforeAll(() => {
        console.error = jest.fn();
    });

    afterAll(() => {
        console.error = originalConsoleError;
    });

    beforeEach(() => {
        jest.clearAllMocks();
        salaryService.generatePayroll.mockResolvedValue({});
    });

    it('renders generate button', () => {
        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);
        expect(screen.getByText('Generate Payroll')).toBeInTheDocument();
    });

    it('opens modal when button clicked', async () => {
        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);

        fireEvent.click(screen.getByText('Generate Payroll'));

        await waitFor(() => {
            expect(screen.getAllByText('Generate Payroll')).toHaveLength(2);
        });
    });

    it('sets current month as default when modal opens', async () => {
        const currentDate = new Date();
        const expectedMonth = `${currentDate.getFullYear()}-${String(currentDate.getMonth() + 1).padStart(2, '0')}`;
        
        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);
        fireEvent.click(screen.getByText('Generate Payroll'));

        const monthInput = screen.getByTestId('month-picker');
        expect(monthInput.value).toBe(expectedMonth);
    });

    it('generates payroll successfully', async () => {
        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);
        fireEvent.click(screen.getByText('Generate Payroll'));

        const monthInput = screen.getByTestId('month-picker');
        fireEvent.change(monthInput, { target: { value: '2024-03' } });

        fireEvent.click(screen.getByText('Generate'));
        fireEvent.click(screen.getByText('OK'));

        await waitFor(() => {
            expect(salaryService.generatePayroll).toHaveBeenCalledWith(
                '2024-03-01'
            );
            expect(toastr.success).toHaveBeenCalled();
            expect(mockOnSuccess).toHaveBeenCalledWith('2024-03');
        });
    });

    it('handles generate payroll error', async () => {
        salaryService.generatePayroll.mockRejectedValue(new Error('API Error'));

        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);
        fireEvent.click(screen.getByText('Generate Payroll'));

        const monthInput = screen.getByTestId('month-picker');
        fireEvent.change(monthInput, { target: { value: '2024-03' } });

        fireEvent.click(screen.getByText('Generate'));
        fireEvent.click(screen.getByText('OK'));

        await waitFor(() => {
            expect(salaryService.generatePayroll).toHaveBeenCalledWith(
                '2024-03-01'
            );
            expect(toastr.error).toHaveBeenCalled();
            expect(mockOnSuccess).not.toHaveBeenCalled();
        });
    });

    it('validates empty month input', () => {
        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);
        fireEvent.click(screen.getByText('Generate Payroll'));

        const monthInput = screen.getByTestId('month-picker');
        fireEvent.change(monthInput, { target: { value: '' } });
        fireEvent.click(screen.getByText('Generate'));

        expect(screen.queryByText('Confirm Generate')).not.toBeInTheDocument();
        expect(salaryService.generatePayroll).not.toHaveBeenCalled();
    });

    it('cancels confirmation dialog', () => {
        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);
        fireEvent.click(screen.getByText('Generate Payroll'));
        fireEvent.click(screen.getByText('Generate'));

        expect(screen.getByText('Confirm Generate')).toBeInTheDocument();

        const cancelButtons = screen.getAllByText('Cancel');
        fireEvent.click(cancelButtons[1]);

        expect(salaryService.generatePayroll).not.toHaveBeenCalled();
        expect(mockOnSuccess).not.toHaveBeenCalled();
    });
});
