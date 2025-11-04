import { render, fireEvent, screen } from '@testing-library/react';
import React from 'react';
import Filter from '../../components/Filter';

jest.mock('../../../../shared/common.functions', () => ({
    resolveClientUrl: jest.fn((url) => url),
}));

describe('Filter Component', () => {
    const mockOnYearChange = jest.fn();
    const mockOnSearch = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
        delete window.location;
        window.location = { href: '' };
    });

    it('should render year select and buttons', () => {
        const { container } = render(
            <Filter 
                selectedYear={2024} 
                onYearChange={mockOnYearChange}
                onSearch={mockOnSearch}
            />
        );

        const yearSelect = container.querySelector('select#yearSelect');
        expect(yearSelect).toBeInTheDocument();
        
        expect(screen.getByText('Search')).toBeInTheDocument();
        expect(screen.getByText('Export All')).toBeInTheDocument();
    });

    it('should call onYearChange when year is changed', () => {
        const { container } = render(
            <Filter 
                selectedYear={2024} 
                onYearChange={mockOnYearChange}
                onSearch={mockOnSearch}
            />
        );

        const yearSelect = container.querySelector('select#yearSelect');
        fireEvent.change(yearSelect, { target: { value: '2023' } });

        expect(mockOnYearChange).toHaveBeenCalledWith(2023);
    });

    it('should call onSearch when search button is clicked', () => {
        render(
            <Filter 
                selectedYear={2024} 
                onYearChange={mockOnYearChange}
                onSearch={mockOnSearch}
            />
        );

        const searchButton = screen.getByText('Search');
        fireEvent.click(searchButton);

        expect(mockOnSearch).toHaveBeenCalled();
    });

    it('should export to excel when export button is clicked', () => {
        render(
            <Filter 
                selectedYear={2024} 
                onYearChange={mockOnYearChange}
                onSearch={mockOnSearch}
            />
        );

        const exportButton = screen.getByText('Export All');
        fireEvent.click(exportButton);

        expect(window.location.href).toContain('HeadcountReport/ExportExcel');
        expect(window.location.href).toContain('year=2024');
    });

    it('should generate correct years list', () => {
        const currentYear = new Date().getFullYear();
        const { container } = render(
            <Filter 
                selectedYear={currentYear} 
                onYearChange={mockOnYearChange}
                onSearch={mockOnSearch}
            />
        );

        const yearSelect = container.querySelector('select#yearSelect');
        const options = yearSelect.querySelectorAll('option');
        
        expect(options.length).toBeGreaterThan(0);
        expect(parseInt(options[0].value)).toBe(currentYear);
    });

    it('should have current year selected by default', () => {
        const currentYear = new Date().getFullYear();
        const { container } = render(
            <Filter 
                selectedYear={currentYear} 
                onYearChange={mockOnYearChange}
                onSearch={mockOnSearch}
            />
        );

        const yearSelect = container.querySelector('select#yearSelect');
        expect(yearSelect.value).toBe(currentYear.toString());
    });
});

