import React from 'react';
import {
    Chart as ChartJS,
    ArcElement,
    Tooltip,
    Legend,
} from 'chart.js';
import { Pie } from 'react-chartjs-2';
import ChartDataLabels from 'chartjs-plugin-datalabels';

ChartJS.register(ArcElement, Tooltip, Legend, ChartDataLabels);

const BUCategoryBreakdown = ({ data }) => {
    if (!data || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    const colors = [
       'rgba(201, 203, 207, 0.8)',
        'rgba(54, 162, 235, 0.8)',
        'rgba(255, 99, 132, 0.8)',
        'rgba(75, 192, 192, 0.8)',
        'rgba(153, 102, 255, 0.8)',
        'rgba(255, 159, 64, 0.8)',
        'rgba(199, 199, 199, 0.8)',
        'rgba(83, 102, 255, 0.8)',
    ];

    const employeesChartData = {
        labels: data.map(item => item.category || item.label1 || ''),
        datasets: [
            {
                label: 'Employees',
                data: data.map(item => item.employeeCount || 0),
                backgroundColor: colors.slice(0, data.length),
                borderWidth: 1,
            },
        ],
    };

    const buChartData = {
        labels: data.map(item => item.category || item.label1 || ''),
        datasets: [
            {
                label: 'Business Units',
                data: data.map(item => item.buCount || 0),
                backgroundColor: colors.slice(0, data.length),
                borderWidth: 1,
            },
        ],
    };

    const chartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        layout: {
            padding: {
                left: 10,
                right: 10,
                top: 10,
                bottom: 10
            }
        },
        plugins: {
            legend: {
                display: false
            },
            datalabels: {
                display: true,
                color: '#ffffff',
                font: {
                    size: 11,
                    weight: 'bold'
                },
                formatter: function(value, context) {
                    const total = context.dataset.data.reduce((a, b) => a + b, 0);
                    const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                    return percentage > 0 ? `${percentage}%` : '';
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
                        const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                        return `${label}: ${value.toLocaleString()} (${percentage}%)`;
                    }
                }
            }
        },
    };

    return (
        <div className="bu-category-breakdown" style={{ height: '300px', overflowY: 'auto' }}>
            <div className="bu-category-charts">
                <div className="chart-item">
                    <h4>Employees</h4>
                    <div className="chart-wrapper" style={{ height: '200px', width: '200px' }}>
                        <Pie data={employeesChartData} options={chartOptions} />
                    </div>
                </div>
                <div className="chart-item">
                    <h4>Business Units</h4>
                    <div className="chart-wrapper" style={{ height: '200px', width: '200px' }}>
                        <Pie data={buChartData} options={chartOptions} />
                    </div>
                </div>
            </div>
            <div className="bu-category-legend">
                {data.map((item, index) => (
                    <div key={index} className="legend-item">
                        <span 
                            className="legend-color" 
                            style={{ backgroundColor: colors[index % colors.length] }}
                        ></span>
                        <span className="legend-label">{item.category || item.label1 || ''}</span>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default BUCategoryBreakdown;

