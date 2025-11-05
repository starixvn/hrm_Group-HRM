import { render, fireEvent } from '@testing-library/react';
import React from 'react';
import TableBody from '../../../components/shared/TableBody';

jest.mock('../../../../../shared/common.functions', () => ({
    resolveClientUrl: jest.fn((url) => url),
}));
const mockOpen = jest.fn();
window.open = mockOpen;

describe('TableBody Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
        mockOpen.mockClear();
    });

    it('should render table body with data', () => {
        const data = [
            {
                label1: 'Unit 1',
                details: [
                    {
                        month: 1,
                        newHire: 5,
                        newHireEmpIds: [1, 2, 3, 4, 5],
                        probationResign: 2,
                        probationResignEmpIds: [6, 7],
                        confirmedResign: 1,
                        confirmedResignEmpIds: [8],
                        transfer: 0,
                        transferInEmpIds: [],
                        transferOutEmpIds: [],
                        currentHeadCount: 10,
                        currentHeadCountEmpIds: [1, 2, 3, 4, 5, 9, 10, 11, 12, 13]
                    },
                    {
                        month: 2,
                        newHire: 0,
                        newHireEmpIds: [],
                        probationResign: 0,
                        probationResignEmpIds: [],
                        confirmedResign: 0,
                        confirmedResignEmpIds: [],
                        transfer: 0,
                        transferInEmpIds: [],
                        transferOutEmpIds: [],
                        currentHeadCount: 10,
                        currentHeadCountEmpIds: []
                    }
                ]
            }
        ];

        const columns = [{ label: 'Unit 1', key: 'Unit 1' }];

        const { container } = render(
            <table>
                <TableBody data={data} columns={columns} />
            </table>
        );

        expect(container.querySelector('tbody')).toBeInTheDocument();
        expect(container.querySelectorAll('tbody tr').length).toBeGreaterThan(0);
    });

    it('should render month labels', () => {
        const data = [
            {
                label1: 'Unit 1',
                details: [{
                    month: 1,
                    newHire: 0,
                    newHireEmpIds: [],
                    probationResign: 0,
                    probationResignEmpIds: [],
                    confirmedResign: 0,
                    confirmedResignEmpIds: [],
                    transfer: 0,
                    transferInEmpIds: [],
                    transferOutEmpIds: [],
                    currentHeadCount: 0,
                    currentHeadCountEmpIds: []
                }]
            }
        ];

        const columns = [{ label: 'Unit 1', key: 'Unit 1' }];

        const { getByText } = render(
            <table>
                <TableBody data={data} columns={columns} />
            </table>
        );

        // getMaxMonth returns 1 (max month value), Array.from({ length: 1 }) creates 1 element (index 0)
        // monthNames[0] = 'Jan'
        expect(getByText('Jan')).toBeInTheDocument();
    });

    it('should render category labels', () => {
        const data = [
            {
                label1: 'Unit 1',
                details: [{
                    month: 1,
                    newHire: 0,
                    newHireEmpIds: [],
                    probationResign: 0,
                    probationResignEmpIds: [],
                    confirmedResign: 0,
                    confirmedResignEmpIds: [],
                    transfer: 0,
                    transferInEmpIds: [],
                    transferOutEmpIds: [],
                    currentHeadCount: 0,
                    currentHeadCountEmpIds: []
                }]
            }
        ];

        const columns = [{ label: 'Unit 1', key: 'Unit 1' }];

        const { getByText } = render(
            <table>
                <TableBody data={data} columns={columns} />
            </table>
        );

        expect(getByText('New Hire')).toBeInTheDocument();
        expect(getByText('Probation Resign')).toBeInTheDocument();
        expect(getByText('Confirmed Resign')).toBeInTheDocument();
        expect(getByText('Transfer')).toBeInTheDocument();
        expect(getByText('Current H/C')).toBeInTheDocument();
    });

    it('should open detail page when clicking cell with employee IDs', () => {
        const data = [
            {
                label1: 'Unit 1',
                details: [
                    {
                        month: 1,
                        newHire: 2,
                        newHireEmpIds: [1, 2],
                        probationResign: 0,
                        probationResignEmpIds: [],
                        confirmedResign: 0,
                        confirmedResignEmpIds: [],
                        transfer: 0,
                        transferInEmpIds: [],
                        transferOutEmpIds: [],
                        currentHeadCount: 0,
                        currentHeadCountEmpIds: []
                    }
                ]
            }
        ];

        const columns = [{ label: 'Unit 1', key: 'Unit 1' }];

        const { container } = render(
            <table>
                <TableBody data={data} columns={columns} />
            </table>
        );

        const link = container.querySelector('a');
        expect(link).toBeInTheDocument();
        
        fireEvent.click(link);

        expect(mockOpen).toHaveBeenCalledWith(
            expect.stringContaining('HeadcountReport/Detail?empIds=1,2'),
            '_blank'
        );
    });

    it('should not render link when cell has no employee IDs', () => {
        const data = [
            {
                label1: 'Unit 1',
                details: [
                    {
                        month: 1,
                        newHire: 0,
                        newHireEmpIds: [],
                        probationResign: 0,
                        probationResignEmpIds: [],
                        confirmedResign: 0,
                        confirmedResignEmpIds: [],
                        transfer: 0,
                        transferInEmpIds: [],
                        transferOutEmpIds: [],
                        currentHeadCount: 0,
                        currentHeadCountEmpIds: []
                    }
                ]
            }
        ];

        const columns = [{ label: 'Unit 1', key: 'Unit 1' }];

        const { container } = render(
            <table>
                <TableBody data={data} columns={columns} />
            </table>
        );

        const links = container.querySelectorAll('a');
        const newHireCells = Array.from(links).filter(link => 
            link.textContent === '0'
        );
        expect(newHireCells.length).toBe(0);
    });

    it('should handle useLabel3ForCategory prop', () => {
        // Note: Component calls getUniqueLabel3Values(data) but function expects columns
        // However, both data and columns arrays contain objects with label3 property
        // so the function will work correctly
        const data = [
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category A',
                details: [
                    {
                        month: 1,
                        newHire: 0,
                        newHireEmpIds: [],
                        probationResign: 0,
                        probationResignEmpIds: [],
                        confirmedResign: 0,
                        confirmedResignEmpIds: [],
                        transfer: 0,
                        transferInEmpIds: [],
                        transferOutEmpIds: [],
                        currentHeadCount: 5,
                        currentHeadCountEmpIds: [1, 2, 3, 4, 5]
                    }
                ]
            },
            {
                label1: 'Unit 1',
                label2: 'Company 1',
                label3: 'Category B',
                details: [
                    {
                        month: 1,
                        newHire: 0,
                        newHireEmpIds: [],
                        probationResign: 0,
                        probationResignEmpIds: [],
                        confirmedResign: 0,
                        confirmedResignEmpIds: [],
                        transfer: 0,
                        transferInEmpIds: [],
                        transferOutEmpIds: [],
                        currentHeadCount: 3,
                        currentHeadCountEmpIds: [6, 7, 8]
                    }
                ]
            }
        ];

        const columns = [
            { 
                label: 'Unit 1', 
                key: 'Unit 1',
                label3: 'Category A',
                originalIndices: [0]
            },
            { 
                label: 'Unit 1', 
                key: 'Unit 1',
                label3: 'Category B',
                originalIndices: [1]
            }
        ];

        const { container } = render(
            <table>
                <TableBody data={data} columns={columns} useLabel3ForCategory={true} />
            </table>
        );

        expect(container.querySelector('tbody')).toBeInTheDocument();
        
        // getMaxMonth returns 1 (max month value from details)
        // Array.from({ length: 1 }) creates array with 1 element (index 0)
        // getUniqueLabel3Values(data) will work since data items have label3 property
        // Should render at least some rows
        const rows = container.querySelectorAll('tbody tr');
        expect(rows.length).toBeGreaterThan(0);
    });

    it('should apply correct CSS classes for bold cells', () => {
        const data = [
            {
                label1: 'Unit 1',
                isGrandTotal: true,
                details: [{
                    month: 1,
                    newHire: 0,
                    newHireEmpIds: [],
                    probationResign: 0,
                    probationResignEmpIds: [],
                    confirmedResign: 0,
                    confirmedResignEmpIds: [],
                    transfer: 0,
                    transferInEmpIds: [],
                    transferOutEmpIds: [],
                    currentHeadCount: 0,
                    currentHeadCountEmpIds: []
                }]
            }
        ];

        const columns = [{ 
            label: 'Unit 1', 
            key: 'Unit 1',
            isGrandTotal: true
        }];

        const { container } = render(
            <table>
                <TableBody data={data} columns={columns} />
            </table>
        );

        const boldCells = container.querySelectorAll('.bold-cell');
        expect(boldCells.length).toBeGreaterThan(0);
    });

    it('should handle empty data', () => {
        const columns = [{ label: 'Unit 1', key: 'Unit 1' }];

        const { container } = render(
            <table>
                <TableBody data={[]} columns={columns} />
            </table>
        );

        expect(container.querySelector('tbody')).toBeInTheDocument();
        expect(container.querySelectorAll('tbody tr').length).toBe(0);
    });
});

