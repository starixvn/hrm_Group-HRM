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
    if (!data || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    // Age ranges: 20-24, 25-29, 30-34, 35-39, 40-44, 45-49, 50-54, 55-59
    const ageRanges = data.map(item => item.ageRange || `${item.minAge || 0}-${item.maxAge || 0}`);

    const chartData = {
        labels: ageRanges,
        datasets: [
            {
                label: 'Number of Employees',
                data: data.map(item => item.count || item.employeeCount || 0),
                backgroundColor: 'rgba(75, 192, 192, 0.6)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1,
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
                        return `Employees: ${context.parsed.y.toLocaleString()}`;
                    }
                }
            }
        },
        scales: {
            y: {
                beginAtZero: true,
            },
        },
    };

    return (
        <div className="chart-wrapper" style={{ height: '300px' }}>
            <Bar data={chartData} options={options} />
        </div>
    );
};

export default EmployeeByAgeChart;

