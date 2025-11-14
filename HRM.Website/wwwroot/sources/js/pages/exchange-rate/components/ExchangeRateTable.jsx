import React from 'react';

const ExchangeRateTable = ({ data }) => {
    const formatRate = (rate) => {
        return new Intl.NumberFormat('en-US', {
            minimumFractionDigits: 8,
            maximumFractionDigits: 8
        }).format(rate);
    };

    return (
        <table className="table table-custom">
            <thead>
                <tr>
                    <th>Base Currency</th>
                    <th>To USD</th>
                </tr>
            </thead>
            <tbody>
                {data && data.length > 0 ? (
                    data.map((item, index) => (
                        <tr key={index}>
                            <td className="align-left">{item.baseCurrency || item.BaseCurrency}</td>
                            <td>{formatRate(item.toUsd || item.ToUsd)}</td>
                        </tr>
                    ))
                ) : (
                    <tr>
                        <td colSpan="2">No information available</td>
                    </tr>
                )}
            </tbody>
        </table>
    );
};

export default ExchangeRateTable;

