import React, { PureComponent } from 'react';

import ShiftAutocomplete from '../../ShiftAutocomplete';

export default class ColDefaultShift extends PureComponent {
    render() {
        const { data, isUsedWeeklyDefaultShift } = this.props;

        return (
            <div className={`table-cell cell-shift cell-default-shift ${isUsedWeeklyDefaultShift ? 'is-weekly' : ''}`}>
                {
                    !isUsedWeeklyDefaultShift &&
                    <ShiftAutocomplete isDisabled shift={data} />
                }
            </div>
        );
    }
}