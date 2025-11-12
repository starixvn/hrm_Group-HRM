import React from 'react';
import {
    Chart as ChartJS,
    ArcElement,
    Tooltip,
    Legend,
} from 'chart.js';
import { Pie } from 'react-chartjs-2';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { PASTEL_COLORS } from '../constants/chartColors';

ChartJS.register(ArcElement, Tooltip, Legend, ChartDataLabels);

const BUCategoryBreakdown = ({ data }) => {
    if (!data || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    const employeesChartData = {
        labels: data.map(item => item.category || item.label1 || ''),
        datasets: [
            {
                label: 'Employees',
                data: data.map(item => item.employeeCount || 0),
                backgroundColor: PASTEL_COLORS.slice(0, data.length),
                borderWidth: 3,
                borderColor: '#ffffff',
                borderRadius: 5,
            },
        ],
    };

    const buChartData = {
        labels: data.map(item => item.category || item.label1 || ''),
        datasets: [
            {
                label: 'Business Units',
                data: data.map(item => item.buCount || 0),
                backgroundColor: PASTEL_COLORS.slice(0, data.length),
                borderWidth: 3,
                borderColor: '#ffffff',
                borderRadius: 5,
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
                },
                formatter: function(value, context) {
                    const total = context.dataset.data.reduce((a, b) => a + b, 0);
                    const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                    return percentage > 6 ? `${percentage}%` : '';
                },
                anchor: 'center',
                align: 'end',
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
                            style={{ backgroundColor: PASTEL_COLORS[index % PASTEL_COLORS.length] }}
                        ></span>
                        <span className="legend-label">{item.category || item.label1 || ''}</span>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default BUCategoryBreakdown;

