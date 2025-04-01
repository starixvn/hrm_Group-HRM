import React, { PureComponent } from 'react';

import ColIndex from '../../table/ColIndex';
import ColNickname from './ColNickname';
import ColShift from './ColShift';

export default class Row extends PureComponent {
    render() {
        const { data } = this.props;

        return (
            <div className="table-row">
                <ColIndex data={data} />
                <ColNickname employeeId={data.employeeId} nickname={data.nickname} remark={data.remark} />
                <ColShift isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.mondaySchedule}
                    scheduleAttrName={'mondaySchedule'}
                />
                <ColShift isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.tuesdaySchedule}
                    scheduleAttrName={'tuesdaySchedule'}
                />
                <ColShift isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.wednesdaySchedule}
                    scheduleAttrName={'wednesdaySchedule'}
                />
                <ColShift isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.thursdaySchedule}
                    scheduleAttrName={'thursdaySchedule'}
                />
                <ColShift isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.fridaySchedule}
                    scheduleAttrName={'fridaySchedule'}
                />
                <ColShift isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.saturdaySchedule}
                    scheduleAttrName={'saturdaySchedule'}
                />
                <ColShift isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.sundaySchedule}
                    scheduleAttrName={'sundaySchedule'}
                />
            </div>
        );
    }
}