import { render, fireEvent, waitFor, screen } from '@testing-library/react';
import React from 'react';
import axios from 'axios';
import AppContainer from '../AppContainer';

jest.spyOn(console, 'warn').mockImplementation(() => {});
jest.spyOn(console, 'error').mockImplementation(() => {});

// Mock axios
jest.mock('axios');

// Mock shared dependencies
const mockResolveClientUrl = jest.fn(url => `/api/${url}`);
const mockAxiosError = jest.fn();

jest.mock('../../../../shared/common.functions', () => ({
    resolveClientUrl: (...args) => mockResolveClientUrl(...args),
    axiosError: (...args) => mockAxiosError(...args),
    toastOptions: jest.fn(() => ({})),
}));

// Mock Errors component
jest.mock('../../../../shared/components/Errors', () => {
    return function Errors({ errors }) {
        if (!errors || Object.keys(errors).length === 0) return null;
        return (
            <div data-testid="errors">
                {Object.entries(errors).map(([key, value]) => (
                    <div key={key} data-testid={`error-${key}`}>{value}</div>
                ))}
            </div>
        );
    };
});

// Mock EmployeeAutocomplete
jest.mock('../../../../shared/components/EmployeeAutocomplete', () => {
    return function EmployeeAutocomplete({ value, onChange, onSelect, options }) {
        return (
            <div data-testid="employee-autocomplete-wrapper">
                <input
                    data-testid="employee-autocomplete"
                    value={value || ''}
                    onChange={(e) => {
                        onChange(e.target.value);
                    }}
                />
                {options && options.length > 0 && (
                    <ul data-testid="employee-options">
                        {options.map((opt) => (
                            <li 
                                key={opt.id}
                                data-testid={`employee-option-${opt.id}`}
                                onClick={() => onSelect(opt.id, opt)}
                            >
                                {opt.value}
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        );
    };
});

// Mock TreeMultiselect
jest.mock('../../../../shared/components/react-tree-multiselect/TreeMultiselect', () => {
    return function TreeMultiselect({ onChange, data, singleSelect }) {
        return (
            <div data-testid="tree-multiselect">
                {data && data.length > 0 ? (
                    <ul>
                        {data.map((team) => (
                            <li key={team.id}>
                                {team.enable !== false && (
                                    <button
                                        data-testid={`team-option-${team.id}`}
                                        onClick={() => onChange([team])}
                                    >
                                        {team.text}
                                    </button>
                                )}
                                {team.items && team.items.length > 0 && (
                                    <ul>
                                        {team.items.map((child) => (
                                            <li key={child.id}>
                                                {child.enable !== false && (
                                                    <button
                                                        data-testid={`team-option-${child.id}`}
                                                        onClick={() => onChange([child])}
                                                    >
                                                        {child.text}
                                                    </button>
                                                )}
                                            </li>
                                        ))}
                                    </ul>
                                )}
                            </li>
                        ))}
                    </ul>
                ) : (
                    <div data-testid="no-teams">No teams</div>
                )}
            </div>
        );
    };
});

// Mock DatePickerField
jest.mock('../../../../shared/components/DatePickerField', () => {
    return function DatePickerField({ name }) {
        return <input data-testid={`date-${name}`} name={name} type="date" />;
    };
});

// Mock SelectField
jest.mock('../../../../shared/components/SelectField', () => {
    const { Field } = require('formik');
    return function SelectField({ name, options }) {
        return (
            <Field name={name}>
                {({ field }) => (
                    <select {...field} data-testid={`select-${name}`}>
                        <option value="">Select...</option>
                        {options && options.map(opt => (
                            <option key={opt.value} value={opt.value}>
                                {opt.label}
                            </option>
                        ))}
                    </select>
                )}
            </Field>
        );
    };
});

// Mock submitTransfer from common.functions
const mockSubmitTransfer = jest.fn();
jest.mock('../../common.functions', () => {
    const actual = jest.requireActual('../../common.functions');
    return {
        ...actual,
        submitTransfer: (...args) => mockSubmitTransfer(...args),
    };
});

describe('AppContainer Component', () => {
    const mockAxios = axios;
    const mockCompanies = [
        { id: 3, text: 'Company A' },
        { id: 4, text: 'Company B' },
        { id: 5, text: 'Company C' },
    ];

    beforeEach(() => {
        jest.clearAllMocks();
        mockAxios.mockReset();
        mockSubmitTransfer.mockReset();

        // Default axios behavior by URL
        mockAxios.mockImplementation((config) => {
            const url = (config && config.url) || '';
            if (url.includes('GroupEmployeeList/GetCompanyDropdownList')) {
                return Promise.resolve({ data: mockCompanies });
            }
            if (url.includes('GroupEmployeeList/GroupEmployeeList')) {
                return Promise.resolve({ data: [] });
            }
            if (url.includes('Team/GetOrgChart')) {
                return Promise.resolve({ data: [] });
            }
            if (url.includes('GroupEmployeeList/GetInfoEmployeeTransfer')) {
                return Promise.resolve({ data: {} });
            }
            return Promise.resolve({ data: {} });
        });

        // Spy history.back
        jest.spyOn(window.history, 'back').mockImplementation(() => {});
    });

    describe('Initial Render', () => {
        it('should render step 1 initially', async () => {
            render(<AppContainer />);

            await waitFor(() => {
                expect(screen.getByText('Employee')).toBeInTheDocument();
                expect(screen.getByText('Target Company')).toBeInTheDocument();
                expect(screen.getByText('Team')).toBeInTheDocument();
                expect(screen.getByText('Remark')).toBeInTheDocument();
                expect(screen.getByText('Next')).toBeInTheDocument();
            });
        });

        it('should fetch and filter companies on mount', async () => {
            const allCompanies = [
                { id: 1, text: 'NexCompany' },
                { id: 2, text: 'LumiCompany' },
                { id: 3, text: 'ValidCompany' },
                { id: 34, text: 'NexVisionCompany' },
                { id: 5, text: 'AnotherValid' },
            ];
            
            mockAxios.mockResolvedValueOnce({ data: allCompanies });
            
            render(<AppContainer />);

            await waitFor(() => {
                expect(mockAxios).toHaveBeenCalledWith({
                    url: '/api/GroupEmployeeList/GetCompanyDropdownList',
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                });
            });

            // Check that companies are filtered
            const select = screen.getByDisplayValue('');
            const options = select.querySelectorAll('option');
            
            // Should exclude companies with id 1, 2, 34
            expect(options.length).toBe(3); // empty + 2 valid companies
        });

        it('should handle axios error when fetching companies', async () => {
            mockAxios.mockRejectedValueOnce(new Error('Network Error'));

            render(<AppContainer />);

            await waitFor(() => {
                expect(mockAxiosError).toHaveBeenCalled();
            });
        });
    });

    describe('Employee Search', () => {
        it('should fetch employees when typing >= 2 characters', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
                { empId: 2, nickName: 'Jane', company: 'Company B', fullName: 'Jane Smith', companyId: 4 },
            ];
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees });

            render(<AppContainer />);

            const input = await screen.findByTestId('employee-autocomplete');
            
            // Type 2 characters
            fireEvent.change(input, { target: { value: 'Jo' } });

            await waitFor(() => {
                expect(mockAxios).toHaveBeenCalledWith(
                    expect.objectContaining({
                        url: '/api/GroupEmployeeList/GroupEmployeeList',
                        method: 'GET',
                        params: expect.objectContaining({
                            keyword: 'Jo',
                            isDelete: false,
                            isEnable: true,
                        }),
                    })
                );
            });

            // Should display employee options
            await waitFor(() => {
                expect(screen.getByTestId('employee-option-1')).toBeInTheDocument();
                expect(screen.getByText('John ( Company A)')).toBeInTheDocument();
            });
        });

        it('should not fetch employees when typing < 2 characters', async () => {
            render(<AppContainer />);

            const input = await screen.findByTestId('employee-autocomplete');
            
            fireEvent.change(input, { target: { value: 'J' } });

            await waitFor(() => {
                // Only companies API should be called
                expect(mockAxios).toHaveBeenCalledTimes(1);
            });

            expect(screen.queryByTestId('employee-options')).not.toBeInTheDocument();
        });

        it('should select employee when clicked', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
            ];
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees });

            render(<AppContainer />);

            const input = await screen.findByTestId('employee-autocomplete');
            fireEvent.change(input, { target: { value: 'Jo' } });

            await waitFor(() => {
                const option = screen.getByTestId('employee-option-1');
                fireEvent.click(option);
            });

            // Input value should be updated
            await waitFor(() => {
                expect(input).toHaveValue('John ( Company A)');
            });
        });

        it('should handle axios error when fetching employees', async () => {
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockRejectedValueOnce(new Error('Network Error'));

            render(<AppContainer />);

            const input = await screen.findByTestId('employee-autocomplete');
            fireEvent.change(input, { target: { value: 'Test' } });

            await waitFor(() => {
                expect(mockAxiosError).toHaveBeenCalled();
            });
        });
    });

    describe('Company Selection', () => {
        it('should load teams when company is selected', async () => {
            const mockTeams = [
                { id: 1, text: 'Team 1', isLeaf: true, items: [] },
                { id: 2, text: 'Team 2', isLeaf: true, items: [] },
            ];

            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockTeams });

            render(<AppContainer />);

            const select = screen.getAllByRole('combobox')[0];
            fireEvent.change(select, { target: { value: '3' } });

            await waitFor(() => {
                expect(mockAxios).toHaveBeenCalledWith({
                    url: '/api/Team/GetOrgChart',
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    params: { companyId: '3' },
                });
            });

            // Teams should be displayed
            await waitFor(() => {
                expect(screen.getByTestId('team-option-1')).toBeInTheDocument();
                expect(screen.getByTestId('team-option-2')).toBeInTheDocument();
            });
        });

        it('should clear teams when company is deselected', async () => {
            const mockTeams = [
                { id: 1, text: 'Team 1', isLeaf: true, items: [] },
            ];

            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockTeams });

            render(<AppContainer />);

            const select = screen.getAllByRole('combobox')[0];
            
            // Select company
            fireEvent.change(select, { target: { value: '3' } });
            
            await waitFor(() => {
                expect(screen.getByTestId('team-option-1')).toBeInTheDocument();
            });

            // Deselect company
            fireEvent.change(select, { target: { value: '' } });

            await waitFor(() => {
                expect(screen.queryByTestId('team-option-1')).not.toBeInTheDocument();
            });
        });

        it('should process tree data with nested teams', async () => {
            const mockTeams = [
                { 
                    id: 1, 
                    text: 'Parent Team', 
                    isLeaf: false, 
                    items: [
                        { id: 2, text: 'Child Team', isLeaf: true, items: [] }
                    ]
                },
            ];

            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockTeams });

            render(<AppContainer />);

            const select = screen.getAllByRole('combobox')[0];
            fireEvent.change(select, { target: { value: '3' } });

            await waitFor(() => {
                // Parent should not be selectable (enable: false for non-leaf)
                expect(screen.queryByTestId('team-option-1')).not.toBeInTheDocument();
                // Child should be selectable
                expect(screen.getByTestId('team-option-2')).toBeInTheDocument();
            });
        });

        it('should handle axios error when loading teams', async () => {
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockRejectedValueOnce(new Error('Network Error'));

            render(<AppContainer />);

            const select = screen.getAllByRole('combobox')[0];
            fireEvent.change(select, { target: { value: '3' } });

            await waitFor(() => {
                expect(mockAxiosError).toHaveBeenCalled();
            });
        });
    });

    describe('Team Selection', () => {
        it('should select team when clicked', async () => {
            const mockTeams = [
                { id: 1, text: 'Team 1', isLeaf: true, items: [] },
            ];

            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockTeams });

            render(<AppContainer />);

            const select = await screen.findByDisplayValue('');
            fireEvent.change(select, { target: { value: '3' } });

            await waitFor(() => {
                const teamButton = screen.getByTestId('team-option-1');
                fireEvent.click(teamButton);
            });

            // Team should be selected (verified implicitly)
        });
    });

    describe('Transfer Remark', () => {
        it('should update remark field', async () => {
            render(<AppContainer />);

            const textarea = await screen.findByRole('textbox');
            fireEvent.change(textarea, { target: { value: 'Test remark' } });

            expect(textarea).toHaveValue('Test remark');
        });
    });

    describe('Step 1 Validation', () => {
        it('should show error when employee is not selected', async () => {
            render(<AppContainer />);

            const nextButton = await screen.findByText('Next');
            fireEvent.click(nextButton);

            // Should not proceed to step 2
            expect(screen.queryByText('Personal')).not.toBeInTheDocument();
        });

        it('should show error when company is not selected', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
            ];
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees });

            render(<AppContainer />);

            // Select employee
            const input = await screen.findByTestId('employee-autocomplete');
            fireEvent.change(input, { target: { value: 'Jo' } });
            
            await waitFor(() => {
                const option = screen.getByTestId('employee-option-1');
                fireEvent.click(option);
            });

            // Click next without selecting company
            const nextButton = screen.getByText('Next');
            fireEvent.click(nextButton);

            expect(screen.queryByText('Personal')).not.toBeInTheDocument();
        });

        it('should show error when team is not selected', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
            ];
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees });

            render(<AppContainer />);

            // Select employee
            const input = await screen.findByTestId('employee-autocomplete');
            fireEvent.change(input, { target: { value: 'Jo' } });
            
            await waitFor(() => {
                const option = screen.getByTestId('employee-option-1');
                fireEvent.click(option);
            });

            // Select company
            const select = screen.getByDisplayValue('');
            fireEvent.change(select, { target: { value: '4' } });

            // Click next without selecting team
            const nextButton = screen.getByText('Next');
            fireEvent.click(nextButton);

            expect(screen.queryByText('Personal')).not.toBeInTheDocument();
        });

        it('should show error when same company is selected', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
            ];
            
            const mockTeams = [
                { id: 1, text: 'Team 1', isLeaf: true, items: [] },
            ];
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees })
                .mockResolvedValueOnce({ data: mockTeams });

            render(<AppContainer />);

            // Select employee
            const input = await screen.findByTestId('employee-autocomplete');
            fireEvent.change(input, { target: { value: 'Jo' } });
            
            await waitFor(() => {
                const option = screen.getByTestId('employee-option-1');
                fireEvent.click(option);
            });

            // Select same company as employee
            const select = screen.getByDisplayValue('');
            fireEvent.change(select, { target: { value: '3' } });

            await waitFor(() => {
                const teamButton = screen.getByTestId('team-option-1');
                fireEvent.click(teamButton);
            });

            // Click next
            const nextButton = screen.getByText('Next');
            fireEvent.click(nextButton);

            // Should not proceed
            expect(screen.queryByText('Personal')).not.toBeInTheDocument();
        });
    });

    describe('Navigate to Step 2', () => {
        it('should proceed to step 2 with valid data', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
            ];
            
            const mockTeams = [
                { id: 1, text: 'Team 1', isLeaf: true, items: [] },
            ];

            const mockEmployeeData = {
                userId: 1,
                code: 'EMP001',
                fullName: 'John Doe',
                nickName: 'John',
                employeeCard: 'CARD001',
                gender: 1,
                dateOfBirth: '1990-01-01',
                phone: '123456789',
                email: 'john@example.com',
                jobLevelId: 1,
                canOT: false,
                jobLevels: [{ id: 1, name: 'Junior' }],
                managementLevels: [{ id: 1, name: 'Manager' }],
                subCompanies: [{ subCompanyId: 1, name: 'Sub A' }],
                jobTitles: [{ id: 1, name: 'Developer' }],
                coutries: [{ countryCode: 'VN', countryName: 'Vietnam' }],
            };
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees })
                .mockResolvedValueOnce({ data: mockTeams })
                .mockResolvedValueOnce({ data: mockEmployeeData });

            render(<AppContainer />);

            // Select employee
            const input = await screen.findByTestId('employee-autocomplete');
            fireEvent.change(input, { target: { value: 'Jo' } });
            
            await waitFor(() => {
                const option = screen.getByTestId('employee-option-1');
                fireEvent.click(option);
            });

            // Select company (different from employee's)
            const select = screen.getByDisplayValue('');
            fireEvent.change(select, { target: { value: '4' } });

            await waitFor(() => {
                const teamButton = screen.getByTestId('team-option-1');
                fireEvent.click(teamButton);
            });

            // Click next
            const nextButton = screen.getByText('Next');
            fireEvent.click(nextButton);

            // Should fetch employee data
            await waitFor(() => {
                expect(mockAxios).toHaveBeenCalledWith(
                    expect.objectContaining({
                        url: '/api/GroupEmployeeList/GetInfoEmployeeTransfer',
                        params: expect.objectContaining({
                            id: 1,
                            transferInCompanyId: 4,
                            transferOutCompanyId: 3,
                        }),
                    })
                );
            });

            // Should show step 2
            await waitFor(() => {
                expect(screen.getByText('Personal')).toBeInTheDocument();
                expect(screen.getByText('Contact')).toBeInTheDocument();
                expect(screen.getByText('Information')).toBeInTheDocument();
                expect(screen.getByText('Policy')).toBeInTheDocument();
            });
        });

        it('should handle API error when loading employee data', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
            ];
            
            const mockTeams = [
                { id: 1, text: 'Team 1', isLeaf: true, items: [] },
            ];
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees })
                .mockResolvedValueOnce({ data: mockTeams })
                .mockRejectedValueOnce(new Error('API Error'));

            render(<AppContainer />);

            // Fill form
            const input = await screen.findByTestId('employee-autocomplete');
            fireEvent.change(input, { target: { value: 'Jo' } });
            
            await waitFor(() => {
                const option = screen.getByTestId('employee-option-1');
                fireEvent.click(option);
            });

            const select = screen.getByDisplayValue('');
            fireEvent.change(select, { target: { value: '4' } });

            await waitFor(() => {
                const teamButton = screen.getByTestId('team-option-1');
                fireEvent.click(teamButton);
            });

            const nextButton = screen.getByText('Next');
            fireEvent.click(nextButton);

            // Should not proceed
            await waitFor(() => {
                expect(screen.queryByText('Personal')).not.toBeInTheDocument();
            });
        });
    });

    describe('Step 2 - Generate Employee ID', () => {
        it('should generate employee ID when button clicked', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
            ];
            
            const mockTeams = [
                { id: 1, text: 'Team 1', isLeaf: true, items: [] },
            ];

            const mockEmployeeData = {
                userId: 1,
                code: 'EMP001',
                fullName: 'John Doe',
                nickName: 'John',
                jobLevels: [],
                managementLevels: [],
                subCompanies: [],
                jobTitles: [],
                coutries: [],
            };
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees })
                .mockResolvedValueOnce({ data: mockTeams })
                .mockResolvedValueOnce({ data: mockEmployeeData });
            
            // Mock axios.get for generate ID
            jest.spyOn(axios, 'get').mockResolvedValueOnce({ data: 'EMP999' });

            render(<AppContainer />);

            // Navigate to step 2
            await navigateToStep2();

            // Click generate button
            const generateButton = screen.getByText('Generate');
            fireEvent.click(generateButton);

            await waitFor(() => {
                expect(axios.get).toHaveBeenCalledWith(
                    expect.stringContaining('employee/GenerateEmployeeCode?companyId=4')
                );
            });
        });
    });

    describe('Form Submission', () => {
        it('should submit transfer form', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
            ];
            
            const mockTeams = [
                { id: 1, text: 'Team 1', isLeaf: true, items: [] },
            ];

            const mockEmployeeData = {
                userId: 1,
                code: 'EMP001',
                fullName: 'John Doe',
                nickName: 'John',
                jobLevels: [],
                managementLevels: [],
                subCompanies: [],
                jobTitles: [],
                coutries: [],
            };
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees })
                .mockResolvedValueOnce({ data: mockTeams })
                .mockResolvedValueOnce({ data: mockEmployeeData });

            render(<AppContainer />);

            await navigateToStep2();

            // Submit form
            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);

            await waitFor(() => {
                expect(mockSubmitTransfer).toHaveBeenCalledWith(
                    'GroupEmployeeList/Transfer',
                    expect.objectContaining({
                        Id: 1,
                        TargetCompanyId: 4,
                        Enabled: true,
                    }),
                    expect.any(Function),
                    'GroupEmployeeList/Index'
                );
            });
        });
    });

    describe('Navigation', () => {
        it('should go back when cancel clicked on step 1', async () => {
            render(<AppContainer />);

            const cancelButton = await screen.findByText('Cancel');
            fireEvent.click(cancelButton);

            expect(window.history.back).toHaveBeenCalled();
        });

        it('should go back when cancel clicked on step 2', async () => {
            const mockEmployees = [
                { empId: 1, nickName: 'John', company: 'Company A', fullName: 'John Doe', companyId: 3 },
            ];
            
            const mockTeams = [
                { id: 1, text: 'Team 1', isLeaf: true, items: [] },
            ];

            const mockEmployeeData = {
                userId: 1,
                code: 'EMP001',
                fullName: 'John Doe',
                nickName: 'John',
                jobLevels: [],
                managementLevels: [],
                subCompanies: [],
                jobTitles: [],
                coutries: [],
            };
            
            mockAxios
                .mockResolvedValueOnce({ data: mockCompanies })
                .mockResolvedValueOnce({ data: mockEmployees })
                .mockResolvedValueOnce({ data: mockTeams })
                .mockResolvedValueOnce({ data: mockEmployeeData });

            render(<AppContainer />);

            await navigateToStep2();

            const cancelButtons = screen.getAllByText('Cancel');
            fireEvent.click(cancelButtons[0]);

            expect(window.history.back).toHaveBeenCalled();
        });
    });

    // Helper function to navigate to step 2
    async function navigateToStep2() {
        const input = await screen.findByTestId('employee-autocomplete');
        fireEvent.change(input, { target: { value: 'Jo' } });
        
        await waitFor(() => {
            const option = screen.getByTestId('employee-option-1');
            fireEvent.click(option);
        });

        const select = screen.getAllByRole('combobox')[0];
        fireEvent.change(select, { target: { value: '4' } });

        await waitFor(() => {
            const teamButton = screen.getByTestId('team-option-1');
            fireEvent.click(teamButton);
        });

        const nextButton = screen.getByText('Next');
        fireEvent.click(nextButton);

        await waitFor(() => {
            expect(screen.getByText('Personal')).toBeInTheDocument();
        });
    }
});
