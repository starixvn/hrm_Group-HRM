import { validationSchema } from '../common.functions';

describe('Salary Structure Validation Schema', () => {
    describe('name field', () => {
        it('should require name field', async () => {
            try {
                await validationSchema.validate({});
                fail('Should have thrown validation error');
            } catch (error) {
                expect(error.message).toContain('Local Salary or Expat Salary is required');
            }
        });

        it('should pass validation with valid name', async () => {
            const validData = {
                name: 'Test Salary Structure',
                jobLevelId: 1,
                basicSalary: 1000
            };
            
            const result = await validationSchema.validate(validData);
            expect(result.name).toBe('Test Salary Structure');
        });

        it('should trim whitespace from name', async () => {
            const dataWithWhitespace = {
                name: '  Test Structure  ',
                jobLevelId: 1,
                basicSalary: 1000
            };
            
            const result = await validationSchema.validate(dataWithWhitespace);
            expect(result.name).toBe('Test Structure');
        });

        it('should fail validation with empty name', async () => {
            const invalidData = {
                name: '',
                jobLevelId: 1,
                basicSalary: 1000
            };
            
            await expect(validationSchema.validate(invalidData))
                .rejects.toThrow('Salary Structure field is required!');
        });

        it('should fail validation with whitespace-only name', async () => {
            const invalidData = {
                name: '   ',
                jobLevelId: 1,
                basicSalary: 1000
            };
            
            await expect(validationSchema.validate(invalidData))
                .rejects.toThrow('Salary Structure field is required!');
        });
    });

    describe('jobLevelId field', () => {
        it('should require jobLevelId field', async () => {
            const invalidData = {
                name: 'Test Structure',
                basicSalary: 1000
            };
            
            await expect(validationSchema.validate(invalidData))
                .rejects.toThrow('Job Level field is required !');
        });

        it('should pass validation with valid jobLevelId', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: 1000
            };
            
            const result = await validationSchema.validate(validData);
            expect(result.jobLevelId).toBe(1);
        });

        it('should fail validation with null jobLevelId', async () => {
            const invalidData = {
                name: 'Test Structure',
                jobLevelId: null,
                basicSalary: 1000
            };
            
            await expect(validationSchema.validate(invalidData))
                .rejects.toThrow('Job Level field is required !');
        });

        it('should fail validation with string jobLevelId', async () => {
            const invalidData = {
                name: 'Test Structure',
                jobLevelId: 'invalid',
                basicSalary: 1000
            };
            
            await expect(validationSchema.validate(invalidData))
                .rejects.toThrow('Job Level field is required !');
        });
    });

    describe('basicSalary field', () => {
        it('should allow null basicSalary', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: null,
                expatSalary: 1000
            };
            
            const result = await validationSchema.validate(validData);
            expect(result.basicSalary).toBeNull();
        });

        it('should transform empty string to null for basicSalary', async () => {
            const dataWithEmptyString = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: '',
                expatSalary: 1000
            };
            
            const result = await validationSchema.validate(dataWithEmptyString);
            expect(result.basicSalary).toBeNull();
        });

        it('should pass validation with valid basicSalary', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: 1000
            };
            
            const result = await validationSchema.validate(validData);
            expect(result.basicSalary).toBe(1000);
        });
    });

    describe('expatSalary field', () => {
        it('should allow null expatSalary', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: 1000,
                expatSalary: null
            };
            
            const result = await validationSchema.validate(validData);
            expect(result.expatSalary).toBeNull();
        });

        it('should transform empty string to null for expatSalary', async () => {
            const dataWithEmptyString = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: 1000,
                expatSalary: ''
            };
            
            const result = await validationSchema.validate(dataWithEmptyString);
            expect(result.expatSalary).toBeNull();
        });

        it('should pass validation with valid expatSalary', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                expatSalary: 2000
            };
            
            const result = await validationSchema.validate(validData);
            expect(result.expatSalary).toBe(2000);
        });
    });

    describe('at-least-one-salary validation', () => {
        it('should pass validation when basicSalary is provided', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: 1000,
                expatSalary: null
            };
            
            const result = await validationSchema.validate(validData);
            expect(result).toBeDefined();
        });

        it('should pass validation when expatSalary is provided', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: null,
                expatSalary: 2000
            };
            
            const result = await validationSchema.validate(validData);
            expect(result).toBeDefined();
        });

        it('should pass validation when both salaries are provided', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: 1000,
                expatSalary: 2000
            };
            
            const result = await validationSchema.validate(validData);
            expect(result).toBeDefined();
        });

        it('should fail validation when neither salary is provided', async () => {
            const invalidData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: null,
                expatSalary: null
            };
            
            await expect(validationSchema.validate(invalidData))
                .rejects.toThrow('Local Salary or Expat Salary is required !');
        });

        it('should fail validation when both salaries are empty strings', async () => {
            const invalidData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: '',
                expatSalary: ''
            };
            
            await expect(validationSchema.validate(invalidData))
                .rejects.toThrow('Local Salary or Expat Salary is required !');
        });

        it('should fail validation when both salaries are NaN', async () => {
            const invalidData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: NaN,
                expatSalary: NaN
            };
            
            await expect(validationSchema.validate(invalidData))
                .rejects.toThrow('Local Salary or Expat Salary is required !');
        });

        it('should pass validation when basicSalary is 0', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: 0,
                expatSalary: null
            };
            
            const result = await validationSchema.validate(validData);
            expect(result).toBeDefined();
        });

        it('should pass validation when expatSalary is 0', async () => {
            const validData = {
                name: 'Test Structure',
                jobLevelId: 1,
                basicSalary: null,
                expatSalary: 0
            };
            
            const result = await validationSchema.validate(validData);
            expect(result).toBeDefined();
        });
    });

    describe('complete validation scenarios', () => {
        it('should pass validation with complete valid data', async () => {
            const completeData = {
                name: 'Complete Salary Structure',
                jobLevelId: 5,
                basicSalary: 1500,
                expatSalary: 2500
            };
            
            const result = await validationSchema.validate(completeData);
            expect(result.name).toBe('Complete Salary Structure');
            expect(result.jobLevelId).toBe(5);
            expect(result.basicSalary).toBe(1500);
            expect(result.expatSalary).toBe(2500);
        });

        it('should pass validation with minimal required data', async () => {
            const minimalData = {
                name: 'Minimal Structure',
                jobLevelId: 1,
                basicSalary: 1000
            };
            
            const result = await validationSchema.validate(minimalData);
            expect(result.name).toBe('Minimal Structure');
            expect(result.jobLevelId).toBe(1);
            expect(result.basicSalary).toBe(1000);
        });
    });
});
