import { validationSchema, submitTransfer } from '../common.functions';
import axios from 'axios';
import toastr from 'toastr';

jest.mock('axios');
jest.mock('toastr');
jest.mock('../../../shared/common.functions', () => ({
    resolveClientUrl: jest.fn(url => `/api/${url}`),
    axiosError: jest.fn(),
    toastOptions: jest.fn(() => ({ timeout: 3000 })),
}));

describe('Transfer Common Functions', () => {
    const mockAxios = axios;
    const mockToastr = toastr;
    const mockCommonFunctions = require('../../../shared/common.functions');

    beforeEach(() => {
        jest.clearAllMocks();
        jest.useFakeTimers();
    });

    afterEach(() => {
        jest.runOnlyPendingTimers();
        jest.useRealTimers();
    });

    describe('validationSchema', () => {
        it('should validate FullName is required', async () => {
            const invalidData = { Code: '12345', FullName: '' };

            await expect(validationSchema.validate(invalidData)).rejects.toThrow(
                'Full Name field is required!'
            );
        });

        it('should validate Code is required', async () => {
            const invalidData = { Code: '', FullName: 'John Doe' };

            await expect(validationSchema.validate(invalidData)).rejects.toThrow(
                'Employee ID field is required!'
            );
        });

        it('should pass validation with valid data', async () => {
            const validData = { Code: '12345', FullName: 'John Doe' };

            await expect(validationSchema.validate(validData)).resolves.toEqual(
                validData
            );
        });
    });

    describe('submitTransfer', () => {
        const mockSetSubmitting = jest.fn();

        beforeEach(() => {
            mockSetSubmitting.mockClear();
            delete window.location;
            window.location = { href: jest.fn() };
        });

        it('should submit transfer successfully without redirect', async () => {
            const mockData = { isSuccessful: true };
            const mockResponse = { data: mockData };
            mockAxios.post.mockResolvedValue(mockResponse);

            const model = { id: 1, fullName: 'Test Employee' };
            const endpoint = 'GroupEmployeeList/Transfer';

            submitTransfer(endpoint, model, mockSetSubmitting);

            await new Promise(process.nextTick);

            expect(mockSetSubmitting).toHaveBeenCalledWith(true);
            expect(mockCommonFunctions.resolveClientUrl).toHaveBeenCalledWith(
                endpoint
            );
            expect(mockAxios.post).toHaveBeenCalledWith(
                `/api/${endpoint}`,
                model,
                {
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'Content-Type': 'application/json',
                    },
                }
            );
            expect(mockToastr.success).toHaveBeenCalledWith(
                'Transfer successfully!',
                '',
                { timeout: 3000 }
            );
            expect(mockSetSubmitting).toHaveBeenCalledWith(false);
        });

        it('should submit transfer successfully with redirect', async () => {
            const mockData = { isSuccessful: true };
            const mockResponse = { data: mockData };
            mockAxios.post.mockResolvedValue(mockResponse);

            const model = { id: 1, fullName: 'Test Employee' };
            const endpoint = 'GroupEmployeeList/Transfer';
            const redirectUrl = 'GroupEmployeeList/Index';

            submitTransfer(endpoint, model, mockSetSubmitting, redirectUrl);

            await new Promise(process.nextTick);

            expect(mockToastr.success).toHaveBeenCalledWith(
                'Transfer successfully!',
                '',
                { timeout: 3000 }
            );

            jest.advanceTimersByTime(3000);

            expect(window.location.href).toBe(`/api/${redirectUrl}`);
        });

        it('should handle unsuccessful response', async () => {
            const mockData = { isSuccessful: false, message: 'Transfer failed' };
            const mockResponse = { data: mockData };
            mockAxios.post.mockResolvedValue(mockResponse);

            const model = { id: 1, fullName: 'Test Employee' };
            const endpoint = 'GroupEmployeeList/Transfer';

            submitTransfer(endpoint, model, mockSetSubmitting);

            await new Promise(process.nextTick);

            expect(mockToastr.error).toHaveBeenCalledWith(
                'Transfer failed',
                '',
                { timeout: 3000 }
            );
            expect(mockSetSubmitting).toHaveBeenCalledWith(false);
        });

        it('should handle API error', async () => {
            mockAxios.post.mockRejectedValue(new Error('API Error'));

            const model = { id: 1, fullName: 'Test Employee' };
            const endpoint = 'GroupEmployeeList/Transfer';

            submitTransfer(endpoint, model, mockSetSubmitting);

            await new Promise(process.nextTick);

            expect(mockCommonFunctions.axiosError).toHaveBeenCalled();
        });
    });
});

