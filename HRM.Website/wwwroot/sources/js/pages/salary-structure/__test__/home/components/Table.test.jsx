// Table.test.jsx
import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import Table from '../../../home/components/Table';

jest.mock('../../../home/components/Row', () => ({ item, rowIndex }) => (
    <tr data-testid="mock-row">
        <td>{rowIndex}</td>
        <td>{item.name}</td>
    </tr>
));

describe('Table component', () => {
    const mockData = [
        { salaryStructureId: 1, name: 'Alpha', jobLevelName: 'L1', basicSalary: 1000 },
        { salaryStructureId: 2, name: 'Bravo', jobLevelName: 'L2', basicSalary: 2000 },
        { salaryStructureId: 3, name: 'Charlie', jobLevelName: 'L3', basicSalary: 1500 }
    ];

    const mockOnDelete = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
    });

    const renderTable = (permission = 'False', data = mockData) => {
        window.hasPermission = permission;
        return render(<Table data={data} onDelete={mockOnDelete} />);
    };

    it('renders with correct columns when hasPermission = True', () => {
        renderTable('True');
        const headers = screen.getAllByRole('columnheader');
        expect(headers.map(th => th.textContent)).toEqual(
            expect.arrayContaining([
                '#',
                'Salary Structure',
                'Job Level',
                'Basic Salary',
                'Job Level Add-On',
                'Basic Salary (USD)',
                'Job Level Add-On (USD)',
                'Edit',
                'Delete'
            ])
        );
    });

    it('renders with correct columns when hasPermission = False', () => {
        renderTable('False');
        const headers = screen.getAllByRole('columnheader');
        expect(headers.map(th => th.textContent)).toEqual(
            expect.arrayContaining([
                '#',
                'Salary Structure',
                'Job Level',
                'Basic Salary',
                'Job Level Add-On',
                'Basic Salary (USD)',
                'Job Level Add-On (USD)'
            ])
        );
        expect(headers.length).toBe(7);
    });

    it('renders all rows when data provided', () => {
        renderTable();
        expect(screen.getAllByTestId('mock-row')).toHaveLength(3);
    });

    it('renders "No information available" when no data', () => {
        renderTable('False', []);
        expect(screen.getByText('No information available')).toBeInTheDocument();
    });

    it('should sort ascending and descending when clicking sortable header', () => {
        renderTable();

        const salaryHeader = screen.getByText('Salary Structure');
        // Sort ascending
        fireEvent.click(salaryHeader);
        const firstAscRow = screen.getAllByTestId('mock-row')[0];
        expect(firstAscRow).toHaveTextContent('Alpha');

        // Sort descending
        fireEvent.click(salaryHeader);
        const firstDescRow = screen.getAllByTestId('mock-row')[0];
        expect(firstDescRow).toHaveTextContent('Charlie');
    });

    it('shows fa-sort, fa-sort-asc, fa-sort-desc icons correctly', () => {
        renderTable();
        const salaryHeader = screen.getByText('Salary Structure');

        // Before click
        expect(salaryHeader.querySelector('.fa-sort')).toBeInTheDocument();

        // Ascending
        fireEvent.click(salaryHeader);
        expect(salaryHeader.querySelector('.fa-sort-asc')).toBeInTheDocument();

        // Descending
        fireEvent.click(salaryHeader);
        expect(salaryHeader.querySelector('.fa-sort-desc')).toBeInTheDocument();
    });
});
