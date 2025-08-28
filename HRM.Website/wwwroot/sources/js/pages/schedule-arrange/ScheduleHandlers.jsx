import { isEqual, unset } from 'lodash';
import moment from 'moment';
import { DateFormat } from '../../shared/constants';

export const createScheduleHandlers = component => ({
    onLeavingPageEventListener: event => {
        if (!component.isDirtyData()) {
            return undefined;
        }

        const confirmationMessage =
            'It looks like you have been editing something. ' +
            'If you leave before saving, your changes will be lost.';

        (event || window.event).returnValue = confirmationMessage;
        return confirmationMessage;
    },

    isDirtyData: () => {
        const schedules = component.state.schedules,
            originalSchedules = component.originalSchedules;

        for (let gindex = 0; gindex < schedules.length; gindex++) {
            const totalSubItems = schedules[gindex].members.length;
            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                const schedule = schedules[gindex].members[eindex],
                    originalSchedule = originalSchedules[gindex].members[eindex];
                unset(schedule, 'selectWeekDays');
                unset(originalSchedule, 'selectWeekDays');

                if (!isEqual(schedule, originalSchedule)) {
                    return true;
                }
            }
        }

        return false;
    },

    handleFilterChange: (name, value) => {
        component.setState({ filter: { ...component.state.filter, [name]: value } });
    },

    handleGetWeeklyShedules: isCurrentWeek => {
        if (component.isDirtyData()) {
            component.handleShowConfirmRefreshDataModal();
        } else {
            const filter = isCurrentWeek
                ? {
                    ...component.state.filter,
                    fromDate: component.currentWeek.fromDate,
                    toDate: component.currentWeek.toDate,
                }
                : component.state.filter;

            component.setState({ filter });
            component.getWeeklyShedules(filter);
        }
    },

    handleGetPreviousWeekSchedule: () => {
        const fromDate = moment(component.selectedWeek.fromDate)
            .add(-7, 'days')
            .format(DateFormat),
            toDate = moment(component.selectedWeek.toDate)
                .add(-7, 'days')
                .format(DateFormat),
            filter = { ...component.state.filter, fromDate, toDate };
        component.setState({ filter });

        if (component.isDirtyData()) {
            component.handleShowConfirmRefreshDataModal();
        } else {
            component.getWeeklyShedules(filter);
        }
    },

    handleGetNextWeekSchedule: () => {
        const fromDate = moment(component.selectedWeek.fromDate)
            .add(7, 'days')
            .format(DateFormat),
            toDate = moment(component.selectedWeek.toDate)
                .add(7, 'days')
                .format(DateFormat),
            filter = { ...component.state.filter, fromDate, toDate };
        component.setState({ filter });

        if (component.isDirtyData()) {
            component.handleShowConfirmRefreshDataModal();
        } else {
            component.getWeeklyShedules(filter);
        }
    },

    // Modal handlers
    handleShowConfirmRefreshDataModal: () => {
        component.setState({ showConfirmRefreshDataModal: true });
    },

    handleCloseConfirmRefreshDataModal: () => {
        const fromDate = moment(component.selectedWeek.fromDate).format(DateFormat),
            toDate = moment(component.selectedWeek.toDate).format(DateFormat),
            filter = { ...component.state.filter, fromDate, toDate };

        component.setState({ filter, showConfirmRefreshDataModal: false });
    },

    handleSubmitRefreshData: () => {
        component.getWeeklyShedules(component.state.filter);
        component.handleCloseConfirmRefreshDataModal();
    },

    handleCloseShiftMultipleModal: () => {
        component.setState({ showShiftMultipleModal: false });
    },

    handleChangeShowRemarkPopover: show => {
        component.setState({ showRemarkPopover: show });
    },

    handleShowWeeklyDefaultShiftModal: (teamId, employeeId) => {
        const schedules = component.state.schedules,
            gindex = schedules.findIndex(item => item.teamId === teamId),
            eindex = schedules[gindex].members.findIndex(item => item.employeeId === employeeId);

        component.setState({
            editWeeklyDefaultShift: {
                show: true,
                teamId,
                employeeId,
                nickname: schedules[gindex].members[eindex].nickname,
                data: schedules[gindex].members[eindex].weeklyDefaultShift,
                isUsedWeeklyDefaultShift: schedules[gindex].members[eindex].isUsedWeeklyDefaultShift,
            },
        });
    },

    handleCloseWeeklyDefaultShiftModal: () => {
        component.setState({
            editWeeklyDefaultShift: {
                show: false,
                teamId: 0,
                employeeId: 0,
                nickname: '',
                data: null,
                isUsedWeeklyDefaultShift: false,
            },
        });
    }
});

export default createScheduleHandlers;
