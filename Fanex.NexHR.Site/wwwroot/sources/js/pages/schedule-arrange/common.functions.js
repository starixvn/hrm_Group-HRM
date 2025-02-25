import moment from 'moment';
import { LeaveType, ChangeShiftType } from '../../shared/constants';

function getColumnShiftInfo(dailySchedule, isHost, defaultShift, isUsedWeeklyDefaultShift) {
    let shift = {
        class: '',
        value: '',
        classOfLeave: '',
        leaveCode: '',
        placeholder: '',
        details: '',
        isDisabled: !isHost || dailySchedule.isDisabled,
        isShowCheckBox: isHost && !dailySchedule.isDisabled
    };
    const moveOffDate = dailySchedule.moveOffFromDate !== null
        ? moment(dailySchedule.moveOffFromDate).format('DD MMM, YYYY') : moment(dailySchedule.moveOffToDate).format('DD MMM, YYYY');

    if (dailySchedule.isLeaveOffDay) {
        shift.value = 'Off';
        shift.class = 'leave offday';
    } else if (dailySchedule.isLeaveDay) {
        if (dailySchedule.isHoliday) {
            shift.value = 'PH';
            shift.class = 'holiday';
        } else if (dailySchedule.isOffDay) {
            shift.value = 'Off';
            shift.class = 'offday';
        } else {
            if (dailySchedule.leaveType !== null && (dailySchedule.leaveType2 === null || dailySchedule.leaveType.code === dailySchedule.leaveType2.code)) {
                switch (dailySchedule.leaveType.id) {
                    case LeaveType.AnnualLeave:
                        shift.classOfLeave = 'annualleave';
                        break;
                    case LeaveType.UnpaidLeave:
                        shift.classOfLeave = 'unpaidleave';
                        break;
                    case LeaveType.BusinessTrip:
                        shift.classOfLeave = 'businesstrip';
                        break;
                    case LeaveType.WeddingLeave:
                        shift.classOfLeave = 'weddingleave';
                        break;
                    default:
                        shift.classOfLeave = 'leaveday';
                        break;
                }
            }

            if (dailySchedule.isLeaveHalfDay) {
                const fromTime = moment(dailySchedule.leaveType.fromTime).format('DD MMM, YYYY HH:mm');
                const toTime = moment(dailySchedule.leaveType.toTime).format('DD MMM, YYYY HH:mm');
                shift.leaveCode = dailySchedule.leaveType.code + ' ½';
                shift.details = `From: ${fromTime}\r\nTo: ${toTime}`;

                if (dailySchedule.shift !== null) {
                    shift.value = dailySchedule.shift.code;
                }

                if (dailySchedule.isChangeShift) {
                    shift.class = 'changeshift';
                }
            } else {
                shift.value = dailySchedule.leaveType.code;
                shift.class = shift.classOfLeave;

                if (dailySchedule.leaveType2 !== null && dailySchedule.leaveType.code !== dailySchedule.leaveType2.code) {
                    shift.class = 'leaveday leavehalfday2';
                    shift.value = dailySchedule.leaveType.code + ' / ' + dailySchedule.leaveType2.code;
                }
            }
        }
    } else if (dailySchedule.isHoliday) {
        shift.class = 'holiday';
        if (dailySchedule.isOffDay) {
            shift.value = 'Off';
        } else {
            shift.value = dailySchedule.shift !== null ? dailySchedule.shift.code : 'PH';
        }

        if (dailySchedule.isChangeShift) {
            shift.details = `Change shift${dailySchedule.changeShiftTypeId === ChangeShiftType.MoveOffDay ? ` - Move off day (${moveOffDate})` : ''}`;
        } else if (dailySchedule.isOvertime) {
            shift.details = 'Overtime';
        } else if (dailySchedule.isKeepInView) {
            shift.details = 'CL';
        }
    } else if (dailySchedule.isOffDay) {
        shift.class = 'offday';
        shift.value = dailySchedule.shift !== null ? dailySchedule.shift.code : 'Off';
        if (dailySchedule.isFixedOffDay) {
            shift.details = 'Off Day OT Fixed';
        } else if (dailySchedule.isChangeShift) {
            shift.details = `Change shift${dailySchedule.changeShiftTypeId === ChangeShiftType.MoveOffDay ? ` - Move off day (${moveOffDate})` : ''}`;
        } else if (dailySchedule.isOvertime) {
            shift.details = 'Overtime';
        } else if (dailySchedule.isKeepInView) {
            shift.details = 'CL';
        }
    } else if (dailySchedule.isKeepInView) {
        shift.class = 'keepinviewday';
        shift.value = dailySchedule.shift !== null ? dailySchedule.shift.code : '';
    } else if (dailySchedule.isChangeShift) {
        shift.class = 'changeshift';
        shift.value = dailySchedule.shift !== null ? dailySchedule.shift.code : '';
        shift.details = dailySchedule.changeShiftTypeId === ChangeShiftType.MoveOffDay ? `Move off day to ${moveOffDate}` : '';
    } else if (dailySchedule.isOvertime) {
        shift.class = 'overtime';
        shift.value = dailySchedule.shift !== null ? dailySchedule.shift.code : '';
    } else {
        shift.value = dailySchedule.shift !== null ? dailySchedule.shift.code : '';
    }

    if (dailySchedule.shift !== null
        && defaultShift !== null
        && shift.value === dailySchedule.shift.code
        && shift.value === defaultShift.code
        && !isUsedWeeklyDefaultShift) {
        shift.placeholder = shift.value;
        shift.class += ' same-default-shift';
    }

    if (dailySchedule.isOffDayReplacement) {
        shift.class += ' offdayreplacement';
    }

    if (!dailySchedule.isModified && dailySchedule.shift !== null) {
        shift.class += ' is-default-shift';
    }

    return shift;
}

function getTotalOffDays(dailySchedule) {
    const attrNames = ['mondaySchedule', 'tuesdaySchedule', 'wednesdaySchedule',
        'thursdaySchedule', 'fridaySchedule', 'saturdaySchedule', 'sundaySchedule'];

    let totalOffDays = 0;
    attrNames.forEach(attrName => {
        const isMoveOffDay = dailySchedule[attrName].changeShiftTypeId === ChangeShiftType.MoveOffDay;
        if (!dailySchedule[attrName].isHoliday
            && !(isMoveOffDay && dailySchedule[attrName].moveOffFromDate !== null)
            && (dailySchedule[attrName].isOffDay
                || dailySchedule[attrName].isLeaveOffDay
                || (isMoveOffDay && dailySchedule[attrName].moveOffToDate !== null))) {
            totalOffDays++;
        }
    });

    return totalOffDays;
}

export {
    getColumnShiftInfo,
    getTotalOffDays
};