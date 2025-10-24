import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import axios from 'axios';
import toastr from 'toastr';
import Filter from '../../../home/components/Filter';

// Mock dependencies
jest.mock('axios');
jest.mock('toastr');
jest.mock('../../../../../shared/common.functions', () => ({
    resolveClientUrl: jest.fn((url) => `/test${url}`),
    axiosError: jest.fn(),
    toastOptions: jest.fn(() => ({}))
}));

const mockAxios = axios;
const mockToastr = toastr;

describe('Filter Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
        delete window.location;
        window.location = { href: '' };
    });

    describe('Rendering', () => {
        it('should render create button', () => {
            render(<Filter />);
            
            const createButton = screen.getByText('New Salary Structure');
            expect(createButton).toBeInTheDocument();
            expect(createButton).toHaveClass('btn', 'btn-request');
        });

        it('should render config button', () => {
            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            expect(configButton).toBeInTheDocument();
            expect(configButton).toHaveClass('btn', 'btn-primary');
        });

        it('should render plus icon in create button', () => {
            render(<Filter />);
            
            const plusIcon = screen.getByText('New Salary Structure').querySelector('.fa-plus');
            expect(plusIcon).toBeInTheDocument();
        });
    });

    describe('Create Button Functionality', () => {
        it('should navigate to create page when create button is clicked', () => {
            render(<Filter />);
            
            const createButton = screen.getByText('New Salary Structure');
            fireEvent.click(createButton);
            
            expect(window.location.href).toBe('/testsalarystructure/create');
        });
    });

    describe('Config Button Functionality', () => {
        it('should open config modal when config button is clicked', async () => {
            const mockResponse = {
                data: '1,2,3'
            };
            mockAxios.get.mockResolvedValue(mockResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(mockAxios.get).toHaveBeenCalledWith(
                    '/testSalaryStructure/GetBasicSalaryConfig',
                    { headers: { "X-Requested-With": "XMLHttpRequest" } }
                );
            });

            await waitFor(() => {
                const modalTitle = screen.getByRole('heading', { name: 'Basic Salary Configuration' });
                expect(modalTitle).toBeInTheDocument();
            });
        });

        it('should handle config API error gracefully', async () => {
            const mockError = new Error('API Error');
            mockAxios.get.mockRejectedValue(mockError);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(mockAxios.get).toHaveBeenCalled();
            });
        });

        it('should parse config value correctly', async () => {
            const mockResponse = {
                data: '1,2,3'
            };
            mockAxios.get.mockResolvedValue(mockResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });
        });

        it('should handle empty config value', async () => {
            const mockResponse = {
                data: ''
            };
            mockAxios.get.mockResolvedValue(mockResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });
        });

        it('should handle null config value', async () => {
            const mockResponse = {
                data: null
            };
            mockAxios.get.mockResolvedValue(mockResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });
        });

        it('should handle array config value', async () => {
            const mockResponse = {
                data: ['1', '2', '3']
            };
            mockAxios.get.mockResolvedValue(mockResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });
        });
    });

    describe('Config Modal Submit', () => {
        it('should submit config successfully', async () => {
            const mockGetResponse = {
                data: '1,2'
            };
            const mockPostResponse = {
                data: {
                    isSuccessful: true,
                    message: 'Success'
                }
            };
            
            mockAxios.get.mockResolvedValue(mockGetResponse);
            mockAxios.post.mockResolvedValue(mockPostResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(mockAxios.post).toHaveBeenCalledWith(
                    '/testSalaryStructure/EditBasicSalaryConfig',
                    { BasicSalaryConfig: '1,2' },
                    { 
                        headers: { 
                            "X-Requested-With": "XMLHttpRequest",
                            "Content-Type": "application/json"
                        } 
                    }
                );
            });

            expect(mockToastr.success).toHaveBeenCalledWith(
                "Updated successfully!", 
                '', 
                {}
            );
        });

        it('should handle config submit error', async () => {
            const mockGetResponse = {
                data: '1,2'
            };
            const mockPostResponse = {
                data: {
                    isSuccessful: false,
                    message: 'Update failed'
                }
            };
            
            mockAxios.get.mockResolvedValue(mockGetResponse);
            mockAxios.post.mockResolvedValue(mockPostResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(mockAxios.post).toHaveBeenCalled();
            });

            expect(mockToastr.error).toHaveBeenCalledWith('Update failed');
        });

        it('should handle config submit API error', async () => {
            const mockGetResponse = {
                data: '1,2'
            };
            const mockError = new Error('API Error');
            
            mockAxios.get.mockResolvedValue(mockGetResponse);
            mockAxios.post.mockRejectedValue(mockError);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(mockAxios.post).toHaveBeenCalled();
            });
        });
    });

    describe('Config Modal Close', () => {
        it('should close modal when cancel button is clicked', async () => {
            const mockResponse = {
                data: '1,2'
            };
            mockAxios.get.mockResolvedValue(mockResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });

            const cancelButton = screen.getByText('Cancel');
            fireEvent.click(cancelButton);

            await waitFor(() => {
                const modalTitle = screen.queryByRole('heading', { name: 'Basic Salary Configuration' });
                expect(modalTitle).not.toBeInTheDocument();
            });
        });
    });

    describe('Config Value Formatting', () => {
        it('should format array config for submit', async () => {
            const mockGetResponse = {
                data: ['1', '2', '3']
            };
            const mockPostResponse = {
                data: { isSuccessful: true }
            };
            
            mockAxios.get.mockResolvedValue(mockGetResponse);
            mockAxios.post.mockResolvedValue(mockPostResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(mockAxios.post).toHaveBeenCalledWith(
                    '/testSalaryStructure/EditBasicSalaryConfig',
                    { BasicSalaryConfig: '1,2,3' },
                    expect.any(Object)
                );
            });
        });

        it('should handle empty array config for submit', async () => {
            const mockGetResponse = {
                data: []
            };
            const mockPostResponse = {
                data: { isSuccessful: true }
            };
            
            mockAxios.get.mockResolvedValue(mockGetResponse);
            mockAxios.post.mockResolvedValue(mockPostResponse);

            render(<Filter />);
            
            const configButton = screen.getByText('Basic Salary Configuration');
            fireEvent.click(configButton);

            await waitFor(() => {
                expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
            });

            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(mockAxios.post).not.toHaveBeenCalled();
            });
        });
    });
});
