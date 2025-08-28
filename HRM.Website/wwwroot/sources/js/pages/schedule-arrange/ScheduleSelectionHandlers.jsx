export const createScheduleSelectionHandlers = component => ({
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
    }
});

export default createScheduleSelectionHandlers;
