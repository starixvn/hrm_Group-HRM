import React, { PureComponent } from 'react';

import { getColumnShiftInfo } from '../../common.functions';
import ShiftAutocomplete from '../../ShiftAutocomplete';
import TooltipRequestInfo from '../../table/TooltipRequestInfo';

export default class ColShift extends PureComponent {
    render() {
        const { employeeId, data, scheduleAttrName, defaultShift, isUsedWeeklyDefaultShift } = this.props;
        const shiftInfo = getColumnShiftInfo(data, false, defaultShift, isUsedWeeklyDefaultShift);

        let shift;
        if (data.shift === null) {
            shift = { code: shiftInfo.value, name: shiftInfo.value };
        } else if (data.shift.code !== shiftInfo.value) {
            shift = { ...data.shift, code: shiftInfo.value, name: shiftInfo.value };
        } else {
            shift = data.shift;
        }

        return (
            <div className="table-cell cell-shift">
                <div className={`cell-shift-wrapper ${data.isLeaveHalfDay ? 'leavehalfday' : ''}`}>
                    {
                        shiftInfo.details !== '' &&
                        <TooltipRequestInfo id={`moreInfo-${scheduleAttrName}${employeeId}`}
                            requestInfo={shiftInfo}
                            isLeaveHalfDay={data.isLeaveHalfDay}
                        />
                    }
                    <ShiftAutocomplete shift={shift} className={shiftInfo.class} isDisabled />
                </div>
            </div>
        );
    }
}