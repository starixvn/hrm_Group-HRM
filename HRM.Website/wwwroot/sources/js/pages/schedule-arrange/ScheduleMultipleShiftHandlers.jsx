import { isEqual } from 'lodash';
import { toast } from 'react-toastify';

const processMemberShift = (member, attrName, shift, isOffDay, gindex, eindex, component) => {
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
};

const processSelectedMembersShift = (schedules, selectedMembers, shift, isOffDay, component) => {
    for (let index = 0; index < selectedMembers.length; index++) {
        const { gindex, eindex } = selectedMembers[index];
        const members = [...schedules[gindex].members];
        const member = { ...members[eindex] };
        
        component.scheduleAttrNames.forEach(attrName => {
            if (member.selectWeekDays[attrName]) {
                processMemberShift(member, attrName, shift, isOffDay, gindex, eindex, component);
            }
        });
        
        members[eindex] = member;
        schedules[gindex] = { ...schedules[gindex], members };
    }
};

export const createScheduleMultipleShiftHandlers = component => ({
    handleShiftMultiple: () => {
        const selectedMembers = component.getSelectedMembers(component.state.schedules);
        
        if (selectedMembers.length === 0) {
            toast.warn('Please choose at least one day to continue!');
            return;
        }
        
        component.setState({ showShiftMultipleModal: true });
    },

    handleChangeMultipleShift: (shift, isOffDay) => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules);
        
        if (selectedMembers.length === 0) {
            component.handleCloseShiftMultipleModal();
            return;
        }
        
        processSelectedMembersShift(schedules, selectedMembers, shift, isOffDay, component);
        component.setState({ schedules });
        component.handleCloseShiftMultipleModal();
    }
});

export default createScheduleMultipleShiftHandlers;
