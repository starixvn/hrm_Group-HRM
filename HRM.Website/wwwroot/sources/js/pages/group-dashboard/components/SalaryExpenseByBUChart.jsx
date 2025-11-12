import React, { useState, useRef } from 'react';
import TreeMap, { transformData } from 'react-canvas-treemap';

const SalaryExpenseByBUChart = ({ data }) => {
    const [hoveredCell, setHoveredCell] = useState(null);
    const [tooltipPosition, setTooltipPosition] = useState({ x: 0, y: 0 });
    const [containerWidth] = useState(590);
    const chartWrapperRef = useRef(null);
    const chartHeight = 300;

    if (!data || data.length === 0) {
        return <div className="chart-placeholder">No data available</div>;
    }

    const treemapInput = data.map((item, index) => ({
        id: item.label || `item-${index}`,
        value: item.value || 0,
        title: item.label || '',
        topLevelParentId: item.label || `item-${index}`,
    }));

    const generateBlueGradientColors = (count = 30) => {
        const colors = [];
        
        const darkBlue = { r: 70, g: 130, b: 200 };   
        const lightBlue = { r: 180, g: 210, b: 250 }; 
        
        for (let i = 0; i < count; i++) {
            const ratio = i / (count - 1);

            const adjustedRatio = Math.pow(ratio, 0.5);            
            const variation = Math.sin(i / count * Math.PI * 2) * 8;
            
            const r = Math.round(darkBlue.r + (lightBlue.r - darkBlue.r) * adjustedRatio + variation);
            const g = Math.round(darkBlue.g + (lightBlue.g - darkBlue.g) * adjustedRatio + variation * 0.7);
            const b = Math.round(darkBlue.b + (lightBlue.b - darkBlue.b) * adjustedRatio + variation * 0.5);
            
            const clampedR = Math.max(0, Math.min(255, r));
            const clampedG = Math.max(0, Math.min(255, g));
            const clampedB = Math.max(0, Math.min(255, b));
            
            const color = `rgba(${clampedR}, ${clampedG}, ${clampedB}, 0.9)`;
            colors.push(color);
        }
        
        return colors;
    };

    const blueColors = generateBlueGradientColors(30);

    const sortedItems = [...treemapInput].sort((a, b) => b.value - a.value);
    
    const colorByItemId = {};
    sortedItems.forEach((item, index) => {
        const colorIndex = Math.min(index, blueColors.length - 1);
        colorByItemId[item.id] = blueColors[colorIndex];
    });

    const colorMap = treemapInput.map(item => ({
        id: item.id,
        color: colorByItemId[item.id] || blueColors[0]
    }));

    const transformedData = transformData({
        data: treemapInput,
        width: containerWidth,
        height: chartHeight,
        colorMap: colorMap,
    });

    const customizedCells = transformedData.treeMapCells.map(cell => {
        const cellData = treemapInput.find(item => item.id === cell.id);
        if (cellData) {
            const cellWidth = cell.x1 - cell.x0;
            const cellHeight = cell.y1 - cell.y0;
            const cellArea = cellWidth * cellHeight;
            
            const minWidth = 10;
            const minHeight = 10;  
            const minArea = 100;   
            const isTooSmall = cellWidth < minWidth || cellHeight < minHeight || cellArea < minArea;
            const buName = cellData.title || cellData.id;
            
            if (!isTooSmall) {
                const baseFontSize = 10;
                const minFontSize = 9;
                const maxFontSize = 13;
                const areaRatio = Math.min(cellArea / 3000, 1.3); // Scale based on area
                const fontSize = Math.max(minFontSize, Math.min(Math.round(baseFontSize * areaRatio), maxFontSize));
                
                return {
                    ...cell,
                    textLayout: {
                        type: 'ShowBoth',
                        label: {
                            showText: true,
                            fontSize: fontSize,
                            useMargin: true, 
                            textSplitIntoLines: [buName], 
                            textUnsplit: buName
                        },
                        share: {
                            showText: false
                        }
                    }
                };
            } 
        }
        return cell;
    });

    const handleMouseOverCell = id => {
        const cell = treemapInput.find(item => item.id === id);
        if (cell) {
            setHoveredCell({
                id: cell.id,
                title: cell.title,
                value: cell.value
            });
        }
    };

    const handleMouseLeaveChart = () => {
        setHoveredCell(null);
    };

    const handleMouseMove = event => {
        setTooltipPosition({
            x: event.clientX || event.pageX || 0,
            y: event.clientY || event.pageY || 0
        });
    };

    return (
        <div 
            className="chart-wrapper treemap-wrapper" 
            ref={chartWrapperRef} 
            style={{ height: '300px', position: 'relative' }}
            onMouseMove={handleMouseMove}
        >
            <TreeMap
                highlighted={undefined}
                cells={customizedCells}
                numCellsTier={0}
                chartContainerWidth={containerWidth}
                chartContainerHeight={chartHeight}
                onMouseOverCell={handleMouseOverCell}
                onMouseLeaveChart={handleMouseLeaveChart}
            />
            {hoveredCell && (
                <div 
                    className="treemap-tooltip"
                    style={{
                        position: 'fixed',
                        left: `${tooltipPosition.x - 200}px`,
                        top: `${tooltipPosition.y - 50}px`,
                        zIndex: 1000,
                        pointerEvents: 'none'
                    }}
                >
                    <div className="tooltip-content">
                        <div className="tooltip-title">{hoveredCell.title}</div>
                        <div className="tooltip-value">
                            Salary Expense: {hoveredCell.value.toLocaleString()} USD
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default SalaryExpenseByBUChart;

