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
    Legend
);

const EmployeeByAgeChart = ({ data }) => {
    if (!data || !Array.isArray(data) || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    const chartData = {
        labels: data.map(item => item.label || ''),
        datasets: [
            {
                label: 'Number of Employees',
                data: data.map(item => item.value || 0),
                backgroundColor: 'rgb(62, 174, 243)',
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
            tooltip: {
                callbacks: {
                    label: function(context) {
                        return `Employees: ${context.parsed.x.toLocaleString()}`;
                    }
                }
            }
        },
        indexAxis: 'y',
        scales: {
            x: {
                beginAtZero: true,
            },
            y: {
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

export default EmployeeByAgeChart;

