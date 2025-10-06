import React from 'react';
import { CommonTable } from '../../shared/teamManagerHelpers';
import THead from './THead';
import Row from './Row';

export default function Table(props) {
    return (
        <CommonTable
            {...props}
            THeadComponent={THead}
            RowComponent={Row}
            colSpan={9}
        />
    );
}