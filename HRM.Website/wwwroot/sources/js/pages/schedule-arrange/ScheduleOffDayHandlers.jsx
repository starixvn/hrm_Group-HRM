import { toast } from 'react-toastify';

export const createScheduleOffDayHandlers = component => ({
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
    }
});

export default createScheduleOffDayHandlers;
