import React from 'react';

const CurrencyRoundingTable = ({ data }) => {
    return (
        <div className="currency-rounding-table-section">
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>Currency</th>
                        <th>Rounding</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {data && data.length > 0 ? (
                        data.map((item, index) => (
                            <tr key={index}>
                                <td className="align-left"><strong>{item.currency || item.Currency}</strong></td>
                                <td>{item.rounding !== undefined ? item.rounding : (item.Rounding !== undefined ? item.Rounding : '')}</td>
                                <td className="align-left">{item.description || item.Description || ''}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="3" style={{ textAlign: 'center', padding: '20px' }}>
                                No data available
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default CurrencyRoundingTable;

