import React from 'react';
import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import axios from 'axios';
import toastr from 'toastr';
import AppContainer from '../../../home/components/AppContainer';

jest.mock('axios');
jest.mock('toastr');
jest.mock('../../../../../shared/common.functions', () => ({
    resolveClientUrl: jest.fn((url) => `/test${url}`),
    axiosError: jest.fn(),
    toastOptions: jest.fn(() => ({}))
}));

jest.mock('../../../home/components/Table', () => {
    return function MockTable({ data, onDelete }) {
        return (
            <div data-testid="table">
                <div data-testid="table-data">{JSON.stringify(data)}</div>
                <button
                    data-testid="delete-button"
                    onClick={() => onDelete({ salaryStructureId: 1, name: 'Test Structure' })}
                >
                    Delete
                </button>
            </div>
        );
    };
});

jest.mock('../../../home/components/Filter', () => () => (

    <div data-testid="filter">Filter Component</div>));

jest.mock('../../../../../shared/components/SweetModalDialog', () => {
    return function MockSweetModalDialog({ show, contents, onSubmit, onHide }) {
        if (!show) return null;
        return (
            <div data-testid="modal">
                <div data-testid="modal-contents">{contents}</div>
                <button data-testid="modal-submit" onClick={onSubmit}>Confirm</button>
                <button data-testid="modal-cancel" onClick={onHide}>Cancel</button>
            </div>
        );
    };
});

const mockAxios = axios;
const mockToastr = toastr;

const mockHasPermission = (value) => {
    Object.defineProperty(window, 'hasPermission', { value, writable: true });
};

describe('Home AppContainer Component', () => {
    const mockSalaryStructures = [
        {
            salaryStructureId: 1,
            name: 'Structure 1',
            details: [{ jobLevelId: 1, name: 'Level 1', basicSalary: 1000, expatSalary: 1500, jobLevelAddOn: 200 }]
        },
        {
            salaryStructureId: 2,
            name: 'Structure 2',
            details: [{ jobLevelId: 2, name: 'Level 2', basicSalary: 1200, expatSalary: 1800, jobLevelAddOn: 300 }]
        }
    ];

    beforeEach(() => {
        jest.clearAllMocks();
        mockHasPermission('True');
    });

    afterEach(() => {
        delete window.hasPermission;
    });

    const renderApp = async () => {
        await act(async () => {
            render(<AppContainer />);
        });
    };

    describe('Rendering', () => {
        it('should render table with salary structures', async () => {
            mockAxios.post.mockResolvedValue({ data: mockSalaryStructures });
            await renderApp();
            await waitFor(() => expect(screen.getByTestId('table')).toBeInTheDocument());
            expect(screen.getByTestId('table-data')).toHaveTextContent(JSON.stringify(mockSalaryStructures));
        });

        it('should render filter when hasPermission is True', async () => {
            mockAxios.post.mockResolvedValue({ data: [] });
            await renderApp();
            expect(screen.getByTestId('filter')).toBeInTheDocument();
        });

        it('should not render filter when hasPermission is False', async () => {
            mockHasPermission('False');
            mockAxios.post.mockResolvedValue({ data: [] });
            await renderApp();
            expect(screen.queryByTestId('filter')).not.toBeInTheDocument();
        });

        it('should not render modal initially', async () => {
            mockAxios.post.mockResolvedValue({ data: [] });
            await renderApp();
            expect(screen.queryByTestId('modal')).not.toBeInTheDocument();
        });


    });

    describe('Data Loading', () => {
        it('should load salary structures on mount', async () => {
            mockAxios.post.mockResolvedValue({ data: mockSalaryStructures });
            await renderApp();
            await waitFor(() =>
                expect(mockAxios.post).toHaveBeenCalledWith(
                    '/testSalaryStructure/GetSalaryStructures',
                    undefined,
                    { headers: { 'X-Requested-With': 'XMLHttpRequest' } }
                )
            );
        });

        it('should handle empty salary structures', async () => {
            mockAxios.post.mockResolvedValue({ data: [] });
            await renderApp();
            await waitFor(() => expect(screen.getByTestId('table-data')).toHaveTextContent('[]'));
        });

        it('should handle API error during data loading', async () => {
            mockAxios.post.mockRejectedValue(new Error('API Error'));
            await renderApp();
            await waitFor(() => expect(mockAxios.post).toHaveBeenCalled());
        });


    });

    describe('Delete Functionality', () => {
        it('should show delete modal when delete is triggered', async () => {
            mockAxios.post.mockResolvedValue({ data: mockSalaryStructures });
            await renderApp();
            const deleteButton = await screen.findByTestId('delete-button');
            await act(async () => fireEvent.click(deleteButton));
            expect(screen.getByTestId('modal')).toBeInTheDocument();
        });

        it('should hide modal when cancel is clicked', async () => {
            mockAxios.post.mockResolvedValue({ data: mockSalaryStructures });
            await renderApp();
            const deleteButton = await screen.findByTestId('delete-button');
            await act(async () => fireEvent.click(deleteButton));
            const cancelButton = screen.getByTestId('modal-cancel');
            await act(async () => fireEvent.click(cancelButton));
            expect(screen.queryByTestId('modal')).not.toBeInTheDocument();
        });

        it('should delete successfully and refresh data', async () => {
            mockAxios.post
                .mockResolvedValueOnce({ data: mockSalaryStructures })
                .mockResolvedValueOnce({ data: { isSuccessful: true } });

            await renderApp();
            const deleteButton = await screen.findByTestId('delete-button');
            await act(async () => fireEvent.click(deleteButton));
            const confirmButton = screen.getByTestId('modal-submit');
            await act(async () => fireEvent.click(confirmButton));

            await waitFor(() =>
                expect(mockAxios.post).toHaveBeenCalledWith(
                    '/testSalaryStructure/Delete',
                    { id: 1 },
                    { headers: { 'X-Requested-With': 'XMLHttpRequest' } }
                )
            );

            expect(mockToastr.success).toHaveBeenCalledWith('Deleted successfully!', '', {});
        });

        it('should handle delete failure', async () => {
            mockAxios.post
                .mockResolvedValueOnce({ data: mockSalaryStructures })
                .mockResolvedValueOnce({ data: { isSuccessful: false, message: 'Delete failed' } });
            await renderApp();
            const deleteButton = await screen.findByTestId('delete-button');
            await act(async () => fireEvent.click(deleteButton));
            const confirmButton = screen.getByTestId('modal-submit');
            await act(async () => fireEvent.click(confirmButton));
            await waitFor(() =>
                expect(mockToastr.error).toHaveBeenCalledWith('Delete failed', '', {})
            );
        });
    });
});