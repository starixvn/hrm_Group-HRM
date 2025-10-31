export const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
export const categories = ['New Hire', 'Probation Resign', 'Confirmed Resign', 'Transfer', 'Current H/C'];

// Helper to process columns data
export const processColumns = (data) => {
    return data.map(item => ({
        label: item.label1 || '',
        label2: item.label2 || '',
        label3: item.label3 || '',
        key: item.label1 || '',
        isUnitCategory: item.isUnitCategory,
        isGrandTotal: item.isGrandTotal
    }));
};

// Get max month from data
export const getMaxMonth = (data) => {
    return data.reduce((max, item) => {
        const details = item.details;
        const itemMaxMonth = (details && details.reduce) 
            ? details.reduce((m, detail) => Math.max(m, detail.month || 0), 0) 
            : 0;
        return Math.max(max, itemMaxMonth);
    }, 0);
};

// Helper to get value by month and category
export const getValue = (unitData, month, categoryIndex) => {
    const details = unitData.details;
    const detail = (details && details.find)
        ? details.find(d => d.month === month + 1)
        : null;

    if (!detail) {
        return { value: 0, empIds: [] };
    }

    switch (categoryIndex) {
        case 0:
            return { value: detail.newHire || 0, empIds: detail.newHireEmpIds || [] };
        case 1:
            return { value: detail.probationResign || 0, empIds: detail.probationResignEmpIds || [] };
        case 2:
            return { value: detail.confirmedResign || 0, empIds: detail.confirmedResignEmpIds || [] };
        case 3:
            return { value: detail.transfer || 0, empIds: [...(detail.transferInEmpIds || []), ...(detail.transferOutEmpIds || [])] };
        case 4:
            return { value: detail.currentHeadCount || 0, empIds: detail.currentHeadCountEmpIds || [] };
        default:
            return { value: 0, empIds: [] };
    }
};

// Helper to group columns by label3
export const groupByLabel3 = (columns) => {
    const groups = [];
    let i = 0;
    while (i < columns.length) {
        const currentLabel1 = columns[i].label || '';
        const currentLabel2 = columns[i].label2 || '';
        const currentLabel3 = columns[i].label3 || '';
        let count = 1;
        while (i + count < columns.length && 
               (columns[i + count].label3 || '') === currentLabel3 &&
               (columns[i + count].label2 || '') === currentLabel2 &&
               (columns[i + count].label || '') === currentLabel1)
        {
            count++;
        }
        groups.push({ 
            label: currentLabel3, 
            colspan: count, 
            isGrandTotal: columns[i].isGrandTotal, 
            isUnitCategory: columns[i].isUnitCategory 
        });
        i += count;
    }
    return groups;
};

// Helper to group columns by label2
export const groupByLabel2 = (columns) => {
    const groups = [];
    let i = 0;

    while (i < columns.length) {
        const currentLabel1 = columns[i].label || '';
        const currentLabel2 = columns[i].label2 || '';
        let count = 1;
        while (
            i + count < columns.length &&
            columns[i + count].label2 === currentLabel2 &&
            columns[i + count].label === currentLabel1
        ) {
            count++;
        }
        if(!columns[i].isGrandTotal) { // to rowspan GRAND TOTAL
            groups.push({ 
                label: currentLabel2, 
                colspan: count, 
                isGrandTotal: columns[i].isGrandTotal, 
                isUnitCategory: columns[i].isUnitCategory 
            });
        }
        i += count;
        
    }
    return groups;
};

// Helper to group columns by label1 (simple)
export const groupByLabel1 = (columns) => {
    const groups = [];
    let i = 0;
    while (i < columns.length) {
        const currentLabel = columns[i].label || '';
        let count = 1;
        while (i + count < columns.length && 
               (columns[i + count].label || '') === currentLabel) {
            count++;
        }
        groups.push({ 
            label: currentLabel, 
            colspan: count, 
            isGrandTotal: columns[i].isGrandTotal, 
            isUnitCategory: columns[i].isUnitCategory 
        });
        i += count;
    }
    return groups;
};

