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

const SalaryExpenseByBUCategoryChart = ({ data }) => {
    if (!data || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    const colors = [
        'rgba(75, 192, 192, 0.8)', // teal
        'rgba(0, 0, 0, 0.8)', // black
        'rgba(153, 102, 255, 0.8)', // light purple
        'rgba(201, 203, 207, 0.8)', // light grey
    ];

    const chartData = {
        labels: data.map(item => item.category || item.label1 || ''),
        datasets: [
            {
                label: 'Salary Expense',
                data: data.map(item => item.salaryExpense || item.amount || 0),
                backgroundColor: colors.slice(0, data.length),
                borderWidth: 1,
            },
        ],
    };

    const options = {
        responsive: true,
        maintainAspectRatio: false,
        layout: {
            padding: {
                left: 20,
                right: 20,
                top: 20,
                bottom: 20
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
                    size: 11,
                    weight: 'bold'
                },
                formatter: function(value, context) {
                    const label = context.chart.data.labels[context.dataIndex];
                    return label;
                },
                anchor: 'end',
                align: 'end',
                offset: 15,
                clip: false
            },
            tooltip: {
                callbacks: {
                    label: function(context) {
                        const label = context.label || '';
                        const value = context.parsed || 0;
                        const total = context.dataset.data.reduce((a, b) => a + b, 0);
                        const percentage = ((value / total) * 100).toFixed(1);
                        return `${label}: ${value.toLocaleString()} (${percentage}%)`;
                    }
                }
            }
        },
    };

    return (
        <div className="chart-wrapper" style={{ height: '250px', maxWidth: '350px', margin: '0 auto', padding: '10px' }}>
            <Doughnut data={chartData} options={options} />
        </div>
    );
};

export default SalaryExpenseByBUCategoryChart;

