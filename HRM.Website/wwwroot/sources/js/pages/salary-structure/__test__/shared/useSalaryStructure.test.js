import React from 'react';
import { render, screen, act, fireEvent, waitFor } from '@testing-library/react';
import axios from 'axios';
import toastr from 'toastr';
import useSalaryStructure from '../../shared/useSalaryStructure';
import { axiosError } from '../../../../shared/common.functions';

jest.mock('axios');
jest.mock('toastr');
jest.mock('../../../../shared/common.functions', () => ({
    resolveClientUrl: jest.fn((url) => url),
    axiosError: jest.fn(),
    toastOptions: jest.fn(() => ({}))
}));

function TestComponent({ isEdit = false }) {
    const { jobLevels, initialValues, handleChangeSalary, validateFormValues, handleSubmit } =
        useSalaryStructure(isEdit);

    return (
        <div>
            <div data-testid="jobLevels">{JSON.stringify(jobLevels)}</div>
            <div data-testid="initialValues">{JSON.stringify(initialValues)}</div>
            <button onClick={() => handleChangeSalary(1, 'basicSalary', '5000')}>Change Salary</button>
            <button
                onClick={() =>
                    handleSubmit(
                        { name: 'Dev', salaryStructureId: '123' },
                        { setSubmitting: jest.fn() }
                    )
                }
            >
                Submit
            </button>
            <button onClick={() => validateFormValues({ name: '' })}>Validate</button>
        </div>
    );
}

describe('useSalaryStructure', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('should load job levels (create mode)', async () => {
        axios.mockResolvedValueOnce({
            data: [
                { id: 1, name: 'Junior' },
                { id: 2, name: 'Senior' }
            ]
        });

        await act(async () => {
            render(<TestComponent />);
        });

        await waitFor(() => {
            expect(axios).toHaveBeenCalledWith(expect.objectContaining({
                url: 'SalaryStructure/GetJobLevels'
            }));
        });

        const jobLevels = JSON.parse(screen.getByTestId('jobLevels').textContent);
        expect(jobLevels.length).toBe(2);
        expect(jobLevels[0].basicSalary).toBe('');
    });

    it('should load job levels (edit mode)', async () => {
        window.salaryStructureId = 999;
        axios.mockResolvedValueOnce({
            data: {
                salaryStructureId: 999,
                name: 'Structure A',
                details: [
                    { jobLevelId: 1, name: 'Junior', basicSalary: 1000, expatSalary: 2000, jobLevelAddOn: 'A' }
                ]
            }
        });

        await act(async () => {
            render(<TestComponent isEdit={true} />);
        });

        await waitFor(() => {
            expect(axios).toHaveBeenCalledWith(expect.objectContaining({
                url: 'salarystructure/getById/999'
            }));
        });

        const jobLevels = JSON.parse(screen.getByTestId('jobLevels').textContent);
        expect(jobLevels[0].name).toBe('Junior');
        const initialValues = JSON.parse(screen.getByTestId('initialValues').textContent);
        expect(initialValues.name).toBe('Structure A');
    });

    it('should handle salary change', async () => {
        axios.mockResolvedValueOnce({ data: [{ id: 1, name: 'Junior' }] });
        render(<TestComponent />);

        await waitFor(() => {
            expect(axios).toHaveBeenCalled();
        });

        await act(async () => {
            fireEvent.click(screen.getByText('Change Salary'));
        });

        await waitFor(() => {
            const jobLevels = JSON.parse(screen.getByTestId('jobLevels').textContent);
            expect(jobLevels[0].basicSalary).toBe('5000');
        });
    });

    it('should validate empty name', () => {
        render(<TestComponent />);
        fireEvent.click(screen.getByText('Validate'));
        expect(toastr.error).toHaveBeenCalledWith(
            'Salary Structure field is required!',
            '',
            {}
        );
    });

    it('should handle submit successfully (create)', async () => {
        axios.post.mockResolvedValueOnce({ data: { isSuccessful: true } });
        render(<TestComponent />);

        await act(async () => {
            fireEvent.click(screen.getByText('Submit'));
        });

        expect(axios.post).toHaveBeenCalledWith(
            'SalaryStructure/Create',
            expect.any(Object),
            expect.any(Object)
        );
        expect(toastr.success).toHaveBeenCalledWith('Created successfully!', '', {});
    });

    it('should handle submit failure', async () => {
        axios.post.mockResolvedValueOnce({ data: { isSuccessful: false, message: 'Error' } });
        render(<TestComponent />);

        await act(async () => {
            fireEvent.click(screen.getByText('Submit'));
        });

        expect(toastr.error).toHaveBeenCalledWith('Error', '', {});
    });

    it('should call axiosError on exception', async () => {
        axios.mockRejectedValueOnce(new Error('Network'));
        render(<TestComponent />);

        await waitFor(() => {
            expect(axiosError).toHaveBeenCalled();
        });
    });

    it('should handle submit successfully (edit mode)', async () => {
        axios.post.mockResolvedValueOnce({ data: { isSuccessful: true } });

        render(<TestComponent isEdit={true} />);

        await act(async () => {
            fireEvent.click(screen.getByText('Submit'));
        });

        expect(axios.post).toHaveBeenCalledWith(
            'SalaryStructure/Edit',
            expect.any(Object),
            expect.any(Object)
        );
        expect(toastr.success).toHaveBeenCalledWith('Updated successfully!', '', {});
    });

});
