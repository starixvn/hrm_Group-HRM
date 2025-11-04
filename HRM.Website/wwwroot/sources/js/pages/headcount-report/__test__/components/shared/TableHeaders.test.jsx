import React from 'react';
import { render } from '@testing-library/react';
import { renderFixedHeaders, renderGroupedHeaders } from '../../../components/shared/TableHeaders';

describe('TableHeaders', () => {
    describe('renderFixedHeaders', () => {
        it('should render Month and Category headers without rowspan', () => {
            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderFixedHeaders()}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            expect(headers.length).toBe(2);
            
            expect(headers[0].textContent).toBe('Month');
            expect(headers[0].className).toContain('header-cell');
            expect(headers[0].className).toContain('month-header');
            expect(headers[0].getAttribute('rowspan')).toBeNull();
            
            expect(headers[1].textContent).toBe('Category');
            expect(headers[1].className).toContain('header-cell');
            expect(headers[1].className).toContain('category-header');
            expect(headers[1].getAttribute('rowspan')).toBeNull();
        });

        it('should render Month and Category headers with rowspan', () => {
            const rowspan = 2;
            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderFixedHeaders(rowspan)}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            expect(headers.length).toBe(2);
            
            expect(headers[0].getAttribute('rowspan')).toBe(String(rowspan));
            expect(headers[1].getAttribute('rowspan')).toBe(String(rowspan));
        });

        it('should handle null rowspan', () => {
            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderFixedHeaders(null)}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            expect(headers[0].getAttribute('rowspan')).toBeNull();
            expect(headers[1].getAttribute('rowspan')).toBeNull();
        });
    });

    describe('renderGroupedHeaders', () => {
        it('should render grouped headers with colspan', () => {
            const groups = [
                { label: 'Group 1', colspan: 2, isGrandTotal: false, isUnitCategory: false },
                { label: 'Group 2', colspan: 3, isGrandTotal: false, isUnitCategory: false },
            ];

            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderGroupedHeaders(groups)}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            expect(headers.length).toBe(2);
            
            expect(headers[0].textContent).toBe('Group 1');
            expect(headers[0].getAttribute('colspan')).toBe('2');
            
            expect(headers[1].textContent).toBe('Group 2');
            expect(headers[1].getAttribute('colspan')).toBe('3');
        });

        it('should apply grand-total-header class for grand total groups', () => {
            const groups = [
                { label: 'Grand Total', colspan: 2, isGrandTotal: true, isUnitCategory: false },
                { label: 'Regular Group', colspan: 3, isGrandTotal: false, isUnitCategory: false },
            ];

            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderGroupedHeaders(groups)}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            expect(headers[0].className).toContain('grand-total-header');
            expect(headers[1].className).not.toContain('grand-total-header');
        });

        it('should apply unit-category-header class for unit category groups', () => {
            const groups = [
                { label: 'Unit Category', colspan: 2, isGrandTotal: false, isUnitCategory: true },
                { label: 'Regular Group', colspan: 3, isGrandTotal: false, isUnitCategory: false },
            ];

            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderGroupedHeaders(groups)}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            expect(headers[0].className).toContain('unit-category-header');
            expect(headers[1].className).not.toContain('unit-category-header');
        });

        it('should apply rowspan for grand total groups', () => {
            const groups = [
                { label: 'Grand Total', colspan: 2, isGrandTotal: true, isUnitCategory: false },
                { label: 'Regular Group', colspan: 3, isGrandTotal: false, isUnitCategory: false },
            ];

            const rowKey = 1;
            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderGroupedHeaders(groups, rowKey)}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            // Grand total should have rowspan = rowKey + 1 = 2
            expect(headers[0].getAttribute('rowspan')).toBe('2');
            // Regular group should not have rowspan
            expect(headers[1].getAttribute('rowspan')).toBeNull();
        });

        it('should handle empty groups array', () => {
            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderGroupedHeaders([])}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            expect(headers.length).toBe(0);
        });

        it('should render all groups correctly', () => {
            const groups = [
                { label: 'Group 1', colspan: 1, isGrandTotal: false, isUnitCategory: false },
                { label: 'Group 2', colspan: 1, isGrandTotal: false, isUnitCategory: false },
                { label: 'Group 3', colspan: 1, isGrandTotal: false, isUnitCategory: false },
            ];

            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderGroupedHeaders(groups, 1)}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            expect(headers.length).toBe(3);
            expect(headers[0].textContent).toBe('Group 1');
            expect(headers[1].textContent).toBe('Group 2');
            expect(headers[2].textContent).toBe('Group 3');
        });

        it('should handle groups with both grand total and unit category flags', () => {
            const groups = [
                { label: 'Special Group', colspan: 2, isGrandTotal: true, isUnitCategory: true },
            ];

            const { container } = render(
                <table>
                    <thead>
                        <tr>
                            {renderGroupedHeaders(groups)}
                        </tr>
                    </thead>
                </table>
            );

            const headers = container.querySelectorAll('th');
            expect(headers[0].className).toContain('grand-total-header');
            expect(headers[0].className).toContain('unit-category-header');
        });
    });
});

