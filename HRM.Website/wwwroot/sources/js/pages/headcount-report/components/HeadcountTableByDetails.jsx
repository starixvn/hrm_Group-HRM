import React from 'react';
import TableWrapper from './shared/TableWrapper';
import TableBody from './shared/TableBody';
import { renderFixedHeaders, renderGroupedHeaders } from './shared/TableHeaders';
import { 
    processColumns, 
    groupByLabel1, 
    groupByLabel3, 
    groupByLabel2,
} from '../utils/tableUtils';

const HeadcountTableByDetails = ({ data }) => {
    const columns = processColumns(data);

    return (
        <TableWrapper>
            <thead>
                <tr>
                    {renderFixedHeaders(3)}
                    {renderGroupedHeaders(groupByLabel1(columns), 1)}
                </tr>                
                <tr>
                    {renderGroupedHeaders(groupByLabel2(columns), 2)}
                </tr>   
                <tr>
                    {renderGroupedHeaders(groupByLabel3(columns), 3)}
                </tr>
            </thead>
            <TableBody 
                data={data} 
                columns={columns} 
            />
        </TableWrapper>
    );
};

export default HeadcountTableByDetails;

