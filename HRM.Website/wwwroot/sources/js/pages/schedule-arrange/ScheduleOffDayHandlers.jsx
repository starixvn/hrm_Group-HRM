import { toast } from 'react-toastify';

const processMemberOffDay = (member, attrName, _gindex, _eindex, component) => {
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
};

const processMemberOffDayOT = (member, attrName, gindex, eindex, component) => {
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
};

const processMemberWorkday = (member, attrName, gindex, eindex, component) => {
    member.selectWeekDays = {
        ...member.selectWeekDays,
        [attrName]: false,
    };
    
    if (
        !member[attrName].isHoliday &&
        member[attrName].shift !== null
    ) {
        const originalSchedule = component.originalSchedules[gindex].members[eindex][attrName];
        member.selectWeekDays[attrName] = false;
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
};

const processMemberFixedOffDay = (member, attrName, gindex, eindex, component) => {
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
};

const processSelectedMembers = (schedules, selectedMembers, processFunction, component) => {
    for (let index = 0; index < selectedMembers.length; index++) {
        const { gindex, eindex } = selectedMembers[index];
        const members = [...schedules[gindex].members];
        const member = { ...members[eindex] };
        
        component.scheduleAttrNames.forEach(attrName => {
            if (member.selectWeekDays[attrName]) {
                processFunction(member, attrName, gindex, eindex, component);
            }
        });
        
        members[eindex] = member;
        schedules[gindex] = { ...schedules[gindex], members };
    }
};

export const createScheduleOffDayHandlers = component => ({
    isChangeOffDayKIV: item => {
        if (item.isKeepInView) {
            return !item.isOffDay;
        } else {
            return true;
        }
    },

    handleChangeOffNoShift: () => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules);
        
        if (selectedMembers.length === 0) {
            toast.warn('Please choose at least one day to off!');
            return;
        }
        
        processSelectedMembers(schedules, selectedMembers, processMemberOffDay, component);
        component.setState({ schedules });
    },

    handleChangeOffDayOT: () => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules);
        
        if (selectedMembers.length === 0) {
            toast.warn('Please choose at least one day to off!');
            return;
        }
        
        processSelectedMembers(schedules, selectedMembers, processMemberOffDayOT, component);
        component.setState({ schedules });
    },

    handleChangeWorkday: () => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules);
        
        if (selectedMembers.length === 0) {
            toast.warn('Please choose at least one day to work!');
            return;
        }
        
        processSelectedMembers(schedules, selectedMembers, processMemberWorkday, component);
        component.setState({ schedules });
    },

    handleFixedOffDay: () => {
        const schedules = [...component.state.schedules];
        const selectedMembers = component.getSelectedMembers(schedules);
        
        if (selectedMembers.length === 0) {
            toast.warn('Please choose at least one day to continue!');
            return;
        }
        
        processSelectedMembers(schedules, selectedMembers, processMemberFixedOffDay, component);
        component.setState({ schedules });
    }
});

export default createScheduleOffDayHandlers;
