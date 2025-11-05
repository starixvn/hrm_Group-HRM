import React from 'react';

const BUCategoryBreakdown = ({ data }) => {
    if (!data || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    return (
        <div className="bu-category-breakdown">
            {data.map((item, index) => (
                <div key={index} className="bu-category-item">
                    <div className="bu-category-name">{item.category || item.label1 || ''}</div>
                    <div className="bu-category-details">
                        <span>{item.buCount || 0} Business Unit</span>
                        <span className="separator">,</span>
                        <span>{item.employeeCount || 0} Employees</span>
                        <span className="separator">,</span>
                        <span className="percentage">{item.percentage || 0}%</span>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default BUCategoryBreakdown;

