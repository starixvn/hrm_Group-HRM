import React from 'react';
import ShiftMultipleModal from './ShiftMultipleModal';
import WeeklyDefaultShiftModal from './WeeklyDefaultShiftModal';
import SweetModalDialog from '../../shared/components/SweetModalDialog';
import moment from 'moment';
import { DateFormat } from '../../shared/constants';

const ScheduleModals = ({
    showShiftMultipleModal,
    showConfirmRefreshDataModal,
    showConfirmSyncDataModalDialog,
    editWeeklyDefaultShift,
    shiftOptions,
    OffDayShiftId,
    isHost,
    currentWeek,
    onCloseShiftMultipleModal,
    onCloseWeeklyDefaultShiftModal,
    onChangeMultipleShift,
    onChangeWeeklyDefaultShift,
    onCloseConfirmRefreshDataModal,
    onSubmitRefreshData,
    onCloseConfirmSyncDataModalDialog,
    onSubmitSyncData
}) => {
    return (
        <>
            {showShiftMultipleModal && (
                <ShiftMultipleModal
                    show={showShiftMultipleModal}
                    onHide={onCloseShiftMultipleModal}
                    shiftOptions={shiftOptions.filter(
                        shift => shift.id !== OffDayShiftId
                    )}
                    onSubmit={onChangeMultipleShift}
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
                    fromDate={moment(currentWeek.toDate)
                        .add(1, 'days')
                        .format(DateFormat)}
                    shiftOptions={shiftOptions}
                    onHide={onCloseWeeklyDefaultShiftModal}
                    onSubmit={onChangeWeeklyDefaultShift}
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
                    onHide={onCloseConfirmRefreshDataModal}
                    onSubmit={onSubmitRefreshData}
                />
            )}
            {showConfirmSyncDataModalDialog && (
                <SweetModalDialog
                    show={showConfirmSyncDataModalDialog}
                    contents={
                        <p>Are you sure you want to sync data from OWP?</p>
                    }
                    onHide={onCloseConfirmSyncDataModalDialog}
                    onSubmit={onSubmitSyncData}
                />
            )}
        </>
    );
};

export default ScheduleModals;
