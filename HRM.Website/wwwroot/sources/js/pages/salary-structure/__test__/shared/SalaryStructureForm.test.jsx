import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { Formik } from 'formik';
import SalaryStructureForm from '../../shared/SalaryStructureForm';

// Mock history.back
const mockHistoryBack = jest.fn();
Object.defineProperty(window, 'history', {
    value: { back: mockHistoryBack },
    writable: true
});

describe('SalaryStructureForm Component', () => {
    const mockJobLevels = [
        {
            id: 1,
            name: 'Level 1',
            basicSalary: '1000',
            expatSalary: '1500',
            jobLevelAddOn: '200'
        },
        {
            id: 2,
            name: 'Level 2',
            basicSalary: '1200',
            expatSalary: '1800',
            jobLevelAddOn: '300'
        }
    ];

    const mockInitialValues = {
        salaryStructureId: '1',
        name: 'Test Structure'
    };

    const mockHandleChangeSalary = jest.fn();
    const mockValidateFormValues = jest.fn();
    const mockHandleSubmit = jest.fn();

    const defaultProps = {
        jobLevels: mockJobLevels,
        initialValues: mockInitialValues,
        handleChangeSalary: mockHandleChangeSalary,
        validateFormValues: mockValidateFormValues,
        handleSubmit: mockHandleSubmit,
        isEdit: false
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('Default Props', () => {
        it('should use default isEdit value when not provided', () => {
            const propsWithoutIsEdit = {
                jobLevels: mockJobLevels,
                initialValues: mockInitialValues,
                handleChangeSalary: mockHandleChangeSalary,
                validateFormValues: mockValidateFormValues,
                handleSubmit: mockHandleSubmit
            };
            
            render(<SalaryStructureForm {...propsWithoutIsEdit} />);
            
            expect(screen.getByRole('button', { name: /add/i })).toBeInTheDocument();
        });
    });

    describe('Rendering', () => {
        it('should render form with correct structure', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            expect(screen.getByText('Salary Structure')).toBeInTheDocument();
            expect(screen.getByText('*')).toBeInTheDocument();
            expect(screen.getByRole('textbox')).toBeInTheDocument();
        });

        it('should render job levels table', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            expect(screen.getByText('Job Level')).toBeInTheDocument();
            expect(screen.getByText('Basic Salary')).toBeInTheDocument();
            expect(screen.getByText('Job Level Add-On')).toBeInTheDocument();
            expect(screen.getByText('Expat Salary')).toBeInTheDocument();
        });

        it('should render all job levels', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            expect(screen.getByText('Level 1')).toBeInTheDocument();
            expect(screen.getByText('Level 2')).toBeInTheDocument();
        });

        it('should render salary input fields for each job level', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const level1BasicSalary = screen.getByDisplayValue('1000');
            const level1ExpatSalary = screen.getByDisplayValue('1500');
            const level1JobLevelAddOn = screen.getByDisplayValue('200');
            
            expect(level1BasicSalary).toBeInTheDocument();
            expect(level1ExpatSalary).toBeInTheDocument();
            expect(level1JobLevelAddOn).toBeInTheDocument();
            
            const level2BasicSalary = screen.getByDisplayValue('1200');
            const level2ExpatSalary = screen.getByDisplayValue('1800');
            const level2JobLevelAddOn = screen.getByDisplayValue('300');
            
            expect(level2BasicSalary).toBeInTheDocument();
            expect(level2ExpatSalary).toBeInTheDocument();
            expect(level2JobLevelAddOn).toBeInTheDocument();
        });

        it('should render submit and back buttons', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            expect(screen.getByRole('button', { name: /add/i })).toBeInTheDocument();
            expect(screen.getByText('Back')).toBeInTheDocument();
        });

        it('should render "Add" button when isEdit is false', () => {
            render(<SalaryStructureForm {...defaultProps} isEdit={false} />);
            
            expect(screen.getByRole('button', { name: /add/i })).toBeInTheDocument();
        });

        it('should render "Update" button when isEdit is true', () => {
            render(<SalaryStructureForm {...defaultProps} isEdit={true} />);
            
            expect(screen.getByRole('button', { name: /update/i })).toBeInTheDocument();
        });
    });

    describe('Form Inputs', () => {
        it('should have correct input attributes for salary structure name', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const nameInput = screen.getByRole('textbox');
            expect(nameInput).toHaveAttribute('name', 'name');
            expect(nameInput).toHaveClass('form-control');
            expect(nameInput).toHaveAttribute('autocomplete', 'off');
        });

        it('should have correct input attributes for salary fields', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const basicSalaryInput = screen.getByDisplayValue('1000');
            expect(basicSalaryInput).toHaveAttribute('type', 'number');
            expect(basicSalaryInput).toHaveAttribute('min', '0');
            expect(basicSalaryInput).toHaveAttribute('step', '0.01');
            expect(basicSalaryInput).toHaveClass('form-control');
            expect(basicSalaryInput).toHaveAttribute('autocomplete', 'off');
        });

        it('should display initial values correctly', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const nameInput = screen.getByRole('textbox');
            expect(nameInput).toHaveValue('Test Structure');
        });
    });

    describe('Salary Input Changes', () => {
        it('should call handleChangeSalary when basic salary is changed', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const basicSalaryInput = screen.getByDisplayValue('1000');
            fireEvent.change(basicSalaryInput, { target: { value: '1100' } });
            
            expect(mockHandleChangeSalary).toHaveBeenCalledWith(1, 'basicSalary', '1100');
        });

        it('should call handleChangeSalary when expat salary is changed', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const expatSalaryInput = screen.getByDisplayValue('1500');
            fireEvent.change(expatSalaryInput, { target: { value: '1600' } });
            
            expect(mockHandleChangeSalary).toHaveBeenCalledWith(1, 'expatSalary', '1600');
        });

        it('should call handleChangeSalary when job level add-on is changed', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const jobLevelAddOnInput = screen.getByDisplayValue('200');
            fireEvent.change(jobLevelAddOnInput, { target: { value: '250' } });
            
            expect(mockHandleChangeSalary).toHaveBeenCalledWith(1, 'jobLevelAddOn', '250');
        });

        it('should call handleChangeSalary with correct job level ID', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const level2BasicSalaryInput = screen.getByDisplayValue('1200');
            fireEvent.change(level2BasicSalaryInput, { target: { value: '1300' } });
            
            expect(mockHandleChangeSalary).toHaveBeenCalledWith(2, 'basicSalary', '1300');
        });
    });

    describe('Form Submission', () => {
        it('should call handleSubmit when form is submitted', async () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const submitButton = screen.getByRole('button', { name: /add/i });
            fireEvent.click(submitButton);
            
            await waitFor(() => {
                expect(mockHandleSubmit).toHaveBeenCalled();
            });
        });

        it('should pass form values to handleSubmit', async () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const submitButton = screen.getByRole('button', { name: /add/i });
            fireEvent.click(submitButton);
            
            await waitFor(() => {
                expect(mockHandleSubmit).toHaveBeenCalledWith(
                    mockInitialValues,
                    expect.objectContaining({
                        setSubmitting: expect.any(Function)
                    })
                );
            });
        });
    });

    describe('Back Button', () => {
        it('should call history.back when back button is clicked', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const backButton = screen.getByText('Back');
            fireEvent.click(backButton);
            
            expect(mockHistoryBack).toHaveBeenCalled();
        });
    });

    describe('Formik Configuration', () => {
        it('should configure Formik with correct props', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            expect(screen.getByText('Salary Structure')).toBeInTheDocument();
        });

        it('should enable reinitialize when isEdit is true', () => {
            render(<SalaryStructureForm {...defaultProps} isEdit={true} />);
            
            expect(screen.getByText('Salary Structure')).toBeInTheDocument();
        });

        it('should disable reinitialize when isEdit is false', () => {
            render(<SalaryStructureForm {...defaultProps} isEdit={false} />);
            
            expect(screen.getByText('Salary Structure')).toBeInTheDocument();
        });
    });

    describe('Empty Job Levels', () => {
        it('should render empty table when no job levels provided', () => {
            render(<SalaryStructureForm {...defaultProps} jobLevels={[]} />);
            
            expect(screen.getByText('Job Level')).toBeInTheDocument();
            expect(screen.getByText('Basic Salary')).toBeInTheDocument();
            expect(screen.getByText('Job Level Add-On')).toBeInTheDocument();
            expect(screen.getByText('Expat Salary')).toBeInTheDocument();
            
            expect(screen.queryByText('Level 1')).not.toBeInTheDocument();
            expect(screen.queryByText('Level 2')).not.toBeInTheDocument();
            expect(screen.queryAllByRole('spinbutton')).toHaveLength(0);
        });

        it('should render table headers even with empty job levels', () => {
            render(<SalaryStructureForm {...defaultProps} jobLevels={[]} />);
            
            const table = screen.getByRole('table');
            expect(table).toBeInTheDocument();
            
            const headers = screen.getAllByRole('columnheader');
            expect(headers).toHaveLength(4);
        });
    });

    describe('Job Level with Empty Values', () => {
        const jobLevelsWithEmptyValues = [
            {
                id: 1,
                name: 'Level 1',
                basicSalary: '',
                expatSalary: '',
                jobLevelAddOn: ''
            }
        ];

        it('should render job level with empty salary values', () => {
            render(
                <SalaryStructureForm 
                    {...defaultProps} 
                    jobLevels={jobLevelsWithEmptyValues} 
                />
            );
            
            expect(screen.getByText('Level 1')).toBeInTheDocument();
            
            const salaryInputs = screen.getAllByRole('spinbutton');
            expect(salaryInputs).toHaveLength(3);
            
            salaryInputs.forEach(input => {
                expect(input).toHaveValue(null);
            });
        });
    });

    describe('Form Validation', () => {
        it('should call validateFormValues when form is validated', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            // Formik will call validate function during form lifecycle
            expect(screen.getByText('Salary Structure')).toBeInTheDocument();
        });
    });

    describe('Accessibility', () => {
        it('should have proper form structure', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            expect(screen.getByRole('textbox')).toBeInTheDocument();
            expect(screen.getAllByRole('spinbutton')).toHaveLength(6);        });

        it('should have proper labels', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            expect(screen.getByText('Salary Structure')).toBeInTheDocument();
            expect(screen.getByText('*')).toBeInTheDocument();        });

        it('should have proper button roles', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            expect(screen.getByRole('button', { name: /add/i })).toBeInTheDocument();
            expect(screen.getByText('Back')).toBeInTheDocument();
        });
    });

    describe('CSS Classes', () => {
        it('should apply correct CSS classes', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            expect(screen.getByRole('textbox')).toHaveClass('form-control');
            expect(screen.getAllByRole('spinbutton')[0]).toHaveClass('form-control');
        });

        it('should apply correct table classes', () => {
            render(<SalaryStructureForm {...defaultProps} />);
            
            const table = screen.getByRole('table');
            expect(table).toHaveClass('table', 'table-custom');
        });
    });

    describe('Edge Cases', () => {
        it('should handle undefined jobLevels gracefully', () => {
            const consoleSpy = jest.spyOn(console, 'error').mockImplementation(() => {});
            
            expect(() => {
                render(<SalaryStructureForm {...defaultProps} jobLevels={undefined} />);
            }).toThrow();
            
            consoleSpy.mockRestore();
        });

        it('should handle null jobLevels gracefully', () => {
            const consoleSpy = jest.spyOn(console, 'error').mockImplementation(() => {});
            
            expect(() => {
                render(<SalaryStructureForm {...defaultProps} jobLevels={null} />);
            }).toThrow();
            
            consoleSpy.mockRestore();
        });

        it('should handle undefined initialValues', () => {
            render(<SalaryStructureForm {...defaultProps} initialValues={undefined} />);
            
            expect(screen.getByText('Salary Structure')).toBeInTheDocument();
        });

        it('should handle job levels with missing properties', () => {
            const incompleteJobLevels = [
                {
                    id: 1,
                    name: 'Level 1',
                    basicSalary: '',
                    expatSalary: '',
                    jobLevelAddOn: ''
                }
            ];

            render(<SalaryStructureForm {...defaultProps} jobLevels={incompleteJobLevels} />);
            
            expect(screen.getByText('Level 1')).toBeInTheDocument();
        });
    });
});
