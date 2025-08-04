import { render } from '@testing-library/react';
import React from 'react';
import SalaryTable from '../../components/SalaryTable';

describe('SalaryTable Component', () => {
    it('should render component with data', () => {
        const data = [
            {
                code: '00328',
                fullName: 'Trương Ân Ân',
                nickName: 'Annie',
                displayJoinedDate: '15 Oct, 2004',
                displayConfirmedDate: '16 Dec, 2004',
                basicSalary: 15000,
                standardWorkingDay: 22,
                unpaidLeave: 0,
                paidDay: 22,
                grossSalary: 15000,
                employmentStatus: 0,
            },
        ];
        const { getAllByText, getByText } = render(<SalaryTable data={data} />);

        expect(getByText('Trương Ân Ân')).toBeInTheDocument();
        expect(getAllByText('15,000').length).toBeGreaterThan(0);
    });

    it('should render empty table with no data', () => {
        const { getByText, container } = render(<SalaryTable data={[]} />);

        expect(getByText('Total')).toBeInTheDocument();
        expect(container.querySelector('tbody').children.length).toBe(0);
    });

    it('should render multiple employees', () => {
        const data = [
            {
                code: 'EMP001',
                fullName: 'John Doe',
                basicSalary: 1000,
                standardWorkingDay: 20,
                unpaidLeave: 1,
                paidDay: 19,
                grossSalary: 950,
                employmentStatus: 2,
            },
            {
                code: 'EMP002',
                fullName: 'Jane Smith',
                basicSalary: 1500,
                standardWorkingDay: 22,
                unpaidLeave: 0,
                paidDay: 22,
                grossSalary: 1500,
                employmentStatus: 1,
            },
        ];

        const { getByText, container } = render(<SalaryTable data={data} />);

        expect(getByText('John Doe')).toBeInTheDocument();
        expect(getByText('2,500')).toBeInTheDocument();
        expect(container.querySelectorAll('tbody tr').length).toBe(2);
    });

    it('should apply employment status styling', () => {
        const data = [
            {
                code: 'EMP001',
                fullName: 'John Doe',
                basicSalary: 1000,
                employmentStatus: 1,
            },
        ];

        const { container } = render(<SalaryTable data={data} />);
        const row = container.querySelector('tbody tr');
        expect(row).toHaveStyle({ color: 'green' });
    });

    it('should format large numbers', () => {
        const data = [
            {
                code: 'EMP001',
                fullName: 'CEO',
                basicSalary: 1234567,
                grossSalary: 1234567,
                employmentStatus: 2,
            },
        ];

        const { getAllByText } = render(<SalaryTable data={data} />);
        expect(getAllByText('1,234,567').length).toBeGreaterThan(0);
    });

    it('should render table structure', () => {
        const data = [
            { code: 'TEST', fullName: 'Test User', basicSalary: 1000 },
        ];
        const { container } = render(<SalaryTable data={data} />);

        expect(container.querySelector('table')).toHaveClass(
            'table',
            'table-custom'
        );
        expect(container.querySelector('thead')).toBeInTheDocument();
        expect(container.querySelectorAll('thead th').length).toBe(11);
    });
});
