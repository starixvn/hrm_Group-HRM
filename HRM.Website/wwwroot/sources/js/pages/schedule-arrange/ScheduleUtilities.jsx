import axios from 'axios';
import { isEqual } from 'lodash';
import moment from 'moment';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { axiosError, blockUI, reorder, resolveClientUrl, unblockUI } from '../../shared/common.functions';
import { DateFormat, OffDayShiftId } from '../../shared/constants';

const FORM_URLENCODED_HEADER = 'application/x-www-form-urlencoded';
const SUCCESS_MESSAGE = 'Updated Successfully!';

export const createScheduleUtilities = (component) => /* NOSONAR */ ({
    updateWeeklyDefaultShift: (
        teamId,
        employeeId,
        weeklyDefaultShift,
        isUsedWeeklyDefaultShift
    ) => {
        const schedules = [...component.state.schedules];
        const gindex = schedules.findIndex(item => item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId);
        members[eindex] = {
            ...members[eindex],
            weeklyDefaultShift,
            isUsedWeeklyDefaultShift,
        };
        component.originalSchedules[gindex].members[eindex] = {
            ...component.originalSchedules[gindex].members[eindex],
            weeklyDefaultShift,
            isUsedWeeklyDefaultShift,
        };
        const fromDate = moment(component.currentWeek.toDate)
            .add(1, 'days')
            .format(DateFormat);

        if (isUsedWeeklyDefaultShift) {
            component.scheduleAttrNames.forEach(attrName => {
                const shiftId = weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`].id,
                    isOffDay = shiftId === OffDayShiftId,
                    shift = isOffDay
                        ? null
                        : weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`],
                    workDate = moment(members[eindex][attrName].workDate);

                if (
                    workDate.isSameOrAfter(fromDate) &&
                    !members[eindex][attrName].isDisabled
                ) {
                    members[eindex][attrName] = {
                        ...members[eindex][attrName],
                        shift,
                        isOffDay,
                    };
                    component.originalSchedules[gindex].members[eindex][attrName] = {
                        ...component.originalSchedules[gindex].members[eindex][attrName],
                        shift,
                        isOffDay,
                    };
                }
            });
        } else {
            const defaultShift = members[eindex].defaultShift;

            if (defaultShift !== null) {
                component.scheduleAttrNames.forEach(attrName => {
                    const workDate = moment(members[eindex][attrName].workDate);
                    if (
                        workDate.isSameOrAfter(fromDate) &&
                        !members[eindex][attrName].isDisabled &&
                        !members[eindex][attrName].isHoliday &&
                        !members[eindex][attrName].isOffDay &&
                        members[eindex][attrName].shift === null
                    ) {
                        members[eindex][attrName] = {
                            ...members[eindex][attrName],
                            shift: defaultShift,
                        };
                    }
                });
            }
        }

        schedules[gindex] = { ...schedules[gindex], members };
        component.setState({ schedules });
    },

    handleChangeWeeklyDefaultShift: (
        weeklyDefaultShift,
        isUsedWeeklyDefaultShift
    ) => {
        const { teamId, employeeId } = component.state.editWeeklyDefaultShift;
        const schedules = component.state.schedules;
        const gindex = schedules.findIndex(item => item.teamId === teamId);
        const members = schedules[gindex].members;
        const eindex = members.findIndex(item => item.employeeId === employeeId);

        let totalOffDays = 0;
        component.scheduleAttrNames.forEach(attrName => {
            const shiftId =
                    weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`].id,
                isOffDay = shiftId === OffDayShiftId;

            if (isOffDay) {
                totalOffDays++;
            }
        });

        if (totalOffDays > members[eindex].offDaysbyWeek) {
            toast.error(
                'Employee has a larger number of days off during the week according to the setting.'
            );

            return;
        }

        axios({
            url: resolveClientUrl('ScheduleArrange/EditWeeklyDefaultShift'),
            method: 'POST',
            data: stringify({ weeklyDefaultShift, isUsedWeeklyDefaultShift }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': FORM_URLENCODED_HEADER,
            },
        })
            .then(response => {
                const data = response.data;
                if (data.isSuccessful) {
                    toast.success(SUCCESS_MESSAGE);
                    component.updateWeeklyDefaultShift(
                        teamId,
                        employeeId,
                        weeklyDefaultShift,
                        isUsedWeeklyDefaultShift
                    );

                    component.handleCloseWeeklyDefaultShiftModal();
                } else {
                    toast.error(data.message);
                }
            })
            .catch(axiosError);
    },

    handleUpdateRemark: (teamId, employeeId, remark) => {
        const schedules = [...component.state.schedules];
        const gindex = schedules.findIndex(item => item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId);

        if (
            !isEqual(
                remark,
                component.originalSchedules[gindex].members[eindex].remark
            )
        ) {
            axios({
                url: resolveClientUrl('ScheduleArrange/EditRemark'),
                method: 'POST',
                data: stringify({ employeeId, remark }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': FORM_URLENCODED_HEADER,
                },
            })
                .then(response => {
                    const data = response.data;
                    if (data.isSuccessful) {
                        toast.success(SUCCESS_MESSAGE);

                        members[eindex] = { ...members[eindex], remark };
                        schedules[gindex] = { ...schedules[gindex], members };
                        component.originalSchedules[gindex].members[eindex].remark = remark;

                        component.setState({ schedules });
                    } else {
                        toast.error(data.message);
                    }
                })
                .catch(axiosError);
        }
    },

    handleEditTeamOrder: () => {
        const displayOrders = [];
        const schedules = component.state.schedules,
            totalItems = schedules.length;

        for (let index = 0; index < totalItems; index++) {
            displayOrders.push({
                teamId: schedules[index].teamId,
                displayOrder: index + 1,
            });
        }

        if (displayOrders.length > 0) {
            axios({
                url: resolveClientUrl('ScheduleArrange/EditTeamOrder'),
                method: 'POST',
                data: stringify({ displayOrders }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': FORM_URLENCODED_HEADER,
                },
            })
                .then(response => {
                    const data = response.data;
                    if (!data.isSuccessful) {
                        toast.error(data.message);
                    }
                })
                .catch(axiosError);
        }
    },

    handleEditDisplayOrder: gindex => {
        const displayOrders = [];
        const members = component.state.schedules[gindex].members,
            totalItems = members.length;

        for (let index = 0; index < totalItems; index++) {
            displayOrders.push({
                employeeId: members[index].employeeId,
                displayOrder: index + 1,
            });
        }

        if (displayOrders.length > 0) {
            axios({
                url: resolveClientUrl('ScheduleArrange/EditDisplayOrder'),
                method: 'POST',
                data: stringify({ displayOrders }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': FORM_URLENCODED_HEADER,
                },
            })
                .then(response => {
                    const data = response.data;
                    if (!data.isSuccessful) {
                        toast.error(data.message);
                    }
                })
                .catch(axiosError);
        }
    },

    handleDragEnd: result => {
        if (
            !result.destination ||
            result.source.index === result.destination.index
        ) {
            return;
        }

        const resultType = result.type,
            sourceIndex = result.source.index,
            destinationIndex = result.destination.index;
        let schedules = [...component.state.schedules];
        if (resultType === 'droppableTeam') {
            schedules = reorder(schedules, sourceIndex, destinationIndex);
            component.originalSchedules = reorder(
                component.originalSchedules,
                sourceIndex,
                destinationIndex
            );
            component.setState({ schedules }, () => component.handleEditTeamOrder());
        } else if (resultType.includes('droppableMember')) {
            const teamId = parseInt(resultType.split('-')[1], 10),
                index = schedules.findIndex(x => x.teamId === teamId);
            component.originalSchedules[index].members = reorder(
                component.originalSchedules[index].members,
                sourceIndex,
                destinationIndex
            );
            schedules[index].members = reorder(
                schedules[index].members,
                sourceIndex,
                destinationIndex
            );
            component.setState({ schedules }, () =>
                component.handleEditDisplayOrder(index)
            );
        }
    },

    handleChangeTeamOrder: (key, teamId) => {
        let schedules = [...component.state.schedules];
        const sourceIndex = schedules.findIndex(x => x.teamId === teamId),
            totalItems = schedules.length;
        let destinationIndex = 0;

        switch (key) {
            case 'up':
                destinationIndex = sourceIndex - 1;
                break;
            case 'down':
                destinationIndex = sourceIndex + 1;
                break;
            case 'bottom':
                destinationIndex = totalItems;
                break;
        }

        if (
            destinationIndex < 0 ||
            destinationIndex > totalItems ||
            sourceIndex === destinationIndex
        ) {
            return;
        }

        schedules = reorder(schedules, sourceIndex, destinationIndex);
        component.originalSchedules = reorder(
            component.originalSchedules,
            sourceIndex,
            destinationIndex
        );
        component.setState({ schedules }, () => component.handleEditTeamOrder());
    },

    handleChangeOrder: (key, teamId, employeeId) => {
        const schedules = [...component.state.schedules];
        const index = schedules.findIndex(x => x.teamId === teamId),
            totalItems = schedules[index].members.length,
            sourceIndex = schedules[index].members.findIndex(schedule => schedule.employeeId === employeeId);
        let destinationIndex = 0;

        switch (key) {
            case 'up':
                destinationIndex = sourceIndex - 1;
                break;
            case 'down':
                destinationIndex = sourceIndex + 1;
                break;
            case 'bottom':
                destinationIndex = totalItems;
                break;
        }

        if (
            destinationIndex < 0 ||
            destinationIndex > totalItems ||
            sourceIndex === destinationIndex
        ) {
            return;
        }

        schedules[index].members = reorder(
            schedules[index].members,
            sourceIndex,
            destinationIndex
        );
        component.originalSchedules[index].members = reorder(
            component.originalSchedules[index].members,
            sourceIndex,
            destinationIndex
        );
        component.setState({ schedules }, () => component.handleEditDisplayOrder(index));
    },

    handleSelectEmployeeOfTeam: (teamId, employeeId, isSelected) => {
        const schedules = [...component.state.schedules];
        const gindex = schedules.findIndex(item => item.teamId === teamId);
        const members = [...schedules[gindex].members];

        if (employeeId !== null) {
            const eindex = members.findIndex(item => item.employeeId === employeeId);
            members[eindex] = { ...members[eindex], isSelected };
            schedules[gindex] = {
                ...schedules[gindex],
                isSelected: members.findIndex(item => !item.isSelected) < 0,
            };
        } else {
            schedules[gindex] = { ...schedules[gindex], isSelected };
            const totalItems = members.length;
            for (let eindex = 0; eindex < totalItems; eindex++) {
                members[eindex] = { ...members[eindex], isSelected };
            }
        }

        schedules[gindex] = { ...schedules[gindex], members };
        component.setState({ schedules });
    },

    handleSyncData: () => {
        const employeeIds = [];
        const schedules = component.state.schedules,
            totalItems = schedules.length;
        for (let gindex = 0; gindex < totalItems; gindex++) {
            const totalMembers = schedules[gindex].members.length;
            for (let eindex = 0; eindex < totalMembers; eindex++) {
                if (schedules[gindex].members[eindex].isSelected) {
                    employeeIds.push(
                        schedules[gindex].members[eindex].employeeId
                    );
                }
            }
        }

        if (employeeIds.length > 0) {
            component.setState({
                selectedEmployeeIds: employeeIds,
                showConfirmSyncDataModalDialog: true,
            });
        } else {
            toast.warn('Please select at least a employee to continue!');
        }
    },

    handleCloseConfirmSyncDataModalDialog: () => {
        component.setState({
            selectedEmployeeIds: [],
            showConfirmSyncDataModalDialog: false,
        });
    },

    handleSubmitSyncData: () => {
        blockUI();
        const { fromDate, toDate } = component.selectedWeek;

        axios({
            url: resolveClientUrl('ScheduleArrange/SyncWorkingShift'),
            method: 'POST',
            data: stringify({
                fromDate,
                toDate,
                employeeIds: component.state.selectedEmployeeIds.join(','),
                companyId: component.state.filter.companyId,
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': FORM_URLENCODED_HEADER,
            },
        })
            .then(response => {
                const data = response.data;
                if (data.isSuccessful) {
                    toast.success('Synced successfully!');

                    component.setState({
                        selectedEmployeeIds: [],
                        showConfirmSyncDataModalDialog: false,
                    });

                    component.getWeeklyShedules(component.state.filter);
                } else {
                    toast.error(data.message);
                }
            })
            .catch(axiosError)
            .then(() => unblockUI());
    }
});

export default createScheduleUtilities;
