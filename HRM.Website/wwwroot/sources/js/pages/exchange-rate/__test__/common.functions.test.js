import { validationSchema } from '../common.functions';
import moment from 'moment';

describe('common.functions', () => {
    describe('validationSchema', () => {
        describe('currencyCode validation', () => {
            it('should validate correct currency code', async () => {
                const validData = {
                    currencyCode: 'EUR',
                    exchangeRate: 1.0,
                    effectiveDate: '2024-01-01'
                };

                await expect(validationSchema.validate(validData)).resolves.toEqual(validData);
            });

            it('should reject USD currency code', async () => {
                const invalidData = {
                    currencyCode: 'USD',
                    exchangeRate: 1.0,
                    effectiveDate: '2024-01-01'
                };

                await expect(validationSchema.validate(invalidData)).rejects.toThrow('Currency must be different from USD');
            });

            it('should reject empty currency code', async () => {
                const invalidData = {
                    currencyCode: '',
                    exchangeRate: 1.0,
                    effectiveDate: '2024-01-01'
                };

                await expect(validationSchema.validate(invalidData)).rejects.toThrow('Currency is required!');
            });

            it('should reject currency code with wrong length', async () => {
                const invalidData = {
                    currencyCode: 'US',
                    exchangeRate: 1.0,
                    effectiveDate: '2024-01-01'
                };

                await expect(validationSchema.validate(invalidData)).rejects.toThrow('Currency must be exactly 3 characters');
            });

            it('should trim whitespace from currency code', async () => {
                const data = {
                    currencyCode: '  EUR  ',
                    exchangeRate: 1.0,
                    effectiveDate: '2024-01-01'
                };

                const result = await validationSchema.validate(data);
                expect(result.currencyCode).toBe('EUR');
            });
        });

        describe('exchangeRate validation', () => {
            it('should validate positive exchange rate', async () => {
                const validData = {
                    currencyCode: 'EUR',
                    exchangeRate: 1.5,
                    effectiveDate: '2024-01-01'
                };

                await expect(validationSchema.validate(validData)).resolves.toEqual(validData);
            });

            it('should reject missing exchange rate', async () => {
                const invalidData = {
                    currencyCode: 'EUR',
                    exchangeRate: undefined,
                    effectiveDate: '2024-01-01'
                };

                await expect(validationSchema.validate(invalidData)).rejects.toThrow('Exchange Rate is required!');
            });

            it('should reject zero exchange rate', async () => {
                const invalidData = {
                    currencyCode: 'EUR',
                    exchangeRate: 0,
                    effectiveDate: '2024-01-01'
                };

                await expect(validationSchema.validate(invalidData)).rejects.toThrow('Exchange Rate must be greater than 0');
            });

            it('should reject negative exchange rate', async () => {
                const invalidData = {
                    currencyCode: 'EUR',
                    exchangeRate: -1,
                    effectiveDate: '2024-01-01'
                };

                await expect(validationSchema.validate(invalidData)).rejects.toThrow('Exchange Rate must be greater than 0');
            });
        });

        describe('effectiveDate validation', () => {
            it('should validate correct date format', async () => {
                const validData = {
                    currencyCode: 'EUR',
                    exchangeRate: 1.0,
                    effectiveDate: '2024-01-01'
                };

                await expect(validationSchema.validate(validData)).resolves.toEqual(validData);
            });

            it('should reject missing effective date', async () => {
                const invalidData = {
                    currencyCode: 'EUR',
                    exchangeRate: 1.0,
                    effectiveDate: undefined
                };

                await expect(validationSchema.validate(invalidData)).rejects.toThrow('Effective Date is required!');
            });

            it('should reject invalid date format', async () => {
                const invalidData = {
                    currencyCode: 'EUR',
                    exchangeRate: 1.0,
                    effectiveDate: '2024/01/01'
                };

                await expect(validationSchema.validate(invalidData)).rejects.toThrow('Effective Date must be a valid date');
            });

            it('should reject invalid date string', async () => {
                const invalidData = {
                    currencyCode: 'EUR',
                    exchangeRate: 1.0,
                    effectiveDate: 'invalid-date'
                };

                await expect(validationSchema.validate(invalidData)).rejects.toThrow('Effective Date must be a valid date');
            });

            it('should accept valid date in YYYY-MM-DD format', async () => {
                const validData = {
                    currencyCode: 'EUR',
                    exchangeRate: 1.0,
                    effectiveDate: '2024-12-31'
                };

                await expect(validationSchema.validate(validData)).resolves.toEqual(validData);
            });
        });

        describe('combined validation', () => {
            it('should validate all fields correctly', async () => {
                const validData = {
                    currencyCode: 'EUR',
                    exchangeRate: 1.1,
                    effectiveDate: '2024-06-15'
                };

                await expect(validationSchema.validate(validData)).resolves.toEqual(validData);
            });

            it('should reject when all fields are invalid', async () => {
                const invalidData = {
                    currencyCode: '',
                    exchangeRate: -1,
                    effectiveDate: 'invalid'
                };

                try {
                    await validationSchema.validate(invalidData, { abortEarly: false });
                } catch (error) {
                    expect(error.inner).toBeDefined();
                    expect(error.inner.length).toBeGreaterThan(0);
                }
            });
        });
    });
});


