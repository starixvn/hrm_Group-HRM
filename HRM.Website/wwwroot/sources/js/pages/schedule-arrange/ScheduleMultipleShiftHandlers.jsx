import { isEqual } from 'lodash';
import { toast } from 'react-toastify';

export const createScheduleMultipleShiftHandlers = component => ({
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
    }
});

export default createScheduleMultipleShiftHandlers;
