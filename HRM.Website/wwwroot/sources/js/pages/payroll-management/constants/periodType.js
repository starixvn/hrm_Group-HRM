export const UserType = {
    LOCAL: 1,
    EXPAT: 2,
};

export const UserTypeLabel = {
    [UserType.LOCAL]: 'Local',
    [UserType.EXPAT]: 'Expat',
};

export const getPeriodTypeLabel = (type) => {
    return UserTypeLabel[type] || 'Unknown';
};

export const PERIOD_TYPES = [UserType.LOCAL, UserType.EXPAT];
