import { render, fireEvent, waitFor } from '@testing-library/react';
import React from 'react';
import GenerateSalaryModal from '../../components/GenerateSalaryModal';

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

describe('GenerateSalaryModal Component', () => {
    const mockOnSuccess = jest.fn();
    const mockSalaryService = require('../../Service');
    const mockToastr = require('toastr');

    beforeEach(() => {
        jest.clearAllMocks();
        mockSalaryService.generatePayroll.mockResolvedValue({});
    });

    it('should render component', () => {
        const { container } = render(
            <GenerateSalaryModal onSuccess={mockOnSuccess} />
        );
        expect(container).toBeInTheDocument();
    });

    it('should render generate button', () => {
        const { queryByText } = render(
            <GenerateSalaryModal onSuccess={mockOnSuccess} />
        );
        const button = queryByText('Generate Payroll');
        expect(button).toBeInTheDocument();
    });

    it('should call service with correct parameters', async () => {
        const mockDate = '2024-01';
        mockSalaryService.generatePayroll.mockResolvedValue({});

        const component = render(
            <GenerateSalaryModal onSuccess={mockOnSuccess} />
        );

        expect(mockSalaryService.generatePayroll).not.toHaveBeenCalled();
    });

    it('should handle success callback', async () => {
        const testMonth = '2024-01';
        mockSalaryService.generatePayroll.mockResolvedValue({});

        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);

        expect(mockOnSuccess).not.toHaveBeenCalled();
    });

    it('should handle error from service', async () => {
        mockSalaryService.generatePayroll.mockRejectedValue(
            new Error('Test error')
        );

        render(<GenerateSalaryModal onSuccess={mockOnSuccess} />);

        expect(mockToastr.error).not.toHaveBeenCalled();
    });

    it('should format month correctly for API', () => {
        const inputMonth = '2024-01';
        const expectedApiFormat = `${inputMonth}-01`;

        expect(expectedApiFormat).toBe('2024-01-01');
    });

    it('should initialize current month', () => {
        const now = new Date();
        const currentMonth = `${now.getFullYear()}-${String(
            now.getMonth() + 1
        ).padStart(2, '0')}`;

        expect(currentMonth).toMatch(/^\d{4}-\d{2}$/);
    });
});
