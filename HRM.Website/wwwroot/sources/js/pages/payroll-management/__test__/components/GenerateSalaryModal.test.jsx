import { render, fireEvent, waitFor, screen } from '@testing-library/react';
import React from 'react';

jest.mock('toastr', () => ({
    success: jest.fn(),
    error: jest.fn(),
}));

jest.mock('../../../../shared/common.functions', () => ({
    toastOptions: jest.fn(() => ({})),
}));

jest.mock('../../Service', () => ({
    generatePayroll: jest.fn(),
}));

import GenerateSalaryModal from '../../components/GenerateSalaryModal';
import toastr from 'toastr';
import salaryService from '../../Service';

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

    it('generates payroll successfully', async () => {
        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);
        fireEvent.click(screen.getByText('Generate Payroll'));

        const monthInput = screen.getByDisplayValue(/\d{4}-\d{2}/);
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

        const monthInput = screen.getByDisplayValue(/\d{4}-\d{2}/);
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

        const monthInput = screen.getByDisplayValue(/\d{4}-\d{2}/);
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
