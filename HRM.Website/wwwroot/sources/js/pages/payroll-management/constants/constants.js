export const Category = {
    LOCAL: 1,
    EXPAT: 0,
};

export const CategoryLabel = {
    [Category.LOCAL]: 'Local',
    [Category.EXPAT]: 'Expat',
};

export const CATEGORY = [Category.LOCAL, Category.EXPAT];

export const EmploymentStatus = {
    PROBATION: 1,
    CONFIRMED: 2,
    RESIGNED: 3,
    MATERNITY_LEAVE: 4,
};

export const EmploymentStatusColor = {
    [EmploymentStatus.PROBATION]: 'green',
    [EmploymentStatus.CONFIRMED]: 'blue',
    [EmploymentStatus.RESIGNED]: 'red',
    [EmploymentStatus.MATERNITY_LEAVE]: 'purple',
};
