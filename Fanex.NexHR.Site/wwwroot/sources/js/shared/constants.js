export const DateFormat = 'DD MMM, YYYY';
export const DateTimeFormat = 'DD MMM, YYYY HH:mm';
export const OffDayShiftId = -1;
export const RequestType = {
    CheckIn: 1,
    CheckOut: 2,
    LateIn: 3,
    EarlyOut: 4,
    ChangeShift: 5,
    Leave: 6,
    KeepInView: 7,
    Overtime: 8
};
export const LeaveType = {
    GeneralLeave: 0,
    AnnualLeave: 2,
    WeddingLeave: 6,
    BusinessTrip: 9,
    KeepInViewByDay: 10,
    MaternityLeave: 12,
    PublicHoliday: 13,
    PaternityLeave: 14,
    BereavementLeave: 21,
    UnpaidLeave: 25,
    BusinessDuringWorking: 26,
    OffsetLeave: 27,
    UnpaidLeaveInsurance: 41,
    KeepInViewByHour: 48,
    HospitalizationLeave: 39
};
export const RoleSystem = {
    Member: 1,
    Verifier: 2,
    Approver: 3,
    ShiftManager: 4,
    OnBehalfRequest: 5,
    HR: 6,
    SystemAdmin: 7,
    DisciplineManager: 8,
    ShiftManager2: 9,
    LocalHR: 10,
    ExpatHR: 11
};
export const RequestCommand = {
    Accept: 1,
    NotAccept: 2,
    Approve: 3,
    Reject: 4,
    Process: 5,
    Void: 6,
    Reset: 7,
    VoidBySA: 8
};
export const CurrentRequestStatus =
{
    Pending: 1,
    Verified: 2,
    NotVerified: 3,
    Approved: 4,
    Rejected: 5,
    Processed: 6,
    Void: 7
};
export const ChangeShiftType = {
    ChangeShift: 1,
    MoveOffDay: 2
};
export const KeepInViewType = {
    KeepInViewByDay: 1,
    KeepInViewByHour: 2
};
export const DisciplineType = {
    ChangeShift: 1,
    MissingCheckInOut: 2,
    LateIn: 3,
    EarlyOut: 4,
    Absent: 5,
    UnpaidLeave: 6,
    WorkingUnderTime: 7,
    UnpaidLeaveAbsent: 8,
    UnplannedLeave: 9,
    WeekendLeave: 10,
    SevenDaysLeave: 11
};
export const Messages = {
    notAvailable: 'Not available',
    invalidInputData: 'Invalid input data',
    errorTryAgain: 'An error occurred while processing your request. Please try again!',
    confirmDelete: 'Are you sure you want to delete this?',
    requireResignedDate: 'Please input the resigned date first!',
    hostileInput: 'You have input data that could be considered hostile. Please change data!'
};