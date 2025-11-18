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
                        data.map((item, index) => {
                            let rounding = '';
                            if (item.rounding !== undefined) {
                                rounding = item.rounding;
                            }
                            return (
                                <tr key={index}>
                                    <td className="align-left"><strong>{item.currency}</strong></td>
                                    <td>{rounding}</td>
                                    <td className="align-left">{item.description || ''}</td>
                                </tr>
                            );
                        })
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

