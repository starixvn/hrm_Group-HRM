import React from 'react';
import { resolveClientUrl } from '../../../../shared/common.functions';
import { monthNames, categories, getValue } from '../../utils/tableUtils';

const TableBody = ({ data, columns, maxMonth }) => {
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

    return (
        <tbody>
            {Array.from({ length: maxMonth }).map((_, monthIdx) => (
                <React.Fragment key={monthIdx}>
                    {categories.map((category, catIdx) => {
                        const isMonthEven = monthIdx % 2 === 0;
                        const isFirstCategory = catIdx === 0;
                        return (
                            <tr 
                                key={`${monthIdx}-${catIdx}`} 
                                className={`month-${isMonthEven ? 'even' : 'odd'}`}
                            >
                                {isFirstCategory ? (
                                    <td 
                                        className='month-label' 
                                        rowSpan={categories.length}
                                    >
                                        {monthNames[monthIdx]}
                                    </td>
                                ) : null}
                                <td className='category-label'>{category}</td>
                                {columns.map((col, colIdx) => {
                                    const { value, empIds } = getValue(data[colIdx], monthIdx, catIdx);
                                    return (
                                        <td 
                                            key={colIdx}
                                            className={col.isGrandTotal || col.isUnitCategory ? 'bold-cell' : ''}
                                        >
                                            {empIds && empIds.length > 0 ? (
                                                <a 
                                                    href={`#`}
                                                    onClick={e => handleCellClick(e, empIds)}
                                                >
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

