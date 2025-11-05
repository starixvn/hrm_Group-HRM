import React from 'react';
import TableWrapper from './shared/TableWrapper';
import TableBody from './shared/TableBody';
import { renderFixedHeaders, renderGroupedHeaders } from './shared/TableHeaders';
import { 
    groupByLabel1, 
    getUniqueColumnsByLabel1,
} from '../utils/tableUtils';

const HeadcountTableSummary = ({ data }) => {
    const { columns: uniqueColumns } = getUniqueColumnsByLabel1(data);

    return (
        <TableWrapper>
            <thead>
                <tr>
                    {renderFixedHeaders(3)}
                    {renderGroupedHeaders(groupByLabel1(uniqueColumns), 1)}
                </tr> 
            </thead>
            <TableBody 
                data={data} 
                columns={uniqueColumns} 
                useLabel3ForCategory={true}
            />
        </TableWrapper>
    );
};

export default HeadcountTableSummary;

