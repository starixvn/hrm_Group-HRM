import React from 'react';
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend,
} from 'chart.js';
import { Bar } from 'react-chartjs-2';

ChartJS.register(
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend,
);

const SalaryExpenseByBUChart = ({ data }) => {
    if (!data || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    const chartData = {
        labels: data.map(item => item.label || ''),
        datasets: [
            {
                label: 'Salary Expense',
                data: data.map(item => item.value || 0),
                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1,
                borderSkipped: 'bottom',
                categoryPercentage: 0.92,
                barPercentage: 0.92,
            },
        ],
    };

    const options = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                display: false,
            },
            datalabels: {
                display: false,
            },
            tooltip: {
                callbacks: {
                    label: function(context) {
                        return `Salary Expense: ${context.parsed.y.toLocaleString()}`;
                    }
                }
            }
        },
        scales: {
            y: {
                beginAtZero: true,
            },
            x: {
                ticks: {
                    autoSkip: false,
                    maxRotation: 45,
                    minRotation: 45,
                    font: {
                        size: 11
                    },
                },
                grid: {
                    display: false,
                }
            }
        },
    };

    return (
        <div className="chart-wrapper" style={{ height: '300px' }}>
            <Bar data={chartData} options={options} />
        </div>
    );
};

export default SalaryExpenseByBUChart;

