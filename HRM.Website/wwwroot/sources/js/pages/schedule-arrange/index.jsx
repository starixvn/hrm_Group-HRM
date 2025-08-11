import 'jquery-ui-bundle/jquery-ui';
import '../../jquery-weekpicker';

import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { toast } from 'react-toastify';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import { cloneDeep, isEqual, unset } from 'lodash';

import {
    axiosError,
    blockUI,
    unblockUI,
    reorder,
    resolveClientUrl,
    downloadFile,
} from '../../shared/common.functions';
import { getTotalOffDays } from './common.functions';
import { DateFormat, OffDayShiftId } from '../../shared/constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import buildData from '../../shared/components/react-tree-multiselect/buildData';
import SweetModalDialog from '../../shared/components/SweetModalDialog';
import CompanySelect, {
    renderCompanySelect,
} from '../../shared/components/CompanySelect';
import ShiftMultipleModal from './ShiftMultipleModal';
import WeeklyDefaultShiftModal from './WeeklyDefaultShiftModal';
import Search from './Search';
import Table from './table/table';

export default class App extends Component {
    constructor(props) {
        super(props);

        const fromDate = moment().day('Monday').format(DateFormat),
            toDate = moment(fromDate).add(6, 'days').format(DateFormat);
        const companyElement = document.getElementById('CompanyId');
        const roleElement = document.getElementById('RoleId');
        const defaultCompanyId = companyElement ? companyElement.value : '';
        const defaultRoleId = roleElement ? roleElement.value : '';

        this.state = {
            isHost: false,
            filter: {
                fromDate: fromDate,
                toDate: toDate,
                teamIds: '',
                companyId: defaultCompanyId,
                employee: { value: 0, label: '' },
            },
            schedules: [],
            employeeOptions: [],
            selectedEmployeeIds: [],
            showShiftMultipleModal: false,
            showConfirmRefreshDataModal: false,
            showConfirmSyncDataModalDialog: false,
            showRemarkPopover: false,
            editWeeklyDefaultShift: {
                show: false,
                teamId: 0,
                employeeId: 0,
                nickname: '',
                data: null,
                isUsedWeeklyDefaultShift: false,
            },
            enableSyncWorkingShift: false,
        };

        this.originalSchedules = [];
        this.teamOptions = [];
        this.shiftOptions = [];
        this.currentWeek = { fromDate, toDate };
        this.selectedWeek = { fromDate, toDate };
        this.defaultRoleId = defaultRoleId;
        this.scheduleAttrNames = [
            'mondaySchedule',
            'tuesdaySchedule',
            'wednesdaySchedule',
            'thursdaySchedule',
            'fridaySchedule',
            'saturdaySchedule',
            'sundaySchedule',
        ];
    }

    componentDidMount() {
        renderCompanySelect({
            defaultCompanyId: this.state.filter.companyId,
            roleId: this.defaultRoleId,
            onChange: this.handleCompanyChange,
        });

        const companyElement = document.getElementById('CompanyId');
        const defaultCompanyId = companyElement ? companyElement.value : '';
        this.loadShifts(defaultCompanyId);

        window.addEventListener(
            'beforeunload',
            this.onLeavingPageEventListener
        );
    }

    componentWillUnmount() {
        window.removeEventListener(
            'beforeunload',
            this.onLeavingPageEventListener
        );
    }

    loadTeams = companyId => {
        axios({
            url: resolveClientUrl('Team/GetOrgChart'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                companyId: companyId || null,
            },
        })
            .then(response => {
                this.teamOptions = response.data;
                const { floatData } = buildData(response.data);
                const filter = {
                    ...this.state.filter,
                    teamIds: floatData
                        .filter(item =>item.isLeaf)
                        .map(item =>item.id)
                        .join(','),
                };

                this.setState({ filter: filter });

                this.getWeeklyShedules(filter);
            })
            .catch(axiosError);
    };

    loadShifts = companyId => {
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
                this.shiftOptions = response.data.map(shift => {
                    return { id: shift.id, code: shift.code, name: shift.name };
                });
            })
            .catch(axiosError);
    };

    handleCompanyChange = companyId => {
        this.setState({
            filter: {
                ...this.state.filter,
                companyId: companyId,
                employee: { value: 0, label: '' },
            },
            employeeOptions: [],
        });

        this.loadTeams(companyId);
        this.loadShifts(companyId);
    };

    onLeavingPageEventListener = event => {
        if (!this.isDirtyData()) {
            return undefined;
        }

        const confirmationMessage =
            'It looks like you have been editing something. ' +
            'If you leave before saving, your changes will be lost.';

        (event || window.event).returnValue = confirmationMessage;

        return confirmationMessage;
    };

    isDirtyData = () => {
        let isDirty = false;
        const schedules = cloneDeep(this.state.schedules),
            originalSchedules = cloneDeep(this.originalSchedules),
            totalItems = schedules.length;

        outerloop: for (let gindex = 0; gindex < totalItems; gindex++) {
            const totalSubItems = schedules[gindex].members.length;
            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                const schedule = schedules[gindex].members[eindex],
                    originalSchedule =
                        originalSchedules[gindex].members[eindex];
                unset(schedule, 'selectWeekDays');
                unset(originalSchedule, 'selectWeekDays');

                if (!isEqual(schedule, originalSchedule)) {
                    isDirty = true;
                    break outerloop;
                }
            }
        }

        return isDirty;
    };

    handleFilterChange = (name, value) => {
        this.setState({ filter: { ...this.state.filter, [name]: value } });
    };

    getEmployees = query => {
        if (!this.state.filter.teamIds) {
            this.setState({ employeeOptions: [] });
            return;
        }

        axios({
            url: resolveClientUrl('ScheduleArrange/GetEmployees'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                teamIds: this.state.filter.teamIds,
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

                this.setState({ employeeOptions: employeeOptions });
            })
            .catch(axiosError);
    };

    getWeeklyShedules = filter => {
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
                employeeId:
                    filter.employee.value === 0 ? null : filter.employee.value,
            },
        })
            .then(response => {
                this.selectedWeek = { fromDate, toDate };
                const { isHost, enableSyncWorkingShift, schedules } =
                    response.data;

                this.originalSchedules = schedules.map(team => {
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

                        Object.keys(schedule.selectWeekDays).forEach
                            (objectKey => {
                                if (schedule[objectKey].isOffDayReplacement) {
                                    schedule.totalOffDayReplacement =
                                        schedule.totalOffDayReplacement + 1;
                                }
                            }
                        );

                        return schedule;
                    });

                    return team;
                });

                this.setState({
                    filter: { ...this.state.filter, fromDate, toDate },
                    isHost,
                    enableSyncWorkingShift,
                    schedules: cloneDeep(this.originalSchedules),
                });
            })
            .catch(axiosError)
            .then( =>unblockUI());
    };

    handleExportExcel = () => {
        const filter = this.state.filter,
            fromDate = moment(filter.fromDate).format('DD-MMM-YYYY'),
            toDate = moment(filter.toDate).format('DD-MMM-YYYY');

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
                employeeId:
                    filter.employee.value === 0 ? null : filter.employee.value,
            },
            responseType: 'blob',
        })
            .then(response => {
                const fileName =
                    response.headers['content-disposition'].match(
                        /filename="(.+)"/
                    )[1];
                downloadFile(response.data, fileName);
            })
            .catch(axiosError)
            .then( =>unblockUI());
    };

    handleExportExcelDriverPickup = () => {
        const filter = this.state.filter,
            fromDate = moment(filter.fromDate).format('DD-MMM-YYYY'),
            toDate = moment(filter.toDate).format('DD-MMM-YYYY');

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
                employeeId:
                    filter.employee.value === 0 ? null : filter.employee.value,
            },
            responseType: 'blob',
        })
            .then(response => {
                const fileName =
                    response.headers['content-disposition'].match(
                        /filename="(.+)"/
                    )[1];
                downloadFile(response.data, fileName);
            })
            .catch(axiosError)
            .then( =>unblockUI());
    };

    handleGetWeeklyShedules = isCurrentWeek => {
        if (this.isDirtyData()) {
            this.handleShowConfirmRefreshDataModal();
        } else {
            const filter = isCurrentWeek
                ? {
                      ...this.state.filter,
                      fromDate: this.currentWeek.fromDate,
                      toDate: this.currentWeek.toDate,
                  }
                : this.state.filter;

            this.setState({ filter });

            this.getWeeklyShedules(filter);
        }
    };

    handleGetPreviousWeekSchedule = () => {
        const fromDate = moment(this.selectedWeek.fromDate)
                .add(-7, 'days')
                .format(DateFormat),
            toDate = moment(this.selectedWeek.toDate)
                .add(-7, 'days')
                .format(DateFormat),
            filter = { ...this.state.filter, fromDate, toDate };
        this.setState({ filter });

        if (this.isDirtyData()) {
            this.handleShowConfirmRefreshDataModal();
        } else {
            this.getWeeklyShedules(filter);
        }
    };

    handleGetNextWeekSchedule = () => {
        const fromDate = moment(this.selectedWeek.fromDate)
                .add(7, 'days')
                .format(DateFormat),
            toDate = moment(this.selectedWeek.toDate)
                .add(7, 'days')
                .format(DateFormat),
            filter = { ...this.state.filter, fromDate, toDate };
        this.setState({ filter });

        if (this.isDirtyData()) {
            this.handleShowConfirmRefreshDataModal();
        } else {
            this.getWeeklyShedules(filter);
        }
    };

    handleChangeSelectWeekDay = (
        teamId,
        employeeId,
        scheduleAttrName,
        isChecked
    ) => {
        const schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item =>item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex
            (item =>item.employeeId === employeeId
        );
        members[eindex] = Object.assign({}, members[eindex], {
            selectWeekDays: {
                ...members[eindex].selectWeekDays,
                [scheduleAttrName]: isChecked,
            },
        });
        schedules[gindex] = { ...schedules[gindex], members };

        this.setState({ schedules });
    };

    handleChangeSelectWeekDays = (scheduleAttrName, isChecked) => {
        const schedules = [...this.state.schedules];
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

        this.setState({ schedules });
    };

    handleChangeShift = (teamId, employeeId, attrName, shift) => {
        const schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item =>item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex
            (item =>item.employeeId === employeeId
        );
        const member = { ...members[eindex] };

        if (shift !== null && isEqual(member[attrName].shift, shift)) {
            return;
        }

        let defaultShift =
            member.defaultShift !== null ? { ...member.defaultShift } : null;
        if (member.isUsedWeeklyDefaultShift) {
            defaultShift =
                member.weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`];
        }

        shift =
            shift === null && !member[attrName].isHoliday
                ? defaultShift
                : shift;
        const isOffDay = shift !== null && shift.id === OffDayShiftId;
        shift = shift !== null && shift.id === OffDayShiftId ? null : shift;
        const originalSchedule =
            this.originalSchedules[gindex].members[eindex][attrName];

        member[attrName] = {
            ...member[attrName],
            isOffDay,
            isOffDayReplacement: false,
            shift,
            isModified: !isEqual(shift, originalSchedule.shift)
                ? true
                : originalSchedule.isModified,
        };

        members[eindex] = member;
        schedules[gindex] = { ...schedules[gindex], members };
        this.setState({ schedules });
    };

    handleChangeDefaultShift = (teamId, employeeId, defaultShift) => {
        const schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item =>item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex
                (item =>item.employeeId === employeeId
            ),
            originalDefaultShift = members[eindex].defaultShift;

        if (defaultShift !== null) {
            this.scheduleAttrNames.forEach(attrName => {
                if (!members[eindex][attrName].isDisabled) {
                    const shift = members[eindex][attrName].shift;
                    if (
                        members[eindex][attrName].isHoliday ||
                        members[eindex][attrName].isOffDay
                    ) {
                        if (
                            shift !== null &&
                            isEqual(originalDefaultShift, shift)
                        ) {
                            const originalSchedule =
                                    this.originalSchedules[gindex].members[
                                        eindex
                                    ][attrName],
                                isModified = !isEqual(
                                    defaultShift,
                                    originalSchedule.shift
                                )
                                    ? true
                                    : originalSchedule.isModified;
                            members[eindex][attrName] = {
                                ...members[eindex][attrName],
                                shift: defaultShift,
                                isModified,
                            };
                        }
                    } else {
                        if (
                            shift === null ||
                            isEqual(originalDefaultShift, shift)
                        ) {
                            const originalSchedule =
                                    this.originalSchedules[gindex].members[
                                        eindex
                                    ][attrName],
                                isModified = !isEqual(
                                    defaultShift,
                                    originalSchedule.shift
                                )
                                    ? true
                                    : originalSchedule.isModified;
                            members[eindex][attrName] = {
                                ...members[eindex][attrName],
                                shift: defaultShift,
                                isModified,
                            };
                        }
                    }
                }
            });
        } else {
            this.scheduleAttrNames.forEach(attrName => {
                const shift = members[eindex][attrName].shift;
                if (
                    !members[eindex][attrName].isModified &&
                    shift !== null &&
                    isEqual(originalDefaultShift, shift)
                ) {
                    const originalSchedule =
                            this.originalSchedules[gindex].members[eindex][
                                attrName
                            ],
                        isModified = !isEqual(
                            defaultShift,
                            originalSchedule.shift
                        )
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

        this.setState({ schedules });
    };

    handleChangePublics = (teamId, isChecked) => {
        const schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item =>item.teamId === teamId);
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

        this.setState({ schedules });
    };

    getSelectedMembers = schedules => {
        const selectedMembers = [];
        const totalItems = schedules.length;
        for (let gindex = 0; gindex < totalItems; gindex++) {
            const totalSubItems = schedules[gindex].members.length;
            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                if 
                    this.scheduleAttrNames.filter(
                        (attrName =>schedules[gindex].members[eindex].selectWeekDays[
                                attrName
                            ]
                    ).length > 0
                ) {
                    selectedMembers.push({ gindex, eindex });
                }
            }
        }

        return selectedMembers;
    };

    isChangeOffDayKIV = item => {
        if (item.isKeepInView) {
            return !item.isOffDay;
        } else return true;
    };

    handleChangeOffNoShift = () => {
        const schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;
                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (
                            !member[attrName].isHoliday &&
                            !member[attrName].isChangeShift &&
                            this.isChangeOffDayKIV(member[attrName])
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

            this.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to off!');
        }
    };

    handleChangeOffDayOT = () => {
        const schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (
                            !member[attrName].isHoliday &&
                            !member[attrName].isChangeShift &&
                            member[attrName].shift !== null &&
                            this.isChangeOffDayKIV(member[attrName])
                        ) {
                            const originalSchedule =
                                this.originalSchedules[gindex].members[eindex][
                                    attrName
                                ];
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

            this.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to off!');
        }
    };

    handleChangeWorkday = () => {
        const schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (
                            !member[attrName].isHoliday &&
                            member[attrName].shift !== null
                        ) {
                            const originalSchedule =
                                this.originalSchedules[gindex].members[eindex][
                                    attrName
                                ];
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

            this.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to work!');
        }
    };

    handleShiftMultiple = () => {
        const selectedMembers = this.getSelectedMembers(this.state.schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            this.setState({ showShiftMultipleModal: true });
        } else {
            toast.warn('Please choose at least one day to continue!');
        }
    };

    handleChangeMultipleShift = (shift, isOffDay) => {
        const schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (!member[attrName].isHoliday) {
                            const originalSchedule =
                                this.originalSchedules[gindex].members[eindex][
                                    attrName
                                ];
                            const isModified =
                                !isEqual(shift, originalSchedule.shift) ||
                                originalSchedule.isOffDay !== isOffDay
                                    ? true
                                    : originalSchedule.isModified;
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay:
                                    isOffDay && !member[attrName].isChangeShift,
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

            this.setState({ schedules });
        }

        this.handleCloseShiftMultipleModal();
    };

    handleFixedOffDay = () => {
        const schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                const members = [...schedules[gindex].members];
                const member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = {
                            ...member.selectWeekDays,
                            [attrName]: false,
                        };
                        if (
                            !member[attrName].isHoliday &&
                            !member[attrName].isChangeShift &&
                            member[attrName].shift !== null &&
                            this.isChangeOffDayKIV(member[attrName])
                        ) {
                            const originalSchedule =
                                this.originalSchedules[gindex].members[eindex][
                                    attrName
                                ];
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: true,
                                isFixedOffDay: true,
                                isOffDayReplacement: false,
                                isModified:
                                    !originalSchedule.isOffDay ||
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

            this.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to continue!');
        }
    };

    handleSave = () => {
        let isValidData = true;
        const weeklySchedules = [],
            defaultShifts = [],
            schedules = cloneDeep(this.state.schedules),
            originalSchedules = cloneDeep(this.originalSchedules);
        const totalItems = schedules.length,
            { fromDate, toDate } = this.selectedWeek;

        outerloop: for (let gindex = 0; gindex < totalItems; gindex++) {
            const totalSubItems = schedules[gindex].members.length;

            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                const schedule = schedules[gindex].members[eindex],
                    originalSchedule =
                        originalSchedules[gindex].members[eindex];
                unset(schedule, 'selectWeekDays');
                unset(originalSchedule, 'selectWeekDays');

                if (!isEqual(schedule, originalSchedule)) {
                    const totalOffDaysDefault =
                            schedule.offDaysbyWeek +
                            schedule.totalOffDayReplacement,
                        totalOffDays = getTotalOffDays(schedule);

                    if (totalOffDaysDefault < totalOffDays) {
                        isValidData = false;
                        break outerloop;
                    }

                    if (
                        !isEqual(
                            originalSchedule.defaultShift,
                            schedule.defaultShift
                        )
                    ) {
                        defaultShifts.push({
                            employeeId: schedule.employeeId,
                            fromDate: schedule.mondaySchedule.workDate,
                            shiftId:
                                schedule.defaultShift !== null
                                    ? schedule.defaultShift.id
                                    : null,
                        });
                    }
                    this.scheduleAttrNames.forEach(attrName => {
                        if (
                            !isEqual(
                                originalSchedule[attrName],
                                schedule[attrName]
                            ) ||
                            originalSchedule.isPublic !== schedule.isPublic
                        ) {
                            weeklySchedules.push({
                                employeeId: schedule.employeeId,
                                workDate: schedule[attrName].workDate,
                                shiftId:
                                    schedule[attrName].shift !== null
                                        ? schedule[attrName].shift.id
                                        : null,
                                isHoliday: schedule[attrName].isHoliday,
                                isOffDay: schedule[attrName].isOffDay,
                                isFixedOffDay: schedule[attrName].isFixedOffDay,
                                isPublic: schedule.isPublic,
                            });
                        }
                    });
                }
            }
        }

        if (!isValidData) {
            toast.error(
                'Some employees have a larger number of days off during the week according to the setting.'
            );

            return;
        }
        if (weeklySchedules.length > 0 || defaultShifts.length > 0) {
            blockUI();

            axios({
                url: resolveClientUrl('ScheduleArrange/EditWeekly'),
                method: 'POST',
                data: stringify({
                    fromDate,
                    toDate,
                    weeklySchedules:
                        weeklySchedules.length > 0 ? weeklySchedules : null,
                    defaultShifts,
                }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
            })
                .then(response => {
                    const data = response.data;
                    if (data.isSuccessful) {
                        toast.success('Updated Successfully!');

                        this.getWeeklyShedules(this.state.filter);
                    } else {
                        toast.error(data.message);
                    }
                })
                .catch(axiosError)
                .then( =>unblockUI());
        }
    };

    handleCloseShiftMultipleModal = () => {
        this.setState({ showShiftMultipleModal: false });
    };

    handleShowConfirmRefreshDataModal = () => {
        this.setState({ showConfirmRefreshDataModal: true });
    };

    handleCloseConfirmRefreshDataModal = () => {
        const fromDate = moment(this.selectedWeek.fromDate).format(DateFormat),
            toDate = moment(this.selectedWeek.toDate).format(DateFormat),
            filter = { ...this.state.filter, fromDate, toDate };

        this.setState({ filter, showConfirmRefreshDataModal: false });
    };

    handleSubmitRefreshData = () => {
        this.getWeeklyShedules(this.state.filter);
        this.handleCloseConfirmRefreshDataModal();
    };

    handleChangeShowRemarkPopover = show => {
        this.setState({ showRemarkPopover: show });
    };

    handleShowWeeklyDefaultShiftModal = (teamId, employeeId) => {
        const schedules = this.state.schedules,
            gindex = schedules.findIndex(item =>item.teamId === teamId),
            eindex = schedules[gindex].members.findIndex
                (item =>item.employeeId === employeeId
            );

        this.setState({
            editWeeklyDefaultShift: {
                show: true,
                teamId,
                employeeId,
                nickname: schedules[gindex].members[eindex].nickname,
                data: schedules[gindex].members[eindex].weeklyDefaultShift,
                isUsedWeeklyDefaultShift:
                    schedules[gindex].members[eindex].isUsedWeeklyDefaultShift,
            },
        });
    };

    handleCloseWeeklyDefaultShiftModal = () => {
        this.setState({
            editWeeklyDefaultShift: {
                show: false,
                teamId: 0,
                employeeId: 0,
                nickname: '',
                data: null,
                isUsedWeeklyDefaultShift: false,
            },
        });
    };

    updateWeeklyDefaultShift = (
        teamId,
        employeeId,
        weeklyDefaultShift,
        isUsedWeeklyDefaultShift
    ) => {
        const schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item =>item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex
            (item =>item.employeeId === employeeId
        );
        members[eindex] = {
            ...members[eindex],
            weeklyDefaultShift,
            isUsedWeeklyDefaultShift,
        };
        this.originalSchedules[gindex].members[eindex] = {
            ...this.originalSchedules[gindex].members[eindex],
            weeklyDefaultShift,
            isUsedWeeklyDefaultShift,
        };
        const fromDate = moment(this.currentWeek.toDate)
            .add(1, 'days')
            .format(DateFormat);

        if (isUsedWeeklyDefaultShift) {
            this.scheduleAttrNames.forEach(attrName => {
                const shiftId =
                        weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`]
                            .id,
                    isOffDay = shiftId === OffDayShiftId,
                    shift = isOffDay
                        ? null
                        : weeklyDefaultShift[
                              `${attrName.substring(0, 3)}Shift`
                          ],
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
                    this.originalSchedules[gindex].members[eindex][attrName] = {
                        ...this.originalSchedules[gindex].members[eindex][
                            attrName
                        ],
                        shift,
                        isOffDay,
                    };
                }
            });
        } else {
            const defaultShift = members[eindex].defaultShift;

            if (defaultShift !== null) {
                this.scheduleAttrNames.forEach(attrName => {
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
        this.setState({ schedules });
    };

    handleChangeWeeklyDefaultShift = (
        weeklyDefaultShift,
        isUsedWeeklyDefaultShift
    ) => {
        const { teamId, employeeId } = this.state.editWeeklyDefaultShift;
        const schedules = this.state.schedules;
        const gindex = schedules.findIndex(item =>item.teamId === teamId);
        const members = schedules[gindex].members;
        const eindex = members.findIndex
            (item =>item.employeeId === employeeId
        );

        let totalOffDays = 0;
        this.scheduleAttrNames.forEach(attrName => {
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
                'Content-Type': 'application/x-www-form-urlencoded',
            },
        })
            .then(response => {
                const data = response.data;
                if (data.isSuccessful) {
                    toast.success('Updated Successfully!');
                    this.updateWeeklyDefaultShift(
                        teamId,
                        employeeId,
                        weeklyDefaultShift,
                        isUsedWeeklyDefaultShift
                    );

                    this.handleCloseWeeklyDefaultShiftModal();
                } else {
                    toast.error(data.message);
                }
            })
            .catch(axiosError);
    };

    handleUpdateRemark = (teamId, employeeId, remark) => {
        const schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item =>item.teamId === teamId);
        const members = [...schedules[gindex].members];
        const eindex = members.findIndex
            (item =>item.employeeId === employeeId
        );

        if (
            !isEqual(
                remark,
                this.originalSchedules[gindex].members[eindex].remark
            )
        ) {
            axios({
                url: resolveClientUrl('ScheduleArrange/EditRemark'),
                method: 'POST',
                data: stringify({ employeeId, remark }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
            })
                .then(response => {
                    const data = response.data;
                    if (data.isSuccessful) {
                        toast.success('Updated Successfully!');

                        members[eindex] = { ...members[eindex], remark };
                        schedules[gindex] = { ...schedules[gindex], members };
                        this.originalSchedules[gindex].members[eindex].remark =
                            remark;

                        this.setState({ schedules });
                    } else {
                        toast.error(data.message);
                    }
                })
                .catch(axiosError);
        }
    };

    handleEditTeamOrder = () => {
        const displayOrders = [];
        const schedules = this.state.schedules,
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
                    'Content-Type': 'application/x-www-form-urlencoded',
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
    };

    handleEditDisplayOrder = gindex => {
        const displayOrders = [];
        const members = this.state.schedules[gindex].members,
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
                    'Content-Type': 'application/x-www-form-urlencoded',
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
    };

    handleDragEnd = result => {
        if (
            !result.destination ||
            result.source.index === result.destination.index
        ) {
            return;
        }

        const resultType = result.type,
            sourceIndex = result.source.index,
            destinationIndex = result.destination.index;
        let schedules = [...this.state.schedules];
        if (resultType === 'droppableTeam') {
            schedules = reorder(schedules, sourceIndex, destinationIndex);
            this.originalSchedules = reorder(
                schedules,
                sourceIndex,
                destinationIndex
            );
            this.setState({ schedules }, () => this.handleEditTeamOrder());
        } else if (resultType.includes('droppableMember')) {
            const teamId = parseInt(resultType.split('-')[1], 10),
                index = schedules.findIndex(x =>x.teamId === teamId);
            this.originalSchedules[index].members = reorder(
                this.originalSchedules[index].members,
                sourceIndex,
                destinationIndex
            );
            schedules[index].members = reorder(
                schedules[index].members,
                sourceIndex,
                destinationIndex
            );
            this.setState({ schedules }, () =>
                this.handleEditDisplayOrder(index)
            );
        }
    };

    handleChangeTeamOrder = (key, teamId) => {
        let schedules = [...this.state.schedules];
        const sourceIndex = schedules.findIndex(x =>x.teamId === teamId),
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
        this.originalSchedules = reorder(
            this.originalSchedules,
            sourceIndex,
            destinationIndex
        );
        this.setState({ schedules }, () => this.handleEditTeamOrder());
    };

    handleChangeOrder = (key, teamId, employeeId) => {
        const schedules = [...this.state.schedules];
        const index = schedules.findIndex(x =>x.teamId === teamId),
            totalItems = schedules[index].members.length,
            sourceIndex = schedules[index].members.findIndex
                (schedule =>schedule.employeeId === employeeId
            );
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
        this.originalSchedules[index].members = reorder(
            this.originalSchedules[index].members,
            sourceIndex,
            destinationIndex
        );
        this.setState({ schedules }, () => this.handleEditDisplayOrder(index));
    };

    handleSelectEmployeeOfTeam = (teamId, employeeId, isSelected) => {
        const schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item =>item.teamId === teamId);
        const members = [...schedules[gindex].members];

        if (employeeId !== null) {
            const eindex = members.findIndex
                (item =>item.employeeId === employeeId
            );
            members[eindex] = { ...members[eindex], isSelected };
            schedules[gindex] = {
                ...schedules[gindex],
                isSelected: members.findIndex(item =>!item.isSelected) < 0,
            };
        } else {
            schedules[gindex] = { ...schedules[gindex], isSelected };
            const totalItems = members.length;
            for (let eindex = 0; eindex < totalItems; eindex++) {
                members[eindex] = { ...members[eindex], isSelected };
            }
        }

        schedules[gindex] = { ...schedules[gindex], members };
        this.setState({ schedules });
    };

    handleSyncData = () => {
        const employeeIds = [];
        const schedules = this.state.schedules,
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
            this.setState({
                selectedEmployeeIds: employeeIds,
                showConfirmSyncDataModalDialog: true,
            });
        } else {
            toast.warn('Please select at least a employee to continue!');
        }
    };

    handleCloseConfirmSyncDataModalDialog = () => {
        this.setState({
            selectedEmployeeIds: [],
            showConfirmSyncDataModalDialog: false,
        });
    };

    handleSubmitSyncData = () => {
        blockUI();
        const { fromDate, toDate } = this.selectedWeek;

        axios({
            url: resolveClientUrl('ScheduleArrange/SyncWorkingShift'),
            method: 'POST',
            data: stringify({
                fromDate,
                toDate,
                employeeIds: this.state.selectedEmployeeIds.join(','),
                companyId: this.state.filter.companyId,
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded',
            },
        })
            .then(response => {
                const data = response.data;
                if (data.isSuccessful) {
                    toast.success('Synced successfully!');

                    this.setState({
                        selectedEmployeeIds: [],
                        showConfirmSyncDataModalDialog: false,
                    });

                    this.getWeeklyShedules(this.state.filter);
                } else {
                    toast.error(data.message);
                }
            })
            .catch(axiosError)
            .then( =>unblockUI());
    };

    render() {
        const {
            isHost,
            enableSyncWorkingShift,
            schedules,
            showShiftMultipleModal,
            showConfirmRefreshDataModal,
            showRemarkPopover,
            editWeeklyDefaultShift,
            showConfirmSyncDataModalDialog,
        } = this.state;

        return 
            <Fragment>
                <ToastLayout />
                <Search
                    showExportExcel
                    teamOptions={this.teamOptions}
                    employeeOptions={this.state.employeeOptions}
                    filter={this.state.filter}
                    onSearchEmployees={this.getEmployees}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleGetWeeklyShedules}
                    onExportExcel={this.handleExportExcel}
                    onExportExcelDriverPickup={
                        this.handleExportExcelDriverPickup
                    }
                />
                <Table
                    isHost={isHost}
                    fromDate={this.selectedWeek.fromDate}
                    toDate={this.selectedWeek.toDate}
                    data={schedules}
                    shiftOptions={this.shiftOptions}
                    showRemarkPopover={showRemarkPopover}
                    onPrevious={this.handleGetPreviousWeekSchedule}
                    onNext={this.handleGetNextWeekSchedule}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                    onChangeSelectWeekDays={this.handleChangeSelectWeekDays}
                    onChangeShift={this.handleChangeShift}
                    onChangeDefaultShift={this.handleChangeDefaultShift}
                    onChangePublics={this.handleChangePublics}
                    onChangeOffNoShift={this.handleChangeOffNoShift}
                    onChangeOffdayOT={this.handleChangeOffDayOT}
                    onChangeWorkday={this.handleChangeWorkday}
                    onShiftMultiple={this.handleShiftMultiple}
                    onFixedOffDay={this.handleFixedOffDay}
                    onChangeShowRemarkPopover={
                        this.handleChangeShowRemarkPopover
                    }
                    onShowWeeklyDefaultShift={
                        this.handleShowWeeklyDefaultShiftModal
                    }
                    onUpdateRemark={this.handleUpdateRemark}
                    onDragEnd={this.handleDragEnd}
                    onChangeTeamOrder={this.handleChangeTeamOrder}
                    onChangeOrder={this.handleChangeOrder}
                    onSelectEmployeeOfTeam={this.handleSelectEmployeeOfTeam}
                    onSyncData={this.handleSyncData}
                    onSave={this.handleSave}
                    enableSyncWorkingShift={this.state.enableSyncWorkingShift}
                />
                {showShiftMultipleModal && (
                    <ShiftMultipleModal
                        show={showShiftMultipleModal}
                        onHide={this.handleCloseShiftMultipleModal}
                        shiftOptions={this.shiftOptions.filter(
                            (shift =>shift.id !== OffDayShiftId
                        )}
                        onSubmit={this.handleChangeMultipleShift}
                    />
                )}
                {editWeeklyDefaultShift.show && (
                    <WeeklyDefaultShiftModal
                        show={editWeeklyDefaultShift.show}
                        isHost={isHost}
                        nickname={editWeeklyDefaultShift.nickname}
                        data={editWeeklyDefaultShift.data}
                        isUsedWeeklyDefaultShift={
                            editWeeklyDefaultShift.isUsedWeeklyDefaultShift
                        }
                        fromDate={moment(this.currentWeek.toDate)
                            .add(1, 'days')
                            .format(DateFormat)}
                        shiftOptions={this.shiftOptions}
                        onHide={this.handleCloseWeeklyDefaultShiftModal}
                        onSubmit={this.handleChangeWeeklyDefaultShift}
                    />
                )}
                {showConfirmRefreshDataModal && (
                    <SweetModalDialog
                        show={showConfirmRefreshDataModal}
                        contents={
                            <p>
                                It looks like you have been editing something.
                                If you refresh before saving, your changes will
                                be lost.
                            </p>
                        }
                        onHide={this.handleCloseConfirmRefreshDataModal}
                        onSubmit={this.handleSubmitRefreshData}
                    />
                )}
                {showConfirmSyncDataModalDialog && (
                    <SweetModalDialog
                        show={showConfirmSyncDataModalDialog}
                        contents={
                            <p>Are you sure you want to sync data from OWP?</p>
                        }
                        onHide={this.handleCloseConfirmSyncDataModalDialog}
                        onSubmit={this.handleSubmitSyncData}
                    />
                )}
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('schedule-wrapper'));

