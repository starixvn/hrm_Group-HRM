import axios from 'axios';
import { cloneDeep, isEqual, unset } from 'lodash';
import moment from 'moment';
import { axiosError, blockUI, downloadFile, resolveClientUrl, unblockUI } from '../../shared/common.functions';
import { DateFormat } from '../../shared/constants';

const EXPORT_DATE_FORMAT = 'DD-MMM-YYYY';

export const createScheduleHandlers = (component) => ({
    loadTeams: companyId => {
        axios({
            url: resolveClientUrl('Team/GetOrgChart'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                companyId: companyId || null,
            },
        })
            .then(response => {
                component.teamOptions = response.data;
                const { floatData } = require('../../shared/components/react-tree-multiselect/buildData').default(response.data);
                const filter = {
                    ...component.state.filter,
                    teamIds: floatData
                        .filter(item => item.isLeaf)
                        .map(item => item.id)
                        .join(','),
                };

                component.setState({ filter: filter });
                component.getWeeklyShedules(filter);
            })
            .catch(axiosError);
    },

    loadShifts: companyId => {
        axios({
            url: resolveClientUrl('Calendar/GetShift'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                query: '',
                isShowOff: true,
                companyId: companyId,
            },
        })
            .then(response => {
                component.shiftOptions = response.data.map(shift => {
                    return { id: shift.id, code: shift.code, name: shift.name };
                });
            })
            .catch(axiosError);
    },

    handleCompanyChange: companyId => {
        component.setState({
            filter: {
                ...component.state.filter,
                companyId: companyId,
                employee: { value: 0, label: '' },
            },
            employeeOptions: [],
        });

        component.loadTeams(companyId);
        component.loadShifts(companyId);
    },

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
        const schedules = cloneDeep(component.state.schedules),
            originalSchedules = cloneDeep(component.originalSchedules);

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

    getEmployees: query => {
        if (!component.state.filter.teamIds) {
            component.setState({ employeeOptions: [] });
            return;
        }

        axios({
            url: resolveClientUrl('ScheduleArrange/GetEmployees'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                teamIds: component.state.filter.teamIds,
                query: query,
            },
        })
            .then(response => {
                const employeeOptions = response.data.map(item => {
                    return {
                        id: item.employeeId,
                        value: `${item.nickname} (${item.fullName})`,
                    };
                });

                component.setState({ employeeOptions: employeeOptions });
            })
            .catch(axiosError);
    },

    getWeeklyShedules: filter => {
        const fromDate = moment(filter.fromDate).format(DateFormat),
            toDate = moment(filter.toDate).format(DateFormat);
        blockUI();

        axios({
            url: resolveClientUrl('ScheduleArrange/GetWeeklySchedule'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                fromDate: fromDate,
                toDate: toDate,
                teamIds: filter.teamIds,
                companyId: filter.companyId,
                isLocal: filter.isLocal,
                employeeId: filter.employee.value === 0 ? null : filter.employee.value,
            },
        })
            .then(response => {
                component.selectedWeek = { fromDate, toDate };
                const { isHost, enableSyncWorkingShift, schedules } = response.data;

                component.originalSchedules = schedules.map(team => {
                    team.members.map(schedule => {
                        schedule.selectWeekDays = {
                            mondaySchedule: false,
                            tuesdaySchedule: false,
                            wednesdaySchedule: false,
                            thursdaySchedule: false,
                            fridaySchedule: false,
                            saturdaySchedule: false,
                            sundaySchedule: false,
                        };
                        schedule.totalOffDayReplacement = 0;

                        Object.keys(schedule.selectWeekDays).forEach(objectKey => {
                            if (schedule[objectKey].isOffDayReplacement) {
                                schedule.totalOffDayReplacement = schedule.totalOffDayReplacement + 1;
                            }
                        });

                        return schedule;
                    });

                    return team;
                });

                component.setState({
                    filter: { ...component.state.filter, fromDate, toDate },
                    isHost,
                    enableSyncWorkingShift,
                    schedules: cloneDeep(component.originalSchedules),
                });
            })
            .catch(axiosError)
            .then(() => unblockUI());
    },

    handleExportExcel: () => {
        const filter = component.state.filter,
            fromDate = moment(filter.fromDate).format(EXPORT_DATE_FORMAT),
            toDate = moment(filter.toDate).format(EXPORT_DATE_FORMAT);

        blockUI();
        axios({
            url: resolveClientUrl('ScheduleArrange/ExportExcel'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                fromDate: fromDate,
                toDate: toDate,
                teamIds: filter.teamIds,
                companyId: filter.companyId,
                isLocal: filter.isLocal,
                employeeId: filter.employee.value === 0 ? null : filter.employee.value,
            },
            responseType: 'blob',
        })
            .then(response => {
                const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
                downloadFile(response.data, fileName);
            })
            .catch(axiosError)
            .then(() => unblockUI());
    },

    handleExportExcelDriverPickup: () => {
        const filter = component.state.filter,
            fromDate = moment(filter.fromDate).format(EXPORT_DATE_FORMAT),
            toDate = moment(filter.toDate).format(EXPORT_DATE_FORMAT);

        blockUI();
        axios({
            url: resolveClientUrl('ScheduleArrange/ExportExcelDriverPickup'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                fromDate: fromDate,
                toDate: toDate,
                teamIds: filter.teamIds,
                companyId: filter.companyId,
                isLocal: filter.isLocal,
                employeeId: filter.employee.value === 0 ? null : filter.employee.value,
            },
            responseType: 'blob',
        })
            .then(response => {
                const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
                downloadFile(response.data, fileName);
            })
            .catch(axiosError)
            .then(() => unblockUI());
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
