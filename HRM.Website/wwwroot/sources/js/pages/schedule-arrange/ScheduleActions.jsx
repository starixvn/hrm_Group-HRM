import axios from 'axios';
import { cloneDeep, isEqual, unset } from 'lodash';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { axiosError, blockUI, resolveClientUrl, unblockUI } from '../../shared/common.functions';
import { OffDayShiftId } from '../../shared/constants';
import { getTotalOffDays } from './common.functions';

const FORM_URLENCODED_HEADER = 'application/x-www-form-urlencoded';
const SUCCESS_MESSAGE = 'Updated Successfully!';

export const createScheduleActions = (component) => ({
    handleChangeSelectWeekDay: (teamId, employeeId, scheduleAttrName, isChecked) => {
        const schedules = [...component.state.schedules];
        const gindex = schedules.findIndex(item => item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId);
        members[eindex] = Object.assign({}, members[eindex], {
            selectWeekDays: {
                ...members[eindex].selectWeekDays,
                [scheduleAttrName]: isChecked,
            },
        });
        schedules[gindex] = { ...schedules[gindex], members };
        component.setState({ schedules });
    },

    handleChangeSelectWeekDays: (scheduleAttrName, isChecked) => {
        const schedules = [...component.state.schedules];
        const totalItems = schedules.length;

        if (scheduleAttrName !== null) {
            for (let gindex = 0; gindex < totalItems; gindex++) {
                const members = [...schedules[gindex].members];
                const totalSubItems = members.length;
                for (let subindex = 0; subindex < totalSubItems; subindex++) {
                    members[subindex] = Object.assign({}, members[subindex], {
                        selectWeekDays: {
                            ...members[subindex].selectWeekDays,
                            [scheduleAttrName]: isChecked,
                        },
                    });
                }
                schedules[gindex] = { ...schedules[gindex], members };
            }
        } else {
            for (let gindex = 0; gindex < totalItems; gindex++) {
                const members = [...schedules[gindex].members];
                const totalSubItems = members.length;
                for (let subindex = 0; subindex < totalSubItems; subindex++) {
                    members[subindex] = Object.assign({}, members[subindex], {
                        selectWeekDays: {
                            mondaySchedule: isChecked,
                            tuesdaySchedule: isChecked,
                            wednesdaySchedule: isChecked,
                            thursdaySchedule: isChecked,
                            fridaySchedule: isChecked,
                            saturdaySchedule: isChecked,
                            sundaySchedule: isChecked,
                        },
                    });
                }
                schedules[gindex] = { ...schedules[gindex], members };
            }
        }

        component.setState({ schedules });
    },

    handleChangeShift: (teamId, employeeId, attrName, shift) => {
        const schedules = [...component.state.schedules];
        const gindex = schedules.findIndex(item => item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId);
        const member = { ...members[eindex] };

        if (shift !== null && isEqual(member[attrName].shift, shift)) {
            return;
        }

        let defaultShift = member.defaultShift !== null ? { ...member.defaultShift } : null;
        if (member.isUsedWeeklyDefaultShift) {
            defaultShift = member.weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`];
        }

        shift = shift === null && !member[attrName].isHoliday ? defaultShift : shift;
        const isOffDay = shift !== null && shift.id === OffDayShiftId;
        shift = shift !== null && shift.id === OffDayShiftId ? null : shift;
        const originalSchedule = component.originalSchedules[gindex].members[eindex][attrName];

        member[attrName] = {
            ...member[attrName],
            isOffDay,
            isOffDayReplacement: false,
            shift,
            isModified: !isEqual(shift, originalSchedule.shift) ? true : originalSchedule.isModified,
        };

        members[eindex] = member;
        schedules[gindex] = { ...schedules[gindex], members };
        component.setState({ schedules });
    },

    updateMemberSchedule: (members, eindex, attrName, originalDefaultShift, defaultShift, gindex) => {
        if (members[eindex][attrName].isDisabled) {
            return;
        }

        const schedule = members[eindex][attrName];
        const shift = schedule.shift;
        const isHolidayOrOffDay = schedule.isHoliday || schedule.isOffDay;
        const shouldUpdate = isHolidayOrOffDay 
            ? (shift !== null && isEqual(originalDefaultShift, shift))
            : (shift === null || isEqual(originalDefaultShift, shift));

        if (shouldUpdate) {
            const originalSchedule = component.originalSchedules[gindex].members[eindex][attrName];
            const isModified = !isEqual(defaultShift, originalSchedule.shift) 
                ? true 
                : originalSchedule.isModified;

            members[eindex][attrName] = {
                ...schedule,
                shift: defaultShift,
                isModified,
            };
        }
    },

    handleChangeDefaultShift: (teamId, employeeId, defaultShift) => {
        const schedules = [...component.state.schedules];
        const gindex = schedules.findIndex(item => item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId),
            originalDefaultShift = members[eindex].defaultShift;

        if (defaultShift !== null) {
            component.scheduleAttrNames.forEach(attrName => {
                component.updateMemberSchedule(members, eindex, attrName, originalDefaultShift, defaultShift, gindex);
            });
        } else {
            component.scheduleAttrNames.forEach(attrName => {
                const shift = members[eindex][attrName].shift;
                if (
                    !members[eindex][attrName].isModified &&
                    shift !== null &&
                    isEqual(originalDefaultShift, shift)
                ) {
                    const originalSchedule = component.originalSchedules[gindex].members[eindex][attrName],
                        isModified = !isEqual(defaultShift, originalSchedule.shift)
                            ? true
                            : originalSchedule.isModified;
                    members[eindex][attrName] = {
                        ...members[eindex][attrName],
                        shift: defaultShift,
                        isModified,
                    };
                }
            });
        }

        members[eindex] = { ...members[eindex], defaultShift };
        schedules[gindex] = { ...schedules[gindex], members };
        component.setState({ schedules });
    },

    handleChangePublics: (teamId, isChecked) => {
        const schedules = [...component.state.schedules];
        const gindex = schedules.findIndex(item => item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const totalItems = members.length;
        for (let index = 0; index < totalItems; index++) {
            members[index] = { ...members[index], isPublic: isChecked };
        }
        schedules[gindex] = {
            ...schedules[gindex],
            isPublic: isChecked,
            members,
        };

        component.setState({ schedules });
    },

    getSelectedMembers: schedules => {
        const selectedMembers = [];
        const totalItems = schedules.length;
        for (let gindex = 0; gindex < totalItems; gindex++) {
            const totalSubItems = schedules[gindex].members.length;
            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                if (
                    component.scheduleAttrNames.filter(attrName =>
                        schedules[gindex].members[eindex].selectWeekDays[attrName]
                    ).length > 0
                ) {
                    selectedMembers.push({ gindex, eindex });
                }
            }
        }

        return selectedMembers;
    },

    isChangeOffDayKIV: item => {
        if (item.isKeepInView) {
            return !item.isOffDay;
        } else return true;
    },

    handleChangeOffNoShift: () => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;
                component.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (
                            !member[attrName].isHoliday &&
                            !member[attrName].isChangeShift &&
                            component.isChangeOffDayKIV(member[attrName])
                        ) {
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: true,
                                isFixedOffDay: false,
                                isOffDayReplacement: false,
                                shift: null,
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            component.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to off!');
        }
    },

    handleChangeOffDayOT: () => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                component.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (
                            !member[attrName].isHoliday &&
                            !member[attrName].isChangeShift &&
                            member[attrName].shift !== null &&
                            component.isChangeOffDayKIV(member[attrName])
                        ) {
                            const originalSchedule = component.originalSchedules[gindex].members[eindex][attrName];
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: true,
                                isFixedOffDay: false,
                                isOffDayReplacement: false,
                                isModified: !originalSchedule.isOffDay
                                    ? true
                                    : originalSchedule.isModified,
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            component.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to off!');
        }
    },

    handleChangeWorkday: () => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                component.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (
                            !member[attrName].isHoliday &&
                            member[attrName].shift !== null
                        ) {
                            const originalSchedule = component.originalSchedules[gindex].members[eindex][attrName];
                            selectWeekDays[attrName] = false;
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: false,
                                isFixedOffDay: false,
                                isOffDayReplacement: false,
                                isModified: originalSchedule.isOffDay
                                    ? true
                                    : originalSchedule.isModified,
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            component.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to work!');
        }
    },

    handleShiftMultiple: () => {
        const selectedMembers = component.getSelectedMembers(component.state.schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            component.setState({ showShiftMultipleModal: true });
        } else {
            toast.warn('Please choose at least one day to continue!');
        }
    },

    handleChangeMultipleShift: (shift, isOffDay) => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                component.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (!member[attrName].isHoliday) {
                            const originalSchedule = component.originalSchedules[gindex].members[eindex][attrName];
                            const isModified = !isEqual(shift, originalSchedule.shift) ||
                                originalSchedule.isOffDay !== isOffDay
                                    ? true
                                    : originalSchedule.isModified;
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: isOffDay && !member[attrName].isChangeShift,
                                isFixedOffDay: isOffDay
                                    ? member[attrName].isFixedOffDay
                                    : false,
                                shift,
                                isModified,
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            component.setState({ schedules });
        }

        component.handleCloseShiftMultipleModal();
    },

    handleFixedOffDay: () => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                component.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (
                            !member[attrName].isHoliday &&
                            !member[attrName].isChangeShift &&
                            member[attrName].shift !== null &&
                            component.isChangeOffDayKIV(member[attrName])
                        ) {
                            const originalSchedule = component.originalSchedules[gindex].members[eindex][attrName];
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: true,
                                isFixedOffDay: true,
                                isOffDayReplacement: false,
                                isModified: !originalSchedule.isOffDay ||
                                    !originalSchedule.isFixedOffDay
                                        ? true
                                        : originalSchedule.isModified,
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            component.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to continue!');
        }
    },

    // Additional handlers that were missing
    validateScheduleChanges: (schedule, totalOffDaysDefault) => {
        const totalOffDays = getTotalOffDays(schedule);
        return totalOffDaysDefault >= totalOffDays;
    },

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
            
            for (let eindex = 0; eindex < members.length; eindex++) {
                const schedule = members[eindex];
                const originalSchedule = originalSchedules[gindex].members[eindex];
                
                unset(schedule, 'selectWeekDays');
                unset(originalSchedule, 'selectWeekDays');

                if (!isEqual(schedule, originalSchedule)) {
                    const totalOffDaysDefault = schedule.offDaysbyWeek + schedule.totalOffDayReplacement;
                    
                    if (!component.validateScheduleChanges(schedule, totalOffDaysDefault)) {
                        toast.error('Some employees have a larger number of days off during the week according to the setting.');
                        return;
                    }

                    const defaultShift = component.collectDefaultShiftChanges(schedule, originalSchedule);
                    if (defaultShift) defaultShifts.push(defaultShift);

                    component.scheduleAttrNames.forEach(attrName => {
                        const weeklySchedule = component.collectWeeklyScheduleChanges(schedule, originalSchedule, attrName);
                        if (weeklySchedule) weeklySchedules.push(weeklySchedule);
                    });
                }
            }
        }

        component.saveScheduleChanges(weeklySchedules, defaultShifts, fromDate, toDate);
    }
});

export default createScheduleActions;
