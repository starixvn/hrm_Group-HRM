import React from 'react';
import Table from './table/table';

const ScheduleTable = ({
    isHost,
    fromDate,
    toDate,
    schedules,
    shiftOptions,
    showRemarkPopover,
    onPrevious,
    onNext,
    onChangeSelectWeekDay,
    onChangeSelectWeekDays,
    onChangeShift,
    onChangeDefaultShift,
    onChangePublics,
    onChangeOffNoShift,
    onChangeOffdayOT,
    onChangeWorkday,
    onShiftMultiple,
    onFixedOffDay,
    onChangeShowRemarkPopover,
    onShowWeeklyDefaultShift,
    onUpdateRemark,
    onDragEnd,
    onChangeTeamOrder,
    onChangeOrder,
    onSelectEmployeeOfTeam,
    onSyncData,
    onSave,
    enableSyncWorkingShift
}) => {
    return (
        <Table
            isHost={isHost}
            fromDate={fromDate}
            toDate={toDate}
            data={schedules}
            shiftOptions={shiftOptions}
            showRemarkPopover={showRemarkPopover}
            onPrevious={onPrevious}
            onNext={onNext}
            onChangeSelectWeekDay={onChangeSelectWeekDay}
            onChangeSelectWeekDays={onChangeSelectWeekDays}
            onChangeShift={onChangeShift}
            onChangeDefaultShift={onChangeDefaultShift}
            onChangePublics={onChangePublics}
            onChangeOffNoShift={onChangeOffNoShift}
            onChangeOffdayOT={onChangeOffdayOT}
            onChangeWorkday={onChangeWorkday}
            onShiftMultiple={onShiftMultiple}
            onFixedOffDay={onFixedOffDay}
            onChangeShowRemarkPopover={onChangeShowRemarkPopover}
            onShowWeeklyDefaultShift={onShowWeeklyDefaultShift}
            onUpdateRemark={onUpdateRemark}
            onDragEnd={onDragEnd}
            onChangeTeamOrder={onChangeTeamOrder}
            onChangeOrder={onChangeOrder}
            onSelectEmployeeOfTeam={onSelectEmployeeOfTeam}
            onSyncData={onSyncData}
            onSave={onSave}
            enableSyncWorkingShift={enableSyncWorkingShift}
        />
    );
};

export default ScheduleTable;
