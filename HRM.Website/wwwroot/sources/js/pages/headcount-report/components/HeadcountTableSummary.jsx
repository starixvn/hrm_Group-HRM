import React from 'react';
import TableWrapper from './shared/TableWrapper';
import TableBody from './shared/TableBody';
import { renderFixedHeaders, renderGroupedHeaders } from './shared/TableHeaders';
import { 
    getMaxMonth, 
    groupByLabel1, 
    getUniqueColumnsByLabel1,
} from '../utils/tableUtils';

const HeadcountTableSummary = ({ data }) => {
    // Get unique columns by label1 (only one column per label1)
    const { columns: uniqueColumns, data: uniqueData } = getUniqueColumnsByLabel1(data);
    const maxMonth = getMaxMonth(data);

    return (
        <TableWrapper>
            <thead>
                <tr>
                    {renderFixedHeaders(3)}
                    {renderGroupedHeaders(groupByLabel1(uniqueColumns), 1)}
                </tr> 
            </thead>
            <TableBody 
                data={uniqueData} 
                columns={uniqueColumns} 
                maxMonth={maxMonth}
                useLabel3ForCategory={true}
            />
        </TableWrapper>
    );
};

export default HeadcountTableSummary;

