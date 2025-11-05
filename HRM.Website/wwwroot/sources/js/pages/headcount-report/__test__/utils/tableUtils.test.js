import {
    monthNames,
    categories,
    processColumns,
    getUniqueColumnsByLabel1,
    getMaxMonth,
    getUniqueLabel3Values,
    getValue,
    getValueByLabel3,
    groupByLabel1,
    groupByLabel2,
    groupByLabel3
} from '../../utils/tableUtils';

describe('tableUtils', () => {
    describe('monthNames', () => {
        it('should have 12 months', () => {
            expect(monthNames.length).toBe(12);
        });

        it('should contain correct month abbreviations', () => {
            expect(monthNames[0]).toBe('Jan');
            expect(monthNames[11]).toBe('Dec');
        });
    });

    describe('categories', () => {
        it('should have 5 categories', () => {
            expect(categories.length).toBe(5);
        });

        it('should contain correct category names', () => {
            expect(categories).toContain('New Hire');
            expect(categories).toContain('Probation Resign');
            expect(categories).toContain('Confirmed Resign');
            expect(categories).toContain('Transfer');
            expect(categories).toContain('Current H/C');
        });
    });

    describe('processColumns', () => {
        it('should process columns correctly', () => {
            const data = [
                {
                    label1: 'Unit 1',
                    label2: 'Company 1',
                    label3: 'Category A',
                    isUnitCategory: false,
                    isGrandTotal: true
                }
            ];

            const result = processColumns(data);

            expect(result).toHaveLength(1);
            expect(result[0].label).toBe('Unit 1');
            expect(result[0].label2).toBe('Company 1');
            expect(result[0].label3).toBe('Category A');
            expect(result[0].key).toBe('Unit 1');
            expect(result[0].isUnitCategory).toBe(false);
            expect(result[0].isGrandTotal).toBe(true);
        });

        it('should handle missing properties', () => {
            const data = [{}];

            const result = processColumns(data);

            expect(result).toHaveLength(1);
            expect(result[0].label).toBe('');
            expect(result[0].label2).toBe('');
            expect(result[0].label3).toBe('');
        });
    });

    describe('getUniqueColumnsByLabel1', () => {
        it('should return unique columns by label1', () => {
            const data = [
                { label1: 'Unit 1', label2: 'Company 1' },
                { label1: 'Unit 1', label2: 'Company 2' },
                { label1: 'Unit 2', label2: 'Company 1' }
            ];

            const result = getUniqueColumnsByLabel1(data);

            expect(result.columns).toHaveLength(2);
            expect(result.columns[0].label).toBe('Unit 1');
            expect(result.columns[1].label).toBe('Unit 2');
        });

        it('should store original indices for aggregation', () => {
            const data = [
                { label1: 'Unit 1' },
                { label1: 'Unit 1' },
                { label1: 'Unit 2' }
            ];

            const result = getUniqueColumnsByLabel1(data);

            expect(result.columns[0].originalIndices).toEqual([0, 1]);
            expect(result.columns[1].originalIndices).toEqual([2]);
        });
    });

    describe('getMaxMonth', () => {
        it('should return max month from data', () => {
            const data = [
                {
                    details: [
                        { month: 1 },
                        { month: 5 },
                        { month: 3 }
                    ]
                },
                {
                    details: [
                        { month: 2 },
                        { month: 12 }
                    ]
                }
            ];

            const result = getMaxMonth(data);

            expect(result).toBe(12);
        });

        it('should return 0 for empty data', () => {
            const result = getMaxMonth([]);
            expect(result).toBe(0);
        });

        it('should handle data without details', () => {
            const data = [{ label1: 'Unit 1' }];
            const result = getMaxMonth(data);
            expect(result).toBe(0);
        });
    });

    describe('getUniqueLabel3Values', () => {
        it('should return unique label3 values sorted', () => {
            const columns = [
                { label3: 'Category B' },
                { label3: 'Category A' },
                { label3: 'Category B' },
                { label3: 'Category C' }
            ];

            const result = getUniqueLabel3Values(columns);

            expect(result).toEqual(['Category A', 'Category B', 'Category C']);
        });

        it('should handle columns without label3', () => {
            const columns = [
                { label3: 'Category A' },
                { label3: null },
                { label3: undefined }
            ];

            const result = getUniqueLabel3Values(columns);

            expect(result).toEqual(['Category A']);
        });
    });

    describe('getValue', () => {
        it('should return newHire value', () => {
            const unitData = {
                details: [{
                    month: 1,
                    newHire: 5,
                    newHireEmpIds: [1, 2, 3, 4, 5]
                }]
            };

            const result = getValue(unitData, 0, 0);

            expect(result.value).toBe(5);
            expect(result.empIds).toEqual([1, 2, 3, 4, 5]);
        });

        it('should return probationResign value', () => {
            const unitData = {
                details: [{
                    month: 1,
                    probationResign: 2,
                    probationResignEmpIds: [6, 7]
                }]
            };

            const result = getValue(unitData, 0, 1);

            expect(result.value).toBe(2);
            expect(result.empIds).toEqual([6, 7]);
        });

        it('should return confirmedResign value', () => {
            const unitData = {
                details: [{
                    month: 1,
                    confirmedResign: 1,
                    confirmedResignEmpIds: [8]
                }]
            };

            const result = getValue(unitData, 0, 2);

            expect(result.value).toBe(1);
            expect(result.empIds).toEqual([8]);
        });

        it('should return transfer value with combined empIds', () => {
            const unitData = {
                details: [{
                    month: 1,
                    transfer: 2,
                    transferInEmpIds: [9],
                    transferOutEmpIds: [10]
                }]
            };

            const result = getValue(unitData, 0, 3);

            expect(result.value).toBe(2);
            expect(result.empIds).toEqual([9, 10]);
        });

        it('should return currentHeadCount value', () => {
            const unitData = {
                details: [{
                    month: 1,
                    currentHeadCount: 10,
                    currentHeadCountEmpIds: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
                }]
            };

            const result = getValue(unitData, 0, 4);

            expect(result.value).toBe(10);
            expect(result.empIds).toHaveLength(10);
        });

        it('should return default value for missing detail', () => {
            const unitData = {
                details: []
            };

            const result = getValue(unitData, 0, 0);

            expect(result.value).toBe(0);
            expect(result.empIds).toEqual([]);
        });
    });

    describe('getValueByLabel3', () => {
        it('should aggregate values by label3', () => {
            const allData = [
                {
                    label3: 'Category A',
                    details: [{
                        month: 1,
                        currentHeadCount: 5,
                        currentHeadCountEmpIds: [1, 2, 3, 4, 5]
                    }]
                },
                {
                    label3: 'Category A',
                    details: [{
                        month: 1,
                        currentHeadCount: 3,
                        currentHeadCountEmpIds: [6, 7, 8]
                    }]
                }
            ];

            const column = {
                label3: 'Category A',
                originalIndices: [0, 1]
            };

            const result = getValueByLabel3(allData, column, 0, 'Category A');

            expect(result.value).toBe(8);
            expect(result.empIds).toHaveLength(8);
        });

        it('should return zero when no matching data', () => {
            const allData = [
                {
                    label3: 'Category B',
                    details: [{ month: 1, currentHeadCount: 5 }]
                }
            ];

            const column = {
                label3: 'Category A',
                originalIndices: [0]
            };

            const result = getValueByLabel3(allData, column, 0, 'Category A');

            expect(result.value).toBe(0);
            expect(result.empIds).toEqual([]);
        });
    });

    describe('groupByLabel1', () => {
        it('should group columns by label1', () => {
            const columns = [
                { label: 'Unit 1', isGrandTotal: false, isUnitCategory: false },
                { label: 'Unit 1', isGrandTotal: false, isUnitCategory: false },
                { label: 'Unit 2', isGrandTotal: false, isUnitCategory: false }
            ];

            const result = groupByLabel1(columns);

            expect(result).toHaveLength(2);
            expect(result[0].label).toBe('Unit 1');
            expect(result[0].colspan).toBe(2);
            expect(result[1].label).toBe('Unit 2');
            expect(result[1].colspan).toBe(1);
        });

        it('should preserve isGrandTotal and isUnitCategory flags', () => {
            const columns = [
                { label: 'Unit 1', isGrandTotal: true, isUnitCategory: false }
            ];

            const result = groupByLabel1(columns);

            expect(result[0].isGrandTotal).toBe(true);
            expect(result[0].isUnitCategory).toBe(false);
        });
    });

    describe('groupByLabel2', () => {
        it('should group columns by label2', () => {
            const columns = [
                { label: 'Unit 1', label2: 'Company 1', isGrandTotal: false },
                { label: 'Unit 1', label2: 'Company 1', isGrandTotal: false },
                { label: 'Unit 1', label2: 'Company 2', isGrandTotal: false }
            ];

            const result = groupByLabel2(columns);

            expect(result).toHaveLength(2);
            expect(result[0].label).toBe('Company 1');
            expect(result[0].colspan).toBe(2);
            expect(result[1].label).toBe('Company 2');
            expect(result[1].colspan).toBe(1);
        });

        it('should skip grand total columns', () => {
            const columns = [
                { label: 'Unit 1', label2: 'Company 1', isGrandTotal: true }
            ];

            const result = groupByLabel2(columns);

            expect(result).toHaveLength(0);
        });
    });

    describe('groupByLabel3', () => {
        it('should group columns by label3', () => {
            const columns = [
                { label: 'Unit 1', label2: 'Company 1', label3: 'Category A', isGrandTotal: false },
                { label: 'Unit 1', label2: 'Company 1', label3: 'Category A', isGrandTotal: false },
                { label: 'Unit 1', label2: 'Company 1', label3: 'Category B', isGrandTotal: false }
            ];

            const result = groupByLabel3(columns);

            expect(result).toHaveLength(2);
            expect(result[0].label).toBe('Category A');
            expect(result[0].colspan).toBe(2);
            expect(result[1].label).toBe('Category B');
            expect(result[1].colspan).toBe(1);
        });

        it('should only group when all labels match', () => {
            const columns = [
                { label: 'Unit 1', label2: 'Company 1', label3: 'Category A' },
                { label: 'Unit 2', label2: 'Company 1', label3: 'Category A' }
            ];

            const result = groupByLabel3(columns);

            expect(result).toHaveLength(2);
            expect(result[0].colspan).toBe(1);
            expect(result[1].colspan).toBe(1);
        });
    });
});

