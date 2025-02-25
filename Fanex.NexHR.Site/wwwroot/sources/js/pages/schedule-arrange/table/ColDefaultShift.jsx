import React, { PureComponent } from 'react';

import ShiftAutocomplete from '../ShiftAutocomplete';

export default class ColDefaultShift extends PureComponent {
    handleChangeDefaultShift = (name, shift) => {
        const { teamId, employeeId } = this.props;
        this.props.onChangeDefaultShift(teamId, employeeId, shift);
    };

    render() {
        const { isHost, shiftOptions, data, isUsedWeeklyDefaultShift } = this.props;

        return (
            <div className={`table-cell cell-shift cell-default-shift ${isUsedWeeklyDefaultShift ? 'is-weekly' : ''}`}>
                {
                    !isUsedWeeklyDefaultShift &&
                    <ShiftAutocomplete isDisabled={!isHost}
                        shift={data} shiftOptions={shiftOptions}
                        onChange={this.handleChangeDefaultShift}
                    />
                }
            </div>
        );
    }
}