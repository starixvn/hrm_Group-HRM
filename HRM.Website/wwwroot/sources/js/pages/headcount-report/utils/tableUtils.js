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

// Helper to get unique columns by label1 (only one column per label1)
export const getUniqueColumnsByLabel1 = (data) => {
    const uniqueMap = new Map();
    const uniqueData = [];
    const uniqueColumns = [];
    
    data.forEach((item, index) => {
        const label1 = item.label1 || '';
        if (!uniqueMap.has(label1)) {
            uniqueMap.set(label1, true);
            uniqueData.push(item);
            uniqueColumns.push({
                label: item.label1 || '',
                label2: item.label2 || '',
                label3: item.label3 || '',
                key: item.label1 || '',
                isUnitCategory: item.isUnitCategory,
                isGrandTotal: item.isGrandTotal,
                originalIndices: [index], // Store original indices for data aggregation
                allData: data // Store reference to all data for aggregation
            });
        } else {
            // Add index to existing column's originalIndices for aggregation
            const existingCol = uniqueColumns.find(col => col.label === label1);
            if (existingCol) {
                existingCol.originalIndices.push(index);
            }
        }
    });
    
    return { columns: uniqueColumns, data: uniqueData };
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

// Helper to get unique label3 values from columns
export const getUniqueLabel3Values = (columns) => {
    const seen = new Set();

    const uniqueLabels = columns
        .filter(col => col.label3 && !seen.has(col.label3) && seen.add(col.label3))
        .map(col => col.label3)
        .sort((a, b) => a.localeCompare(b, undefined, { sensitivity: 'base' }));

    return uniqueLabels;
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

// Helper to get total value for a label3 by month
export const getValueByLabel3 = (column, month, label3) => {
    let total = 0;
    const allEmpIds = [];
    
    column.originalIndices.forEach(idx => {
        const dataItem = column.allData[idx];
        if (dataItem && dataItem.label3 === label3) {
            const details = dataItem.details;
            const detail = details
                ? details.find(d => d.month === month + 1)
                : null;
                
            if (detail) {
                total += (detail.currentHeadCount || 0);
                if (detail.currentHeadCountEmpIds) {
                    allEmpIds.push(...detail.currentHeadCountEmpIds);
                }
            }
        }
    });
    
    return { value: total, empIds: allEmpIds };
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

