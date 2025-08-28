import axios from 'axios';
import { cloneDeep } from 'lodash';
import moment from 'moment';
import { axiosError, blockUI, downloadFile, resolveClientUrl, unblockUI } from '../../shared/common.functions';
import { DateFormat } from '../../shared/constants';

const EXPORT_DATE_FORMAT = 'DD-MMM-YYYY';

export const createScheduleDataExportHandlers = component => ({
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
        component.exportScheduleData('ScheduleArrange/ExportExcel');
    },

    handleExportExcelDriverPickup: () => {
        component.exportScheduleData('ScheduleArrange/ExportExcelDriverPickup');
    },

    exportScheduleData: endpoint => {
        const filter = component.state.filter,
            fromDate = moment(filter.fromDate).format(EXPORT_DATE_FORMAT),
            toDate = moment(filter.toDate).format(EXPORT_DATE_FORMAT);

        blockUI();
        axios({
            url: resolveClientUrl(endpoint),
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
    }
});

export default createScheduleDataExportHandlers;
