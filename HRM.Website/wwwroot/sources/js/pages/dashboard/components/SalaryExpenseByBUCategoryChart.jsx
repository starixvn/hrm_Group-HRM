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
        labels: data.map(item => item.label),
        datasets: [
            {
                label: 'Salary Expense',
                data: data.map(item => item.value),
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
                display: true,
                position: 'right',
            },
            datalabels: {
                display: true,
                color: '#ffffff',
                font: {
                    size: 12,
                    weight: 'bold'
                },
                formatter: function(value, context) {
                    const total = context.dataset.data.reduce((a, b) => a + b, 0);
                    const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                    return `${percentage}%`;
                },
                anchor: 'center',
                align: 'center',
                offset: 0,
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
        <div className="chart-wrapper" style={{ height: '300px'}}>
            <Doughnut data={chartData} options={options} />
        </div>
    );
};

export default SalaryExpenseByBUCategoryChart;

