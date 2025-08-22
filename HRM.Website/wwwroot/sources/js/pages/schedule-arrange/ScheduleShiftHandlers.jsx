import { isEqual } from 'lodash';
import { OffDayShiftId } from '../../shared/constants';

const findIndices = (schedules, teamId, employeeId) => {
    const gindex = schedules.findIndex(item => item.teamId === teamId);
    const members = [...schedules[gindex].members];
    const eindex = members.findIndex(item => item.employeeId === employeeId);
    return { gindex, members, eindex };
};

const determineEffectiveShift = (member, attrName, shift) => {
    if (shift !== null) return shift;
    
    if (member[attrName].isHoliday) return null;
    
    if (member.isUsedWeeklyDefaultShift) {
        return member.weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`];
    }
    
    return member.defaultShift !== null ? { ...member.defaultShift } : null;
};

const processOffDayShift = (shift) => {
    const isOffDay = shift !== null && shift.id === OffDayShiftId;
    const processedShift = shift !== null && shift.id === OffDayShiftId ? null : shift;
    return { isOffDay, processedShift };
};

const updateMemberSchedule = (member, attrName, shift, isOffDay, originalSchedule) => {
    member[attrName] = {
        ...member[attrName],
        isOffDay,
        isOffDayReplacement: false,
        shift,
        isModified: !isEqual(shift, originalSchedule.shift) ? true : originalSchedule.isModified,
    };
};

export const createScheduleShiftHandlers = component => ({
    handleChangeShift: (teamId, employeeId, attrName, shift) => {
        const schedules = [...component.state.schedules];
        const { gindex, members, eindex } = findIndices(schedules, teamId, employeeId);
        const member = { ...members[eindex] };

        if (shift !== null && isEqual(member[attrName].shift, shift)) {
            return;
        }

        const effectiveShift = determineEffectiveShift(member, attrName, shift);
        
        const { isOffDay, processedShift } = processOffDayShift(effectiveShift);
        
        const originalSchedule = component.originalSchedules[gindex].members[eindex][attrName];
        
        updateMemberSchedule(member, attrName, processedShift, isOffDay, originalSchedule);
        
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
    }
});

export default createScheduleShiftHandlers;
