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

const HeadcountByBUChart = ({ data }) => {
    if (!data || !Array.isArray(data) || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    const chartData = {
        labels: data.map(item => item.label),
        datasets: [
            {
                label: 'Headcount',
                data: data.map(item => item.value),
                backgroundColor: '#42A5F5',
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
                        return `Headcount: ${context.parsed.y.toLocaleString()}`;
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
                    callback: function(value, _) {
                        const label = this.getLabelForValue(value);
                        return label;
                    }
                },
                grid: {
                    display: false,
                }
            }
        },
    };

    return (
        <div className="chart-wrapper" style={{ height: '300px', overflowY: 'auto' }}>
            <Bar data={chartData} options={options} />
        </div>
    );
};

export default HeadcountByBUChart;

