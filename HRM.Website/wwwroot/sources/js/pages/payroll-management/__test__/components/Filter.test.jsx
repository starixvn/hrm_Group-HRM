import { render, fireEvent } from '@testing-library/react';
import React from 'react';
import Filter from '../../components/Filter';

jest.mock('../../components/GenerateSalaryModal', () => {
    return function GenerateSalaryModal({ onSuccess }) {
        return (
            <div>
                <button onClick={() => onSuccess('2024-05')}>
                    Generate Payroll
                </button>
            </div>
        );
    };
});

describe('Filter Component', () => {
    const mockOnSearch = jest.fn();

    beforeAll(() => {
        Object.defineProperty(window, 'hasPermissionToUpload', {
            value: 'True',
            writable: true,
        });
    });

    beforeEach(() => {
        jest.clearAllMocks();

        jest.spyOn(global, 'Date').mockImplementation(() => ({
            getFullYear: () => 2024,
            getMonth: () => 0,
        }));
    });

    afterEach(() => {
        jest.restoreAllMocks();
    });

    it('should render and call onSearch on mount', () => {
        render(<Filter onSearch={mockOnSearch} />);

        expect(mockOnSearch).toHaveBeenCalledWith({
            month: '2024-01-01',
            isLocal: true,
        });
    });

    it('should handle month change and search button click', () => {
        const { getByDisplayValue, getByText } = render(
            <Filter onSearch={mockOnSearch} />
        );

        const monthInput = getByDisplayValue('2024-01');
        fireEvent.change(monthInput, { target: { value: '2024-03' } });
        fireEvent.click(getByText('Search'));

        expect(mockOnSearch).toHaveBeenCalledWith({
            month: '2024-03-01',
            isLocal: true,
        });
    });

    it('should handle category change to Expat and search', () => {
        const { getByDisplayValue, getByText } = render(
            <Filter onSearch={mockOnSearch} />
        );

        const categorySelect = getByDisplayValue('Local');
        fireEvent.change(categorySelect, { target: { value: '0' } });
        fireEvent.click(getByText('Search'));

        expect(mockOnSearch).toHaveBeenCalledWith({
            month: '2024-01-01',
            isLocal: false,
        });
    });

    it('should call showPicker on month input click', () => {
        const { getByDisplayValue } = render(
            <Filter onSearch={mockOnSearch} />
        );

        const monthInput = getByDisplayValue('2024-01');
        monthInput.showPicker = jest.fn();
        fireEvent.click(monthInput);

        expect(monthInput.showPicker).toHaveBeenCalled();
    });

    it('should handle generate success', () => {
        const { getByText } = render(<Filter onSearch={mockOnSearch} />);

        fireEvent.click(getByText('Generate Payroll'));

        expect(mockOnSearch).toHaveBeenCalledWith({
            month: '2024-05-01',
            isLocal: true,
        });
    });

    it('should not render GenerateSalaryModal when permission is false', () => {
        Object.defineProperty(window, 'hasPermissionToUpload', {
            value: 'False',
            writable: true,
        });

        const { queryByText } = render(<Filter onSearch={mockOnSearch} />);

        expect(queryByText('Generate Payroll')).not.toBeInTheDocument();
    });
});
