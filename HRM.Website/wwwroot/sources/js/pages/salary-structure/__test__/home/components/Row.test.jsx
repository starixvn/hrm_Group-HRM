import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import Row from '../../../home/components/Row';

// Mock resolveClientUrl
jest.mock('../../../../../shared/common.functions', () => ({
    resolveClientUrl: jest.fn((url) => `/test${url}`)
}));

// Mock window.hasPermission
const mockHasPermission = (value) => {
    Object.defineProperty(window, 'hasPermission', {
        value: value,
        writable: true
    });
};

// Mock window.location
const mockLocation = () => {
    Object.defineProperty(window, 'location', {
        value: { href: '' },
        writable: true
    });
};

// Mock document.querySelectorAll
const mockQuerySelectorAll = jest.fn();
Object.defineProperty(document, 'querySelectorAll', {
    value: mockQuerySelectorAll,
    writable: true
});

// Create mock elements with addEventListener
const createMockElement = () => ({
    addEventListener: jest.fn(),
    removeEventListener: jest.fn(),
    classList: {
        add: jest.fn(),
        remove: jest.fn(),
        contains: jest.fn()
    }
});

describe('Row Component', () => {
    const mockItem = {
        salaryStructureId: 1,
        name: 'Test Structure',
        details: [
            {
                jobLevelId: 1,
                name: 'Level 1',
                basicSalary: 1000,
                jobLevelAddOn: 200,
                expatSalary: 1500
            },
            {
                jobLevelId: 2,
                name: 'Level 2',
                basicSalary: 1200,
                jobLevelAddOn: 300,
                expatSalary: 1800
            }
        ]
    };

    const mockOnDelete = jest.fn();

    // Helper function to render Row component with proper table structure
    const renderRow = (props) => {
        return render(
            <table>
                <tbody>
                    <Row {...props} />
                </tbody>
            </table>
        );
    };

    beforeEach(() => {
        jest.clearAllMocks();
        mockHasPermission('True');
        mockLocation();

        const mockElements = [
            { classList: { add: jest.fn(), remove: jest.fn() } },
            { classList: { add: jest.fn(), remove: jest.fn() } }
        ];
        mockQuerySelectorAll.mockReturnValue(mockElements);
    });

    afterEach(() => {
        delete window.hasPermission;
        delete window.location;
    });

    describe('Rendering with Details', () => {
        it('should render structure name and details', () => {
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });

            expect(screen.getByText('Test Structure')).toBeInTheDocument();
            expect(screen.getByText('Level 1')).toBeInTheDocument();
            expect(screen.getByText('Level 2')).toBeInTheDocument();
        });

        it('should render formatted salary values', () => {
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });

            expect(screen.getByText('1,000')).toBeInTheDocument();
            expect(screen.getByText('1,200')).toBeInTheDocument();
            expect(screen.getByText('200')).toBeInTheDocument();
            expect(screen.getByText('300')).toBeInTheDocument();
            expect(screen.getByText('1,500')).toBeInTheDocument();
            expect(screen.getByText('1,800')).toBeInTheDocument();
        });

        it('should render row index', () => {
            renderRow({ rowIndex: 5, item: mockItem, onDelete: mockOnDelete });

            expect(screen.getByText('5')).toBeInTheDocument();
        });

        it('should render edit and delete buttons when hasPermission is True', () => {
            const { container } = renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });

            const editButton = container.querySelector('.icon-edit');
            const deleteButton = container.querySelector('.icon-delete');

            expect(editButton).toBeInTheDocument();
            expect(deleteButton).toBeInTheDocument();
        });

        it('should not render edit and delete buttons when hasPermission is False', () => {
            mockHasPermission('False');
            const { container } = renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });

            const editButton = container.querySelector('.icon-edit');
            const deleteButton = container.querySelector('.icon-delete');

            expect(editButton).not.toBeInTheDocument();
            expect(deleteButton).not.toBeInTheDocument();
        });

        it('should apply correct row classes for even index', () => {
            renderRow({ rowIndex: 2, item: mockItem, onDelete: mockOnDelete });

            const rows = screen.getAllByRole('row');
            rows.forEach(row => {
                expect(row).toHaveClass('row-group-1', 'odd-row');
            });
        });

        it('should apply correct row classes for odd index', () => {
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });

            const rows = screen.getAllByRole('row');
            rows.forEach(row => {
                expect(row).toHaveClass('row-group-1', 'even-row');
            });
        });
    });

    describe('Rendering without Details', () => {
        const itemWithoutDetails = {
            salaryStructureId: 2,
            name: 'Structure without details',
            details: []
        };

        it('should render structure name when no details', () => {
            renderRow({ rowIndex: 1, item: itemWithoutDetails, onDelete: mockOnDelete });

            expect(screen.getByText('Structure without details')).toBeInTheDocument();
        });

        it('should render empty cells for salary columns when no details', () => {
            renderRow({ rowIndex: 1, item: itemWithoutDetails, onDelete: mockOnDelete });

            const row = screen.getByRole('row');
            const cells = row.querySelectorAll('td');

            expect(cells[2].textContent).toBe('');
            expect(cells[3].textContent).toBe('');
            expect(cells[4].textContent).toBe('');
            expect(cells[5].textContent).toBe('');
        });

        it('should render edit and delete buttons when no details and hasPermission is True', () => {
            const { container } = renderRow({ rowIndex: 1, item: itemWithoutDetails, onDelete: mockOnDelete });

            const editButton = container.querySelector('.icon-edit');
            const deleteButton = container.querySelector('.icon-delete');

            expect(editButton).toBeInTheDocument();
            expect(deleteButton).toBeInTheDocument();
        });
    });

    describe('Edit Functionality', () => {
        it('should navigate to edit page when edit button is clicked', () => {
            const { container } = renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });

            const editButton = container.querySelector('.icon-edit');

            fireEvent.click(editButton);

            expect(window.location).toBe('/testsalarystructure/edit/1');
        });

        it('should navigate to edit page with correct ID', () => {
            const itemWithDifferentId = {
                ...mockItem,
                salaryStructureId: 999
            };

            const { container } = renderRow({ rowIndex: 1, item: itemWithDifferentId, onDelete: mockOnDelete });

            const editButton = container.querySelector('.icon-edit');

            fireEvent.click(editButton);

            expect(window.location).toBe('/testsalarystructure/edit/999');
        });
    });

    describe('Delete Functionality', () => {
        it('should call onDelete when delete button is clicked', () => {
            const { container } = renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });
            
            const deleteButton = container.querySelector('.icon-delete');
            
            fireEvent.click(deleteButton);
            
            expect(mockOnDelete).toHaveBeenCalledWith(mockItem);
        });

        it('should call onDelete with correct item', () => {
            const customItem = {
                salaryStructureId: 123,
                name: 'Custom Structure',
                details: []
            };
            
            const { container } = renderRow({ rowIndex: 1, item: customItem, onDelete: mockOnDelete });
            
            const deleteButton = container.querySelector('.icon-delete');
            
            fireEvent.click(deleteButton);
            
            expect(mockOnDelete).toHaveBeenCalledWith(customItem);
        });
    });

    describe('Salary Formatting', () => {
        it('should format null salary values as empty string', () => {
            const itemWithNullSalaries = {
                salaryStructureId: 1,
                name: 'Test Structure',
                details: [
                    {
                        jobLevelId: 1,
                        name: 'Level 1',
                        basicSalary: null,
                        jobLevelAddOn: null,
                        expatSalary: null
                    }
                ]
            };

            renderRow({ rowIndex: 1, item: itemWithNullSalaries, onDelete: mockOnDelete });

            const row = screen.getByRole('row');
            const cells = row.querySelectorAll('td');

            expect(cells[3].textContent).toBe('');
            expect(cells[4].textContent).toBe('');
            expect(cells[5].textContent).toBe('');
        });

        it('should format undefined salary values as empty string', () => {
            const itemWithUndefinedSalaries = {
                salaryStructureId: 1,
                name: 'Test Structure',
                details: [
                    {
                        jobLevelId: 1,
                        name: 'Level 1',
                        basicSalary: undefined,
                        jobLevelAddOn: undefined,
                        expatSalary: undefined
                    }
                ]
            };

            renderRow({ rowIndex: 1, item: itemWithUndefinedSalaries, onDelete: mockOnDelete });

            const row = screen.getByRole('row');
            const cells = row.querySelectorAll('td');

            expect(cells[3].textContent).toBe('');
            expect(cells[4].textContent).toBe('');
            expect(cells[5].textContent).toBe('');
        });

        it('should format empty string salary values as empty string', () => {
            const itemWithEmptyStringSalaries = {
                salaryStructureId: 1,
                name: 'Test Structure',
                details: [
                    {
                        jobLevelId: 1,
                        name: 'Level 1',
                        basicSalary: '',
                        jobLevelAddOn: '',
                        expatSalary: ''
                    }
                ]
            };

            renderRow({ rowIndex: 1, item: itemWithEmptyStringSalaries, onDelete: mockOnDelete });

            const row = screen.getByRole('row');
            const cells = row.querySelectorAll('td');

            expect(cells[3].textContent).toBe('');
            expect(cells[4].textContent).toBe('');
            expect(cells[5].textContent).toBe('');
        });

        it('should format NaN salary values as empty string', () => {
            const itemWithNaNSalaries = {
                salaryStructureId: 1,
                name: 'Test Structure',
                details: [
                    {
                        jobLevelId: 1,
                        name: 'Level 1',
                        basicSalary: NaN,
                        jobLevelAddOn: NaN,
                        expatSalary: NaN
                    }
                ]
            };

            renderRow({ rowIndex: 1, item: itemWithNaNSalaries, onDelete: mockOnDelete });

            const row = screen.getByRole('row');
            const cells = row.querySelectorAll('td');

            expect(cells[3].textContent).toBe('');
            expect(cells[4].textContent).toBe('');
            expect(cells[5].textContent).toBe('');
        });

        it('should format large numbers correctly', () => {
            const itemWithLargeNumbers = {
                salaryStructureId: 1,
                name: 'Test Structure',
                details: [
                    {
                        jobLevelId: 1,
                        name: 'Level 1',
                        basicSalary: 1234567.89,
                        jobLevelAddOn: 987654.32,
                        expatSalary: 2345678.90
                    }
                ]
            };

            renderRow({ rowIndex: 1, item: itemWithLargeNumbers, onDelete: mockOnDelete });

            expect(screen.getByText('1,234,567.89')).toBeInTheDocument();
            expect(screen.getByText('987,654.32')).toBeInTheDocument();
            expect(screen.getByText('2,345,678.9')).toBeInTheDocument();
        });

        it('should format zero values correctly', () => {
            const itemWithZeroValues = {
                salaryStructureId: 1,
                name: 'Test Structure',
                details: [
                    {
                        jobLevelId: 1,
                        name: 'Level 1',
                        basicSalary: 0,
                        jobLevelAddOn: 0,
                        expatSalary: 0
                    }
                ]
            };

            renderRow({ rowIndex: 1, item: itemWithZeroValues, onDelete: mockOnDelete });

            const zeroElements = screen.getAllByText('0');
            expect(zeroElements).toHaveLength(3);
        });
    });

    describe('Row Spanning', () => {
        it('should apply rowspan to structure name when multiple details', () => {
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });

            const rows = screen.getAllByRole('row');
            const firstRow = rows[0];
            const structureNameCell = firstRow.querySelector('td:nth-child(2)');

            expect(structureNameCell).toHaveAttribute('rowspan', '2');
        });

        it('should apply rowspan to row index when multiple details', () => {
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });

            const rows = screen.getAllByRole('row');
            const firstRow = rows[0];
            const rowIndexCell = firstRow.querySelector('td:nth-child(1)');

            expect(rowIndexCell).toHaveAttribute('rowspan', '2');
        });

        it('should apply rowspan to action buttons when multiple details', () => {
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });

            const rows = screen.getAllByRole('row');
            const firstRow = rows[0];
            const editCell = firstRow.querySelector('td:nth-child(7)');
            const deleteCell = firstRow.querySelector('td:nth-child(8)');

            expect(editCell).toHaveAttribute('rowspan', '2');
            expect(deleteCell).toHaveAttribute('rowspan', '2');
        });

        it('should not apply rowspan when single detail', () => {
            const itemWithSingleDetail = {
                salaryStructureId: 1,
                name: 'Test Structure',
                details: [
                    {
                        jobLevelId: 1,
                        name: 'Level 1',
                        basicSalary: 1000,
                        jobLevelAddOn: 200,
                        expatSalary: 1500
                    }
                ]
            };

            renderRow({ rowIndex: 1, item: itemWithSingleDetail, onDelete: mockOnDelete });

            const row = screen.getByRole('row');
            const structureNameCell = row.querySelector('td:nth-child(2)');

            expect(structureNameCell).toHaveAttribute('rowspan', '1');
        });
    });

    describe('Mouse Events', () => {
        it('should call querySelectorAll on mouse enter', () => {
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });
            
            const rows = screen.getAllByRole('row');
            const firstRow = rows[0];
            
            fireEvent.mouseEnter(firstRow);
            
            expect(mockQuerySelectorAll).toHaveBeenCalledWith('.row-group-1');
        });

        it('should call querySelectorAll on mouse leave', () => {
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });
            
            const rows = screen.getAllByRole('row');
            const firstRow = rows[0];
            
            fireEvent.mouseLeave(firstRow);
            
            expect(mockQuerySelectorAll).toHaveBeenCalledWith('.row-group-1');
        });

        it('should add hovered-row class to elements on mouse enter', () => {
            const mockElements = [
                { classList: { add: jest.fn(), remove: jest.fn() } },
                { classList: { add: jest.fn(), remove: jest.fn() } }
            ];
            mockQuerySelectorAll.mockReturnValue(mockElements);
            
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });
            
            const rows = screen.getAllByRole('row');
            const firstRow = rows[0];
            
            fireEvent.mouseEnter(firstRow);
            
            mockElements.forEach(element => {
                expect(element.classList.add).toHaveBeenCalledWith('hovered-row');
            });
        });

        it('should remove hovered-row class from elements on mouse leave', () => {
            const mockElements = [
                { classList: { add: jest.fn(), remove: jest.fn() } },
                { classList: { add: jest.fn(), remove: jest.fn() } }
            ];
            mockQuerySelectorAll.mockReturnValue(mockElements);
            
            renderRow({ rowIndex: 1, item: mockItem, onDelete: mockOnDelete });
            
            const rows = screen.getAllByRole('row');
            const firstRow = rows[0];
            
            fireEvent.mouseLeave(firstRow);
            
            mockElements.forEach(element => {
                expect(element.classList.remove).toHaveBeenCalledWith('hovered-row');
            });
        });
    });

    describe('Edge Cases', () => {
        it('should handle item with very long name', () => {
            const itemWithLongName = {
                salaryStructureId: 1,
                name: 'This is a very long salary structure name that might cause layout issues',
                details: []
            };

            renderRow({ rowIndex: 1, item: itemWithLongName, onDelete: mockOnDelete });

            expect(screen.getByText('This is a very long salary structure name that might cause layout issues')).toBeInTheDocument();
        });

        it('should handle item with special characters in name', () => {
            const itemWithSpecialChars = {
                salaryStructureId: 1,
                name: 'Structure with Special Chars: @#$%^&*()',
                details: []
            };

            renderRow({ rowIndex: 1, item: itemWithSpecialChars, onDelete: mockOnDelete });

            expect(screen.getByText('Structure with Special Chars: @#$%^&*()')).toBeInTheDocument();
        });

        it('should handle item with unicode characters', () => {
            const itemWithUnicode = {
                salaryStructureId: 1,
                name: 'Cấu trúc lương với ký tự Unicode',
                details: []
            };

            renderRow({ rowIndex: 1, item: itemWithUnicode, onDelete: mockOnDelete });

            expect(screen.getByText('Cấu trúc lương với ký tự Unicode')).toBeInTheDocument();
        });
    });
});
