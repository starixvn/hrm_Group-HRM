import React from 'react';
import { CommonTable } from '../../shared/teamManagerHelpers';
import THead from './THead';
import Row from './Row';

const Table = props => (
    <CommonTable
        {...props}
        THeadComponent={THead}
        RowComponent={Row}
    />
);

export default Table;
