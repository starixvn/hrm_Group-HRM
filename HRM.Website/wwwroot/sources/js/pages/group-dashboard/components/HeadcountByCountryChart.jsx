import React from 'react';
import {
    Chart as ChartJS,
    ArcElement,
    Tooltip,
    Legend,
} from 'chart.js';
import { Doughnut } from 'react-chartjs-2';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { PASTEL_COLORS } from '../constants/chartColors';

ChartJS.register(ArcElement, Tooltip, Legend, ChartDataLabels);

const HeadcountByCountryChart = ({ data }) => {
    if (!data || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    const chartData = {
        labels: data.map(item => item.label),
        datasets: [
            {
                label: 'Headcount',
                data: data.map(item => item.value || 0),
                backgroundColor: PASTEL_COLORS.slice(0, data.length),
                borderWidth: 2,
                borderColor: '#ffffff',
                borderRadius: 5,
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
                top: 40,
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
                    const total = context.dataset.data.reduce((a, b) => a + b, 0);
                    const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                    return `${label} (${percentage}%)`;
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
                        const percentage = ((value / total) * 100).toFixed(1);
                        return `${label}: ${value.toLocaleString()} (${percentage}%)`;
                    }
                }
            }
        },
    };

    return (
        <div className="chart-wrapper" style={{ height: '300px' }}>
            <Doughnut data={chartData} options={options} />
        </div>
    );
};

export default HeadcountByCountryChart;

