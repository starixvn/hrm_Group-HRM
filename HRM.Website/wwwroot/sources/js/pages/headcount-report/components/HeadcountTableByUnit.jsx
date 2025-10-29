import React from 'react';
import TableWrapper from './shared/TableWrapper';
import TableBody from './shared/TableBody';
import { renderFixedHeaders, renderGroupedHeaders } from './shared/TableHeaders';
import { processColumns, getMaxMonth, groupByLabel1 } from '../utils/tableUtils';

const HeadcountTableByUnit = ({ data }) => {
    if (!data || data.length === 0) {
        return null;
    }

    const columns = processColumns(data);
    const maxMonth = getMaxMonth(data);

    return (
        <TableWrapper>
            <thead>
                <tr>
                    {renderFixedHeaders()}
                    {renderGroupedHeaders(groupByLabel1(columns), 1)}
                </tr>
            </thead>
            <TableBody 
                data={data} 
                columns={columns} 
                maxMonth={maxMonth} 
            />
        </TableWrapper>
    );
};

export default HeadcountTableByUnit;

