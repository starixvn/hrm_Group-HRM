import React from 'react';
import TableWrapper from './shared/TableWrapper';
import TableBody from './shared/TableBody';
import { renderFixedHeaders, renderGroupedHeaders } from './shared/TableHeaders';
import { 
    processColumns, 
    getMaxMonth, 
    groupByLabel1, 
    groupByLabel2,
} from '../utils/tableUtils';

const HeadcountTableByCompany = ({ data }) => {
    const columns = processColumns(data);
    const maxMonth = getMaxMonth(data);

    return (
        <TableWrapper>
            <thead>
                <tr>
                    {renderFixedHeaders(2)}
                    {renderGroupedHeaders(groupByLabel1(columns), 1)}
                </tr>
                <tr>
                    {renderGroupedHeaders(groupByLabel2(columns), 'row2')}
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

export default HeadcountTableByCompany;

