import { render, waitFor, fireEvent } from '@testing-library/react';
import React from 'react';
import DetailContainer from '../../../components/detail/DetailContainer';

jest.mock('axios', () => ({
    get: jest.fn(),
}));

jest.mock('../../../../../shared/common.functions', () => ({
    blockUI: jest.fn(),
    unblockUI: jest.fn(),
    resolveClientUrl: jest.fn((url) => url),
}));

// Mock moment
jest.mock('moment', () => {
    const moment = jest.requireActual('moment');
    return (date) => {
        if (!date) return { format: () => '' };
        return moment(date);
    };
});

describe('DetailContainer Component', () => {
    const mockAxios = require('axios');
    const mockCommonFunctions = require('../../../../../shared/common.functions');

    beforeEach(() => {
        jest.clearAllMocks();
        
        // Reset window.location.search
        delete window.location;
        window.location = { search: '' };
        
        // Setup default axios mock
        mockAxios.get.mockResolvedValue({
            data: []
        });
    });

    it('should render page title and table', () => {
        const { container } = render(<DetailContainer />);

        expect(container.querySelector('.page-title')).toBeInTheDocument();
        expect(container.querySelector('.page-title').textContent).toBe('Headcount Report Detail');
        expect(container.querySelector('.table-data')).toBeInTheDocument();
        expect(container.querySelector('table')).toBeInTheDocument();
    });

    it('should render all column headers', () => {
        const { container } = render(<DetailContainer />);

        const headers = container.querySelectorAll('thead th');
        expect(headers.length).toBe(17); // 17 columns based on the component
        
        expect(headers[0].textContent).toContain('#');
        expect(headers[1].textContent).toContain('CtrId');
        expect(headers[2].textContent).toContain('EmpId');
    });

    it('should fetch data on mount when empIds are in URL', async () => {
        window.location.search = '?empIds=1,2,3';
        
        const mockData = [
            {
                ctrIdFormatted: 'CTR001',
                empId: 'EMP001',
                code: 'CODE001',
                nickname: 'Nickname1',
                bu: 'BU1',
                teamName: 'Team1',
                departmentName: 'Dept1',
                location: 'Location1',
                joinedDate: '2024-01-01',
                resignedDate: null,
                confirmedDate: '2024-02-01',
                isLocal: true,
                isSpecialUser: false,
                companyDate: '2024-01-01',
                company: 'Company1',
                subCompany: 'SubCompany1'
            }
        ];

        mockAxios.get.mockResolvedValueOnce({
            data: mockData
        });

        render(<DetailContainer />);

        await waitFor(() => {
            expect(mockAxios.get).toHaveBeenCalled();
            expect(mockAxios.get).toHaveBeenCalledWith(
                expect.stringContaining('HeadcountReport/GetDetail?empIds=1%2C2%2C3')
            );
            expect(mockCommonFunctions.blockUI).toHaveBeenCalled();
            expect(mockCommonFunctions.unblockUI).toHaveBeenCalled();
        });
    });

    it('should not fetch data when empIds are not in URL', async () => {
        window.location.search = '';

        render(<DetailContainer />);

        await waitFor(() => {
            expect(mockAxios.get).not.toHaveBeenCalled();
        });
    });

    it('should render table rows with data', async () => {
        window.location.search = '?empIds=1';
        
        const mockData = [
            {
                ctrIdFormatted: 'CTR001',
                empId: 'EMP001',
                code: 'CODE001',
                nickname: 'Nickname1',
                bu: 'BU1',
                teamName: 'Team1',
                departmentName: 'Dept1',
                location: 'Location1',
                joinedDate: '2024-01-01',
                resignedDate: null,
                confirmedDate: '2024-02-01',
                isLocal: true,
                isSpecialUser: false,
                companyDate: '2024-01-01',
                company: 'Company1',
                subCompany: 'SubCompany1'
            },
            {
                ctrIdFormatted: 'CTR002',
                empId: 'EMP002',
                code: 'CODE002',
                nickname: 'Nickname2',
                bu: 'BU2',
                teamName: 'Team2',
                departmentName: 'Dept2',
                location: 'Location2',
                joinedDate: '2024-03-01',
                resignedDate: '2024-12-01',
                confirmedDate: '2024-04-01',
                isLocal: false,
                isSpecialUser: true,
                companyDate: '2024-03-01',
                company: 'Company2',
                subCompany: 'SubCompany2'
            }
        ];

        mockAxios.get.mockResolvedValueOnce({
            data: mockData
        });

        const { container } = render(<DetailContainer />);

        await waitFor(() => {
            const rows = container.querySelectorAll('tbody tr');
            expect(rows.length).toBe(2);
        });

        const firstRow = container.querySelectorAll('tbody tr')[0];
        expect(firstRow.querySelectorAll('td')[0].textContent).toBe('1');
        expect(firstRow.querySelectorAll('td')[1].textContent).toBe('CTR001');
        expect(firstRow.querySelectorAll('td')[2].textContent).toBe('EMP001');
        expect(firstRow.querySelectorAll('td')[12].textContent).toBe('Yes'); // isLocal
        expect(firstRow.querySelectorAll('td')[13].textContent).toBe('No'); // isSpecialUser
    });

    it('should format dates correctly', async () => {
        window.location.search = '?empIds=1';
        
        const mockData = [
            {
                ctrIdFormatted: 'CTR001',
                empId: 'EMP001',
                code: 'CODE001',
                nickname: 'Nickname1',
                bu: 'BU1',
                teamName: 'Team1',
                departmentName: 'Dept1',
                location: 'Location1',
                joinedDate: '2024-01-15T00:00:00Z',
                resignedDate: null,
                confirmedDate: '2024-02-20T00:00:00Z',
                isLocal: true,
                isSpecialUser: false,
                companyDate: '2024-01-10T00:00:00Z',
                company: 'Company1',
                subCompany: 'SubCompany1'
            }
        ];

        mockAxios.get.mockResolvedValueOnce({
            data: mockData
        });

        const { container } = render(<DetailContainer />);

        await waitFor(() => {
            const rows = container.querySelectorAll('tbody tr');
            expect(rows.length).toBe(1);
        });

        const row = container.querySelectorAll('tbody tr')[0];
        const joinedDateCell = row.querySelectorAll('td')[9];
        const confirmedDateCell = row.querySelectorAll('td')[11];
        
        // Date should be formatted as 'DD MMM, YYYY'
        expect(joinedDateCell.textContent).toMatch(/\d{2} \w{3}, \d{4}/);
        expect(confirmedDateCell.textContent).toMatch(/\d{2} \w{3}, \d{4}/);
    });

    it('should handle empty dates', async () => {
        window.location.search = '?empIds=1';
        
        const mockData = [
            {
                ctrIdFormatted: 'CTR001',
                empId: 'EMP001',
                code: 'CODE001',
                nickname: 'Nickname1',
                bu: 'BU1',
                teamName: 'Team1',
                departmentName: 'Dept1',
                location: 'Location1',
                joinedDate: null,
                resignedDate: null,
                confirmedDate: null,
                isLocal: true,
                isSpecialUser: false,
                companyDate: null,
                company: 'Company1',
                subCompany: 'SubCompany1'
            }
        ];

        mockAxios.get.mockResolvedValueOnce({
            data: mockData
        });

        const { container } = render(<DetailContainer />);

        await waitFor(() => {
            const rows = container.querySelectorAll('tbody tr');
            expect(rows.length).toBe(1);
        });

        const row = container.querySelectorAll('tbody tr')[0];
        const joinedDateCell = row.querySelectorAll('td')[9];
        const resignedDateCell = row.querySelectorAll('td')[10];
        
        expect(joinedDateCell.textContent).toBe('');
        expect(resignedDateCell.textContent).toBe('');
    });

    it('should apply deleted-user-row class for deleted users', async () => {
        window.location.search = '?empIds=1';
        
        const mockData = [
            {
                ctrIdFormatted: 'CTR001',
                empId: 'EMP001',
                code: 'CODE001',
                nickname: 'Nickname1',
                bu: 'BU1',
                teamName: 'Team1',
                departmentName: 'Dept1',
                location: 'Location1',
                joinedDate: '2024-01-01',
                resignedDate: null,
                confirmedDate: '2024-02-01',
                isLocal: true,
                isSpecialUser: false,
                companyDate: '2024-01-01',
                company: 'Company1',
                subCompany: 'SubCompany1',
                isDeleted: true
            },
            {
                ctrIdFormatted: 'CTR002',
                empId: 'EMP002',
                code: 'CODE002',
                nickname: 'Nickname2',
                bu: 'BU2',
                teamName: 'Team2',
                departmentName: 'Dept2',
                location: 'Location2',
                joinedDate: '2024-03-01',
                resignedDate: null,
                confirmedDate: '2024-04-01',
                isLocal: false,
                isSpecialUser: true,
                companyDate: '2024-03-01',
                company: 'Company2',
                subCompany: 'SubCompany2',
                enabled: false
            }
        ];

        mockAxios.get.mockResolvedValueOnce({
            data: mockData
        });

        const { container } = render(<DetailContainer />);

        await waitFor(() => {
            const rows = container.querySelectorAll('tbody tr');
            expect(rows.length).toBe(2);
        });

        const firstRow = container.querySelectorAll('tbody tr')[0];
        const secondRow = container.querySelectorAll('tbody tr')[1];
        
        expect(firstRow.className).toContain('deleted-user-row');
        expect(secondRow.className).toContain('deleted-user-row');
    });

    it('should handle sorting when clicking sortable column', async () => {
        window.location.search = '?empIds=1';
        
        const mockData = [
            {
                ctrIdFormatted: 'CTR002',
                empId: 'EMP002',
                code: 'CODE002',
                nickname: 'Nickname2',
                bu: 'BU2',
                teamName: 'Team2',
                departmentName: 'Dept2',
                location: 'Location2',
                joinedDate: '2024-03-01',
                resignedDate: null,
                confirmedDate: '2024-04-01',
                isLocal: false,
                isSpecialUser: true,
                companyDate: '2024-03-01',
                company: 'Company2',
                subCompany: 'SubCompany2'
            },
            {
                ctrIdFormatted: 'CTR001',
                empId: 'EMP001',
                code: 'CODE001',
                nickname: 'Nickname1',
                bu: 'BU1',
                teamName: 'Team1',
                departmentName: 'Dept1',
                location: 'Location1',
                joinedDate: '2024-01-01',
                resignedDate: null,
                confirmedDate: '2024-02-01',
                isLocal: true,
                isSpecialUser: false,
                companyDate: '2024-01-01',
                company: 'Company1',
                subCompany: 'SubCompany1'
            }
        ];

        mockAxios.get.mockResolvedValueOnce({
            data: mockData
        });

        const { container } = render(<DetailContainer />);

        await waitFor(() => {
            const rows = container.querySelectorAll('tbody tr');
            expect(rows.length).toBe(2);
        });

        // Click on EmpId column header (3rd column, index 2)
        const headers = container.querySelectorAll('thead th');
        const empIdHeader = headers[2];
        
        // Verify initial order (CTR002 should be first)
        let rows = container.querySelectorAll('tbody tr');
        expect(rows[0].querySelectorAll('td')[2].textContent).toBe('EMP002');

        // Click to sort ascending
        fireEvent.click(empIdHeader);

        await waitFor(() => {
            rows = container.querySelectorAll('tbody tr');
            // After sorting, EMP001 should come first
            expect(rows[0].querySelectorAll('td')[2].textContent).toBe('EMP001');
        });

        // Click again to sort descending
        fireEvent.click(empIdHeader);

        await waitFor(() => {
            rows = container.querySelectorAll('tbody tr');
            // After descending sort, EMP002 should come first
            expect(rows[0].querySelectorAll('td')[2].textContent).toBe('EMP002');
        });
    });

    it('should not allow sorting on non-sortable columns', async () => {
        window.location.search = '?empIds=1';
        
        const mockData = [
            {
                ctrIdFormatted: 'CTR001',
                empId: 'EMP001',
                code: 'CODE001',
                nickname: 'Nickname1',
                bu: 'BU1',
                teamName: 'Team1',
                departmentName: 'Dept1',
                location: 'Location1',
                joinedDate: '2024-01-01',
                resignedDate: null,
                confirmedDate: '2024-02-01',
                isLocal: true,
                isSpecialUser: false,
                companyDate: '2024-01-01',
                company: 'Company1',
                subCompany: 'SubCompany1'
            }
        ];

        mockAxios.get.mockResolvedValueOnce({
            data: mockData
        });

        const { container } = render(<DetailContainer />);

        await waitFor(() => {
            const rows = container.querySelectorAll('tbody tr');
            expect(rows.length).toBe(1);
        });

        // First column (#) is not sortable
        const headers = container.querySelectorAll('thead th');
        const firstHeader = headers[0];
        
        expect(firstHeader.className).not.toContain('col-sorter');
        
        // Clicking should not change sort icon
        const initialIcon = firstHeader.querySelector('.fa-sort');
        fireEvent.click(firstHeader);
        
        // Should still have no sort icon or same icon
        expect(firstHeader.className).not.toContain('col-sorter');
    });

    it('should display sort icons correctly', async () => {
        window.location.search = '?empIds=1';
        
        const mockData = [
            {
                ctrIdFormatted: 'CTR001',
                empId: 'EMP001',
                code: 'CODE001',
                nickname: 'Nickname1',
                bu: 'BU1',
                teamName: 'Team1',
                departmentName: 'Dept1',
                location: 'Location1',
                joinedDate: '2024-01-01',
                resignedDate: null,
                confirmedDate: '2024-02-01',
                isLocal: true,
                isSpecialUser: false,
                companyDate: '2024-01-01',
                company: 'Company1',
                subCompany: 'SubCompany1'
            }
        ];

        mockAxios.get.mockResolvedValueOnce({
            data: mockData
        });

        const { container } = render(<DetailContainer />);

        await waitFor(() => {
            const rows = container.querySelectorAll('tbody tr');
            expect(rows.length).toBe(1);
        });

        const headers = container.querySelectorAll('thead th');
        const sortableHeader = headers[1]; // CtrId column
        
        // Initially should show sort icon (not sorted)
        expect(sortableHeader.querySelector('.fa-sort')).toBeInTheDocument();
        
        // Click to sort ascending
        fireEvent.click(sortableHeader);
        
        await waitFor(() => {
            expect(sortableHeader.querySelector('.fa-sort-asc')).toBeInTheDocument();
        });
        
        // Click to sort descending
        fireEvent.click(sortableHeader);
        
        await waitFor(() => {
            expect(sortableHeader.querySelector('.fa-sort-desc')).toBeInTheDocument();
        });
    });

    it('should handle empty data', async () => {
        window.location.search = '?empIds=1';
        
        mockAxios.get.mockResolvedValueOnce({
            data: []
        });

        const { container } = render(<DetailContainer />);

        await waitFor(() => {
            expect(mockAxios.get).toHaveBeenCalled();
        });

        const rows = container.querySelectorAll('tbody tr');
        expect(rows.length).toBe(0);
    });
});

