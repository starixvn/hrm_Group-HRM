import React from 'react';
import {
    Chart as ChartJS,
    ArcElement,
    Tooltip,
    Legend,
} from 'chart.js';
import { Doughnut } from 'react-chartjs-2';
import ChartDataLabels from 'chartjs-plugin-datalabels';

ChartJS.register(ArcElement, Tooltip, Legend, ChartDataLabels);

const SalaryExpenseByCountryByBU = ({ data }) => {
    if (!data || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    const colors = [
        'rgba(201, 203, 207, 0.8)', // light grey
        'rgba(153, 102, 255, 0.8)', // light purple
        'rgba(75, 192, 192, 0.8)', // teal
        'rgba(0, 0, 0, 0.8)', // black
    ];

    return (
        <div className="salary-expense-by-country-bu" style={{ height: '300px', overflowY: 'auto' }}>
            {data.map((countryData, index) => {
                const categories = countryData.categories || [];
                const chartData = {
                    labels: categories.map(item => item.category || item.label1 || ''),
                    datasets: [
                        {
                            label: 'Salary Expense',
                            data: categories.map(item => item.salaryExpense || item.amount || 0),
                            backgroundColor: colors,
                            borderWidth: 1,
                        },
                    ],
                };

                const options = {
                    responsive: true,
                    maintainAspectRatio: true,
                    layout: {
                        padding: {
                            left: 15,
                            right: 15,
                            top: 15,
                            bottom: 15
                        }
                    },
                    plugins: {
                        legend: {
                            display: false,
                        },
                        datalabels: {
                            display: true,
                            color: '#2c3e50',
                            font: {
                                size: 10,
                                weight: 'bold'
                            },
                            formatter: function(value, context) {
                                const label = context.chart.data.labels[context.dataIndex];
                                return label;
                            },
                            anchor: 'end',
                            align: 'end',
                            offset: 5,
                            clip: false
                        },
                        tooltip: {
                            callbacks: {
                                label: function(context) {
                                    const label = context.label || '';
                                    const value = context.parsed || 0;
                                    const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                    const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                                    return `${label}: ${value.toLocaleString()} (${percentage}%)`;
                                }
                            }
                        }
                    },
                };

                return (
                    <div key={index} className="country-chart-item" style={{ marginBottom: '20px' }}>
                        <h4>{countryData.countryName || countryData.country || `Country ${index + 1}`}</h4>
                        <div style={{ height: '200px', maxWidth: '280px', margin: '0 auto', padding: '10px' }}>
                            <Doughnut data={chartData} options={options} />
                        </div>
                    </div>
                );
            })}
        </div>
    );
};

export default SalaryExpenseByCountryByBU;

