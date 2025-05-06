export const PeriodType = {
    ALL: 0,
    FIRST_HALF: 1,
    SECOND_HALF: 2,
    FULL_MONTH: 3,
};

export const PeriodTypeLabel = {
    [PeriodType.FIRST_HALF]: "1st Half (1-15)",
    [PeriodType.SECOND_HALF]: "2nd Half (16-end)",
    [PeriodType.FULL_MONTH]: "Full Month",
    [PeriodType.ALL]: "-- All --",
};

export const getPeriodTypeLabel = (type) => {
    return PeriodTypeLabel[type] || "Unknown";
};

export const PERIOD_TYPES = [
    PeriodType.FIRST_HALF,
    PeriodType.SECOND_HALF,
    PeriodType.FULL_MONTH,
];
