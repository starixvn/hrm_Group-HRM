import PayrollService from '../service';
import axios from 'axios';
import toastr from 'toastr';

jest.mock('axios');
jest.mock('toastr');
jest.mock('../../../shared/common.functions', () => ({
    blockUI: jest.fn(),
    unblockUI: jest.fn(),
    resolveClientUrl: jest.fn(url => `/api/${url}`),
    toastOptions: jest.fn(() => ({ timeout: 3000 })),
}));

describe('PayrollService', () => {
    const mockAxios = axios;
    const mockToastr = toastr;
    const mockCommonFunctions = require('../../../shared/common.functions');

    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('getSalaryData', () => {
        it('should fetch salary data successfully', async () => {
            const mockData = [{ id: 1, name: 'Test Employee' }];
            const mockResponse = { data: mockData };
            mockAxios.mockResolvedValue(mockResponse);

            const filters = { month: '2024-01-01', isLocal: true };
            const result = await PayrollService.getSalaryData(filters);

            expect(mockCommonFunctions.blockUI).toHaveBeenCalled();
            expect(mockAxios).toHaveBeenCalledWith({
                url: '/api/PayrollManagement/GetPayrollData',
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    month: '2024-01-01',
                    isLocal: true,
                },
                responseType: 'json',
            });
            expect(result).toEqual({data: mockData});
            expect(mockCommonFunctions.unblockUI).toHaveBeenCalled();
        });

        it('should handle empty filters with default values', async () => {
            const mockData = [];
            const mockResponse = { data: mockData };
            mockAxios.mockResolvedValue(mockResponse);

            const result = await PayrollService.getSalaryData();

            expect(mockAxios).toHaveBeenCalledWith({
                url: '/api/PayrollManagement/GetPayrollData',
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    month: null,
                    isLocal: false,
                },
                responseType: 'json',
            });
            expect(result).toEqual(mockData);
        });

        it('should call unblockUI even when error occurs', async () => {
            mockAxios.mockRejectedValue(new Error('API Error'));

            try {
                await PayrollService.getSalaryData();
            } catch (error) {
                // Expected to throw
            }

            expect(mockCommonFunctions.unblockUI).toHaveBeenCalled();
        });
    });

    describe('generatePayroll', () => {
        it('should generate payroll successfully', async () => {
            const mockData = { success: true, message: 'Generated' };
            const mockResponse = { data: mockData };
            mockAxios.mockResolvedValue(mockResponse);

            const month = '2024-01-01';
            const result = await PayrollService.generatePayroll(month);

            expect(mockCommonFunctions.blockUI).toHaveBeenCalled();
            expect(mockAxios).toHaveBeenCalledWith({
                url: '/api/PayrollManagement/GeneratePayroll',
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    month: '2024-01-01',
                },
                responseType: 'json',
            });
            expect(result).toEqual(mockData);
            expect(mockCommonFunctions.unblockUI).toHaveBeenCalled();
        });

        it('should handle null month parameter', async () => {
            const mockData = { success: true };
            const mockResponse = { data: mockData };
            mockAxios.mockResolvedValue(mockResponse);

            const result = await PayrollService.generatePayroll();

            expect(mockAxios).toHaveBeenCalledWith({
                url: '/api/PayrollManagement/GeneratePayroll',
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    month: null,
                },
                responseType: 'json',
            });
            expect(result).toEqual(mockData);
        });

        it('should ensure unblockUI is called even on error', async () => {
            mockAxios.mockRejectedValue(new Error('Server Error'));

            try {
                await PayrollService.generatePayroll('2024-02');
            } catch (error) {
                // Expected to throw
            }

            expect(mockCommonFunctions.unblockUI).toHaveBeenCalled();
        });
    });

    describe('resolveClientUrl usage', () => {
        it('should use resolveClientUrl for getSalaryData', async () => {
            mockAxios.mockResolvedValue({ data: [] });

            await PayrollService.getSalaryData();

            expect(mockCommonFunctions.resolveClientUrl).toHaveBeenCalledWith(
                'PayrollManagement/GetPayrollData'
            );
        });

        it('should use resolveClientUrl for generatePayroll', async () => {
            mockAxios.mockResolvedValue({ data: {} });

            await PayrollService.generatePayroll();

            expect(mockCommonFunctions.resolveClientUrl).toHaveBeenCalledWith(
                'PayrollManagement/GeneratePayroll'
            );
        });
    });
});