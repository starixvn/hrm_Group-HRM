import React from 'react';

// Helper to render Month and Category header cells with rowspan
export const renderFixedHeaders = (rowspan = null) => {
    return (
        <>
            <th 
                className="header-cell month-header" 
                rowSpan={rowspan || undefined}
            >
                Month
            </th>
            <th 
                className="header-cell category-header" 
                rowSpan={rowspan || undefined}
            >
                Category
            </th>
        </>
    );
};

// Helper to render grouped header cells with colspan
export const renderGroupedHeaders = (groups, rowKey = 1) => 
    groups.map((cell, idx) => (
        <th 
            key={`row${rowKey}-${idx}`}
            colSpan={cell.colspan}
            className={`header-cell ${cell.isGrandTotal ? 'grand-total-header' : ''} ${cell.isUnitCategory ? 'unit-category-header' : ''}`}
            rowSpan={cell.isGrandTotal ? rowKey + 1 : undefined}
        >
            {cell.label}
        </th>
    ));

