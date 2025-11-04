import React from 'react';
import { render } from '@testing-library/react';
import TableWrapper from '../../../components/shared/TableWrapper';

describe('TableWrapper Component', () => {
    it('should render wrapper with correct structure', () => {
        const { container } = render(
            <TableWrapper>
                <thead>
                    <tr>
                        <th>Header</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Data</td>
                    </tr>
                </tbody>
            </TableWrapper>
        );

        const wrapper = container.querySelector('.headcount-table-wrapper');
        expect(wrapper).toBeInTheDocument();

        const containerDiv = container.querySelector('.headcount-table-container');
        expect(containerDiv).toBeInTheDocument();

        const table = container.querySelector('.table.table-custom.headcount-table');
        expect(table).toBeInTheDocument();
    });

    it('should render children inside table', () => {
        const { container } = render(
            <TableWrapper>
                <thead>
                    <tr>
                        <th>Column 1</th>
                        <th>Column 2</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Value 1</td>
                        <td>Value 2</td>
                    </tr>
                </tbody>
            </TableWrapper>
        );

        const thead = container.querySelector('thead');
        expect(thead).toBeInTheDocument();
        expect(thead.querySelectorAll('th').length).toBe(2);

        const tbody = container.querySelector('tbody');
        expect(tbody).toBeInTheDocument();
        expect(tbody.querySelectorAll('td').length).toBe(2);
    });

    it('should render multiple children', () => {
        const { container } = render(
            <TableWrapper>
                <thead>
                    <tr>
                        <th>Header</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Row 1</td>
                    </tr>
                </tbody>
                <tbody>
                    <tr>
                        <td>Row 2</td>
                    </tr>
                </tbody>
            </TableWrapper>
        );

        const tbodyElements = container.querySelectorAll('tbody');
        expect(tbodyElements.length).toBe(2);
    });

    it('should apply correct CSS classes', () => {
        const { container } = render(
            <TableWrapper>
                <thead>
                    <tr>
                        <th>Header</th>
                    </tr>
                </thead>
            </TableWrapper>
        );

        const wrapper = container.querySelector('.headcount-table-wrapper');
        expect(wrapper).toBeInTheDocument();

        const containerDiv = container.querySelector('.headcount-table-container');
        expect(containerDiv).toBeInTheDocument();

        const table = container.querySelector('table');
        expect(table.className).toContain('table');
        expect(table.className).toContain('table-custom');
        expect(table.className).toContain('headcount-table');
    });

    it('should handle empty children', () => {
        const { container } = render(<TableWrapper></TableWrapper>);

        const wrapper = container.querySelector('.headcount-table-wrapper');
        expect(wrapper).toBeInTheDocument();

        const table = container.querySelector('table');
        expect(table).toBeInTheDocument();
        expect(table.children.length).toBe(0);
    });
});

