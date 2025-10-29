import React from 'react';
import { monthNames, categories, getValue } from '../../utils/tableUtils';

const TableBody = ({ data, columns, maxMonth }) => {
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
                                {columns.map((col, colIdx) => (
                                    <td 
                                        key={colIdx}
                                        className={col.isGrandTotal || col.isUnitCategory ? 'bold-cell' : ''}
                                    >
                                        {getValue(data[colIdx], monthIdx, catIdx)}
                                    </td>
                                    )
                                )}
                            </tr>
                        );
                    })}
                </React.Fragment>
            ))}
        </tbody>
    );
};

export default TableBody;

