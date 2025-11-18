import axios from 'axios';
import ExchangeRateService from '../service';
import { blockUI, unblockUI, resolveClientUrl } from '../../../shared/common.functions';

jest.mock('axios');
jest.mock('../../../shared/common.functions', () => ({
    blockUI: jest.fn(),
    unblockUI: jest.fn(),
    resolveClientUrl: jest.fn((url) => `/test${url}`)
}));

describe('ExchangeRateService', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('getExchangeRates', () => {
        it('should call blockUI and unblockUI', async () => {
            axios.mockResolvedValue({ data: [] });

            await ExchangeRateService.getExchangeRates();

            expect(blockUI).toHaveBeenCalled();
            expect(unblockUI).toHaveBeenCalled();
        });

        it('should return exchange rates data', async () => {
            const mockData = [{ currencyId: 1, baseCurrency: 'USD' }];
            axios.mockResolvedValue({ data: mockData });

            const result = await ExchangeRateService.getExchangeRates();

            expect(result).toEqual(mockData);
            expect(axios).toHaveBeenCalledWith({
                url: '/testExchangeRate/GetExchangeRates',
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'json',
            });
        });

        it('should call unblockUI even when error occurs', async () => {
            axios.mockRejectedValue(new Error('Network error'));

            try {
                await ExchangeRateService.getExchangeRates();
            } catch (error) {
                expect(unblockUI).toHaveBeenCalled();
            }
        });
    });

    describe('createExchangeRate', () => {
        it('should call blockUI and unblockUI', async () => {
            const mockData = { currencyCode: 'USD', exchangeRate: 1.0 };
            axios.mockResolvedValue({ data: { code: 0 } });

            await ExchangeRateService.createExchangeRate(mockData);

            expect(blockUI).toHaveBeenCalled();
            expect(unblockUI).toHaveBeenCalled();
        });

        it('should send POST request with correct data', async () => {
            const mockData = {
                currencyCode: 'USD',
                exchangeRate: 1.0,
                effectiveDate: '2024-01-01',
                rounding: 0
            };
            axios.mockResolvedValue({ data: { code: 0 } });

            await ExchangeRateService.createExchangeRate(mockData);

            expect(axios).toHaveBeenCalledWith({
                url: '/testExchangeRate/Create',
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                data: mockData,
                responseType: 'json',
            });
        });

        it('should call unblockUI even when error occurs', async () => {
            axios.mockRejectedValue(new Error('Network error'));

            try {
                await ExchangeRateService.createExchangeRate({});
            } catch (error) {
                expect(unblockUI).toHaveBeenCalled();
            }
        });
    });

    describe('updateExchangeRate', () => {
        it('should call blockUI and unblockUI', async () => {
            const currencyId = 1;
            const mockData = { currencyCode: 'USD', exchangeRate: 1.0 };
            axios.mockResolvedValue({ data: { code: 0 } });

            await ExchangeRateService.updateExchangeRate(currencyId, mockData);

            expect(blockUI).toHaveBeenCalled();
            expect(unblockUI).toHaveBeenCalled();
        });

        it('should send POST request with correct data structure', async () => {
            const currencyId = 1;
            const mockData = {
                currencyCode: 'USD',
                exchangeRate: 1.0,
                effectiveDate: '2024-01-01',
                oldEffectiveDate: '2024-01-01',
                rounding: 0
            };
            axios.mockResolvedValue({ data: { code: 0 } });

            await ExchangeRateService.updateExchangeRate(currencyId, mockData);

            expect(axios).toHaveBeenCalledWith({
                url: '/testExchangeRate/Update',
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                data: {
                    currencyId: currencyId,
                    currencyCode: mockData.currencyCode,
                    exchangeRate: mockData.exchangeRate,
                    effectiveDate: mockData.effectiveDate,
                    oldEffectiveDate: mockData.oldEffectiveDate,
                    rounding: mockData.rounding,
                },
                responseType: 'json',
            });
        });

        it('should call unblockUI even when error occurs', async () => {
            axios.mockRejectedValue(new Error('Network error'));

            try {
                await ExchangeRateService.updateExchangeRate(1, {});
            } catch (error) {
                expect(unblockUI).toHaveBeenCalled();
            }
        });
    });

    describe('deleteExchangeRate', () => {
        it('should call blockUI and unblockUI', async () => {
            const currencyId = 1;
            const effectiveDate = '2024-01-01';
            axios.mockResolvedValue({ data: { code: 0 } });

            await ExchangeRateService.deleteExchangeRate(currencyId, effectiveDate);

            expect(blockUI).toHaveBeenCalled();
            expect(unblockUI).toHaveBeenCalled();
        });

        it('should send POST request with correct data', async () => {
            const currencyId = 1;
            const effectiveDate = '2024-01-01';
            axios.mockResolvedValue({ data: { code: 0 } });

            await ExchangeRateService.deleteExchangeRate(currencyId, effectiveDate);

            expect(axios).toHaveBeenCalledWith({
                url: '/testExchangeRate/Delete',
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                data: {
                    currencyId: currencyId,
                    effectiveDate: effectiveDate
                },
                responseType: 'json',
            });
        });

        it('should call unblockUI even when error occurs', async () => {
            axios.mockRejectedValue(new Error('Network error'));

            try {
                await ExchangeRateService.deleteExchangeRate(1, '2024-01-01');
            } catch (error) {
                expect(unblockUI).toHaveBeenCalled();
            }
        });
    });

    describe('getRoundingRules', () => {
        it('should return rounding rules data', async () => {
            const mockData = [{ currency: 'USD', rounding: 0 }];
            axios.mockResolvedValue({ data: mockData });

            const result = await ExchangeRateService.getRoundingRules();

            expect(result).toEqual(mockData);
            expect(axios).toHaveBeenCalledWith({
                url: '/testExchangeRate/GetRoundingRules',
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'json',
            });
        });

        it('should return empty array when data is null', async () => {
            axios.mockResolvedValue({ data: null });

            const result = await ExchangeRateService.getRoundingRules();

            expect(result).toEqual([]);
        });

        it('should return empty array when data is undefined', async () => {
            axios.mockResolvedValue({ data: undefined });

            const result = await ExchangeRateService.getRoundingRules();

            expect(result).toEqual([]);
        });

        it('should not call blockUI or unblockUI', async () => {
            axios.mockResolvedValue({ data: [] });

            await ExchangeRateService.getRoundingRules();

            expect(blockUI).not.toHaveBeenCalled();
            expect(unblockUI).not.toHaveBeenCalled();
        });
    });
});


