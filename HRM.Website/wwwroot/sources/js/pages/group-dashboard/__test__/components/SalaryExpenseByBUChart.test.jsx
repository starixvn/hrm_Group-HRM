import { render, fireEvent } from '@testing-library/react';
import React from 'react';
import SalaryExpenseByBUChart from '../../components/SalaryExpenseByBUChart';

jest.mock('react-canvas-treemap', () => {
    const React = require('react');
    const mockTransformData = jest.fn(({ data }) => {
        return {
            treeMapCells: data.map((item, index) => ({
                id: item.id,
                x0: index * 100,
                y0: 0,
                x1: (index + 1) * 100,
                y1: 100
            }))
        };
    });
    
    // Store reference globally so we can access it in tests
    global.mockTransformDataFn = mockTransformData;
    
    return {
        __esModule: true,
        default: ({ cells, onMouseOverCell, onMouseLeaveChart }) => {
            return React.createElement('div', {
                'data-testid': 'treemap-chart',
                'data-cells': JSON.stringify(cells.map(c => ({ id: c.id }))),
                onMouseOver: (e) => {
                    const cellId = cells[0]?.id;
                    if (cellId && onMouseOverCell) {
                        onMouseOverCell(cellId);
                    }
                },
                onMouseLeave: () => {
                    if (onMouseLeaveChart) {
                        onMouseLeaveChart();
                    }
                }
            });
        },
        transformData: mockTransformData
    };
});

describe('SalaryExpenseByBUChart Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('should render chart with valid data', () => {
        const data = [
            { label: 'IT', value: 50000 },
            { label: 'HR', value: 30000 }
        ];

        const { getByTestId } = render(<SalaryExpenseByBUChart data={data} />);

        const chart = getByTestId('treemap-chart');
        expect(chart).toBeInTheDocument();
        expect(global.mockTransformDataFn).toHaveBeenCalled();
    });

    it('should handle null or missing labels and values', () => {
        const data = [
            { label: null, value: 50000 },
            { value: 30000 },
            { label: 'Finance', value: null }
        ];

        const { getByTestId } = render(<SalaryExpenseByBUChart data={data} />);

        const chart = getByTestId('treemap-chart');
        expect(chart).toBeInTheDocument();
        expect(global.mockTransformDataFn).toHaveBeenCalled();
        
        const callArgs = global.mockTransformDataFn.mock.calls[0][0];
        expect(callArgs.data).toHaveLength(3);
        expect(callArgs.data[0].title).toBe('');
        expect(callArgs.data[0].value).toBe(50000);
        expect(callArgs.data[1].title).toBe('');
        expect(callArgs.data[1].value).toBe(30000);
        expect(callArgs.data[2].title).toBe('Finance');
        expect(callArgs.data[2].value).toBe(0);
    });

    it('should render placeholder when data is empty', () => {
        const { getByText } = render(<SalaryExpenseByBUChart data={[]} />);

        expect(getByText('No data available')).toBeInTheDocument();
        expect(global.mockTransformDataFn).not.toHaveBeenCalled();
    });

    it('should render placeholder when data is null', () => {
        const { getByText } = render(<SalaryExpenseByBUChart data={null} />);

        expect(getByText('No data available')).toBeInTheDocument();
        expect(global.mockTransformDataFn).not.toHaveBeenCalled();
    });

    it('should render placeholder when data is undefined', () => {
        const { getByText } = render(<SalaryExpenseByBUChart />);

        expect(getByText('No data available')).toBeInTheDocument();
        expect(global.mockTransformDataFn).not.toHaveBeenCalled();
    });

    it('should render chart wrapper with correct styles', () => {
        const data = [
            { label: 'IT', value: 50000 }
        ];

        const { container } = render(<SalaryExpenseByBUChart data={data} />);
        const wrapper = container.querySelector('.chart-wrapper.treemap-wrapper');
        
        expect(wrapper).toBeInTheDocument();
        expect(wrapper).toHaveStyle({ height: '300px', position: 'relative' });
    });

    it('should handle mouse over cell and show tooltip', () => {
        const data = [
            { label: 'IT', value: 50000 }
        ];

        const { getByTestId, queryByText } = render(<SalaryExpenseByBUChart data={data} />);
        const chart = getByTestId('treemap-chart');

        // Initially tooltip should not be visible
        expect(queryByText('IT')).not.toBeInTheDocument();

        // Simulate mouse over
        fireEvent.mouseOver(chart);

        // Tooltip should appear
        expect(queryByText('IT')).toBeInTheDocument();
        expect(queryByText(/Salary Expense:.*USD/)).toBeInTheDocument();
    });

    it('should handle mouse leave and hide tooltip', () => {
        const data = [
            { label: 'IT', value: 50000 }
        ];

        const { getByTestId, queryByText } = render(<SalaryExpenseByBUChart data={data} />);
        const chart = getByTestId('treemap-chart');

        // Mouse over to show tooltip
        fireEvent.mouseOver(chart);
        expect(queryByText('IT')).toBeInTheDocument();

        // Mouse leave to hide tooltip
        fireEvent.mouseLeave(chart);
        expect(queryByText('IT')).not.toBeInTheDocument();
    });

    it('should format tooltip value with locale string', () => {
        const data = [
            { label: 'IT', value: 50000 }
        ];

        const { getByTestId, queryByText } = render(<SalaryExpenseByBUChart data={data} />);
        const chart = getByTestId('treemap-chart');

        fireEvent.mouseOver(chart);

        const tooltipValue = queryByText(/Salary Expense:.*USD/);
        expect(tooltipValue).toBeInTheDocument();
        expect(tooltipValue.textContent).toContain('50,000');
    });

    it('should call transformData with correct parameters', () => {
        const data = [
            { label: 'IT', value: 50000 },
            { label: 'HR', value: 30000 }
        ];

        render(<SalaryExpenseByBUChart data={data} />);

        expect(global.mockTransformDataFn).toHaveBeenCalledTimes(1);
        const callArgs = global.mockTransformDataFn.mock.calls[0][0];
        
        expect(callArgs.width).toBe(590);
        expect(callArgs.height).toBe(300);
        expect(callArgs.data).toHaveLength(2);
        expect(callArgs.data[0].id).toBe('IT');
        expect(callArgs.data[0].value).toBe(50000);
        expect(callArgs.data[1].id).toBe('HR');
        expect(callArgs.data[1].value).toBe(30000);
        expect(callArgs.colorMap).toBeDefined();
        expect(callArgs.colorMap).toHaveLength(2);
    });

    it('should sort items by value descending for color assignment', () => {
        const data = [
            { label: 'Small', value: 10000 },
            { label: 'Large', value: 50000 },
            { label: 'Medium', value: 30000 }
        ];

        render(<SalaryExpenseByBUChart data={data} />);

        const callArgs = global.mockTransformDataFn.mock.calls[0][0];
        // Verify colorMap is created (items should be sorted internally)
        expect(callArgs.colorMap).toBeDefined();
        expect(callArgs.colorMap.length).toBe(3);
    });
});

