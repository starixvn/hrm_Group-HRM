import React from 'react';

const TableWrapper = ({ children }) => {
    return (
        <div className="headcount-table-wrapper">
            <div className="headcount-table-container">
                <table className="table table-custom headcount-table">
                    {children}
                </table>
            </div>
        </div>
    );
};

export default TableWrapper;

