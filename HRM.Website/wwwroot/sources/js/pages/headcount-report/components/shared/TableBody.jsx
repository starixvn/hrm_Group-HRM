import React from 'react';
import { resolveClientUrl } from '../../../../shared/common.functions';
import { monthNames, categories, getValue, getUniqueLabel3Values, getValueByLabel3, getMaxMonth } from '../../utils/tableUtils';

const TableBody = ({ data, columns, useLabel3ForCategory = false }) => {
    const handleCellClick = (e, empIds) => {
        if (empIds && empIds.length > 0) {
            e.preventDefault();
            const empIdsParam = empIds.join(',');
            const detailUrl = resolveClientUrl(
                `HeadcountReport/Detail?empIds=${empIdsParam}`
            );
            window.open(detailUrl, '_blank');
        }
    };
    const maxMonth = getMaxMonth(data);
    const label3Categories = useLabel3ForCategory ? getUniqueLabel3Values(data) : null;
    const displayCategories = useLabel3ForCategory ? label3Categories : categories;

    return (
        <tbody>
            {Array.from({ length: maxMonth }).map((_, monthIdx) => (
                <React.Fragment key={monthIdx}>
                    {displayCategories.map((category, catIdx) => {
                        const isMonthEven = monthIdx % 2 === 0;
                        const isFirstCategory = catIdx === 0;
                        
                        return (
                            <tr 
                                key={`${monthIdx}-${catIdx}`} 
                                className={`month-${isMonthEven ? 'even' : 'odd'}`}
                            >
                                {isFirstCategory && (
                                    <td className='month-label' rowSpan={displayCategories.length}>
                                        {monthNames[monthIdx]}
                                    </td>
                                )}
                                <td className='category-label'>{category}</td>
                                {columns.map((col, colIdx) => {
                                    let value, empIds;
                                    if (useLabel3ForCategory) {
                                        const result = getValueByLabel3(data, col, monthIdx, category);
                                        value = result.value;
                                        empIds = result.empIds;
                                    } else {
                                        const result = getValue(data[colIdx], monthIdx, catIdx);
                                        value = result.value;
                                        empIds = result.empIds;
                                    }
                                    
                                    return (
                                        <td 
                                            key={colIdx}
                                            className={col.isGrandTotal || col.isUnitCategory ? 'bold-cell' : ''}
                                        >
                                            {empIds && empIds.length > 0 ? (
                                                <a href={`#`} onClick={e => handleCellClick(e, empIds)}>
                                                    {value}
                                                </a>
                                            ) : (
                                                value
                                            )}
                                        </td>
                                    );
                                })}
                            </tr>
                        );
                    })}
                </React.Fragment>
            ))}
        </tbody>
    );
};

export default TableBody;

