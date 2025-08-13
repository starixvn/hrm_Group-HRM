import React from 'react';

import ShiftAutocomplete from '../ShiftAutocomplete';

const ColDefaultShift = ({ teamId, employeeId, isHost, shiftOptions, data, isUsedWeeklyDefaultShift, onChangeDefaultShift }) => {
    const handleChangeDefaultShift = (_, shift) => {
        onChangeDefaultShift(teamId, employeeId, shift);
    };

    return (
        <div className={`table-cell cell-shift cell-default-shift ${isUsedWeeklyDefaultShift ? 'is-weekly' : ''}`}>
            {
                !isUsedWeeklyDefaultShift &&
                <ShiftAutocomplete isDisabled={!isHost}
                    shift={data} shiftOptions={shiftOptions}
                    onChange={handleChangeDefaultShift}
                />
            }
        </div>
    );
};

export default ColDefaultShift;