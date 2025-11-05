import React from 'react';
import TableWrapper from './shared/TableWrapper';
import TableBody from './shared/TableBody';
import { renderFixedHeaders, renderGroupedHeaders } from './shared/TableHeaders';
import { 
    processColumns, 
    groupByLabel1, 
    groupByLabel2,
} from '../utils/tableUtils';

const HeadcountTableByCompany = ({ data }) => {
    const columns = processColumns(data);

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
            />
        </TableWrapper>
    );
};

export default HeadcountTableByCompany;

