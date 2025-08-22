import axios from 'axios';
import { cloneDeep, isEqual, unset } from 'lodash';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { axiosError, blockUI, resolveClientUrl, unblockUI } from '../../shared/common.functions';

const FORM_URLENCODED_HEADER = 'application/x-www-form-urlencoded';
const SUCCESS_MESSAGE = 'Updated Successfully!';

const processScheduleMember = (schedule, originalSchedule, weeklySchedules, defaultShifts, component) => {
    unset(schedule, 'selectWeekDays');
    unset(originalSchedule, 'selectWeekDays');

    if (!isEqual(schedule, originalSchedule)) {
        const totalOffDaysDefault = schedule.offDaysbyWeek + schedule.totalOffDayReplacement;
        
        if (!component.validateScheduleChanges(schedule, totalOffDaysDefault)) {
            toast.error('Some employees have a larger number of days off during the week according to the setting.');
            return false;
        }

        const defaultShift = component.collectDefaultShiftChanges(schedule, originalSchedule);
        if (defaultShift) {
            defaultShifts.push(defaultShift);
        }

        component.scheduleAttrNames.forEach(attrName => {
            const weeklySchedule = component.collectWeeklyScheduleChanges(schedule, originalSchedule, attrName);
            if (weeklySchedule) {
                weeklySchedules.push(weeklySchedule);
            }
        });
    }
    return true;
};

const processScheduleTeam = (members, originalMembers, weeklySchedules, defaultShifts, component) => {
    for (let eindex = 0; eindex < members.length; eindex++) {
        const schedule = members[eindex];
        const originalSchedule = originalMembers[eindex];
        
        const success = processScheduleMember(schedule, originalSchedule, weeklySchedules, defaultShifts, component);
        if (!success) {
            return false;
        }
    }
    return true;
};

export const createScheduleSaveHandlers = component => ({
    collectDefaultShiftChanges: (schedule, originalSchedule) => {
        if (!isEqual(originalSchedule.defaultShift, schedule.defaultShift)) {
            return {
                employeeId: schedule.employeeId,
                fromDate: schedule.mondaySchedule.workDate,
                shiftId: schedule.defaultShift !== null ? schedule.defaultShift.id : null,
            };
        }
        return null;
    },

    collectWeeklyScheduleChanges: (schedule, originalSchedule, attrName) => {
        if (!isEqual(originalSchedule[attrName], schedule[attrName]) || 
            originalSchedule.isPublic !== schedule.isPublic) {
            return {
                employeeId: schedule.employeeId,
                workDate: schedule[attrName].workDate,
                shiftId: schedule[attrName].shift !== null ? schedule[attrName].shift.id : null,
                isHoliday: schedule[attrName].isHoliday,
                isOffDay: schedule[attrName].isOffDay,
                isFixedOffDay: schedule[attrName].isFixedOffDay,
                isPublic: schedule.isPublic,
            };
        }
        return null;
    },

    saveScheduleChanges: (weeklySchedules, defaultShifts, fromDate, toDate) => {
        if (weeklySchedules.length > 0 || defaultShifts.length > 0) {
            blockUI();
            axios({
                url: resolveClientUrl('ScheduleArrange/EditWeekly'),
                method: 'POST',
                data: stringify({
                    fromDate,
                    toDate,
                    weeklySchedules: weeklySchedules.length > 0 ? weeklySchedules : null,
                    defaultShifts,
                }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': FORM_URLENCODED_HEADER,
                },
            })
                .then(response => {
                    const data = response.data;
                    if (data.isSuccessful) {
                        toast.success(SUCCESS_MESSAGE);
                        component.getWeeklyShedules(component.state.filter);
                    } else {
                        toast.error(data.message);
                    }
                })
                .catch(axiosError)
                .then(() => unblockUI());
        }
    },

    handleSave: () => {
        const weeklySchedules = [];
        const defaultShifts = [];
        const schedules = cloneDeep(component.state.schedules);
        const originalSchedules = cloneDeep(component.originalSchedules);
        const { fromDate, toDate } = component.selectedWeek;

        for (let gindex = 0; gindex < schedules.length; gindex++) {
            const members = schedules[gindex].members;
            const originalMembers = originalSchedules[gindex].members;
            
            const success = processScheduleTeam(members, originalMembers, weeklySchedules, defaultShifts, component);
            if (!success) {
                return;
            }
        }

        component.saveScheduleChanges(weeklySchedules, defaultShifts, fromDate, toDate);
    }
});

export default createScheduleSaveHandlers;
