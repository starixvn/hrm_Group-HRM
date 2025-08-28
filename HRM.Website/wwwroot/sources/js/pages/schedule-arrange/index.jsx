import 'jquery-ui-bundle/jquery-ui';
import '../../jquery-weekpicker';

import moment from 'moment';
import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';

import { renderCompanySelect } from '../../shared/components/CompanySelect';
import SweetModalDialog from '../../shared/components/SweetModalDialog';
import { DateFormat, OffDayShiftId } from '../../shared/constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import createScheduleSelectionHandlers from './ScheduleSelectionHandlers';
import createScheduleShiftHandlers from './ScheduleShiftHandlers';
import createScheduleOffDayHandlers from './ScheduleOffDayHandlers';
import createScheduleMultipleShiftHandlers from './ScheduleMultipleShiftHandlers';
import createScheduleSaveHandlers from './ScheduleSaveHandlers';
import createScheduleActions from './ScheduleActions';
import createScheduleUtilities from './ScheduleUtilities';
import createScheduleHandlers from './ScheduleHandlers';
import createScheduleDataExportHandlers from './ScheduleDataExportHandlers';
import Search from './Search';
import ShiftMultipleModal from './ShiftMultipleModal';
import Table from './table/table';
import WeeklyDefaultShiftModal from './WeeklyDefaultShiftModal';

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

        // Initialize handlers, actions, and utilities from external files
        const selectionHandlers = createScheduleSelectionHandlers(this);
        const shiftHandlers = createScheduleShiftHandlers(this);
        const offDayHandlers = createScheduleOffDayHandlers(this);
        const multipleShiftHandlers = createScheduleMultipleShiftHandlers(this);
        const saveHandlers = createScheduleSaveHandlers(this);
        const actions = createScheduleActions(this);
        const utilities = createScheduleUtilities(this);
        const handlers = createScheduleHandlers(this);
        const dataExportHandlers = createScheduleDataExportHandlers(this);
        Object.assign(this, selectionHandlers, shiftHandlers, offDayHandlers, multipleShiftHandlers, saveHandlers, actions, utilities, handlers, dataExportHandlers);
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

    render() {
        const {
            isHost,
            schedules,
            showShiftMultipleModal,
            showConfirmRefreshDataModal,
            showRemarkPopover,
            editWeeklyDefaultShift,
            showConfirmSyncDataModalDialog,
        } = this.state;

        return (
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
                            shift =>shift.id !== OffDayShiftId
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

ReactDOM.render(
    <App />,
    document.getElementById('schedule-wrapper')
);
