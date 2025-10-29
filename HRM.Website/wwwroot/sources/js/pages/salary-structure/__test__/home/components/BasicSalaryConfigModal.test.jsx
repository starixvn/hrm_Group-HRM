import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import BasicSalaryConfigModal from '../../../home/components/BasicSalaryConfigModal';

describe('BasicSalaryConfigModal Component', () => {
    const defaultProps = {
        show: false,
        onClose: jest.fn(),
        onSubmit: jest.fn(),
        selectedConfigs: [],
        onConfigChange: jest.fn()
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    beforeAll(() => {
        // Suppress React lifecycle warnings from react-bootstrap Modal
        jest.spyOn(console, 'warn').mockImplementation((message) => {
            if (
                typeof message === 'string' &&
                (message.includes('componentWillReceiveProps') || 
                 message.includes('componentWillUpdate'))
            ) {
                return;
            }
            console.warn(message);
        });
    });
    
    afterAll(() => {
        console.warn.mockRestore();
    });
    

    describe('Rendering', () => {
        it('should not render when show is false', () => {
            render(<BasicSalaryConfigModal {...defaultProps} />);
            
            expect(screen.queryByText('Basic Salary Configuration')).not.toBeInTheDocument();
        });

        it('should render when show is true', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            expect(screen.getByText('Basic Salary Configuration')).toBeInTheDocument();
        });

        it('should render all config options', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            expect(screen.getByText('Basic Salary')).toBeInTheDocument();
            expect(screen.getByText('Job Level Add-On')).toBeInTheDocument();
            expect(screen.getByText('Special Subsidy')).toBeInTheDocument();
            expect(screen.getByText('Management Allowance')).toBeInTheDocument();
        });

        it('should render submit and cancel buttons', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            expect(screen.getByText('Submit')).toBeInTheDocument();
            expect(screen.getByText('Cancel')).toBeInTheDocument();
        });

        it('should render close button in header', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const closeButton = screen.getByRole('button', { name: /close/i });
            expect(closeButton).toBeInTheDocument();
        });
    });

    describe('Checkbox Functionality', () => {
        it('should render unchecked checkboxes by default', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const checkboxes = screen.getAllByRole('checkbox');
            checkboxes.forEach(checkbox => {
                expect(checkbox).not.toBeChecked();
            });
        });

        it('should render checked checkboxes for selected configs', () => {
            const selectedConfigs = ['1', '3'];
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    selectedConfigs={selectedConfigs} 
                />
            );
            
            const basicSalaryCheckbox = screen.getByDisplayValue('1');
            const specialSubsidyCheckbox = screen.getByDisplayValue('3');
            
            expect(basicSalaryCheckbox).toBeChecked();
            expect(specialSubsidyCheckbox).toBeChecked();
        });

        it('should call onConfigChange when checkbox is clicked', () => {
            const mockOnConfigChange = jest.fn();
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    onConfigChange={mockOnConfigChange}
                />
            );
            
            const basicSalaryCheckbox = screen.getByDisplayValue('1');
            fireEvent.click(basicSalaryCheckbox);
            
            expect(mockOnConfigChange).toHaveBeenCalledWith(['1']);
        });

        it('should remove config when checked checkbox is clicked', () => {
            const mockOnConfigChange = jest.fn();
            const selectedConfigs = ['1', '2'];
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    selectedConfigs={selectedConfigs}
                    onConfigChange={mockOnConfigChange}
                />
            );
            
            const basicSalaryCheckbox = screen.getByDisplayValue('1');
            fireEvent.click(basicSalaryCheckbox);
            
            expect(mockOnConfigChange).toHaveBeenCalledWith(['2']);
        });

        it('should add config when unchecked checkbox is clicked', () => {
            const mockOnConfigChange = jest.fn();
            const selectedConfigs = ['2'];
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    selectedConfigs={selectedConfigs}
                    onConfigChange={mockOnConfigChange}
                />
            );
            
            const basicSalaryCheckbox = screen.getByDisplayValue('1');
            fireEvent.click(basicSalaryCheckbox);
            
            expect(mockOnConfigChange).toHaveBeenCalledWith(['2', '1']);
        });
    });

    describe('Submit Button Functionality', () => {
        it('should be disabled when no configs are selected', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const submitButton = screen.getByText('Submit');
            expect(submitButton).toBeDisabled();
        });

        it('should be enabled when configs are selected', () => {
            const selectedConfigs = ['1'];
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    selectedConfigs={selectedConfigs}
                />
            );
            
            const submitButton = screen.getByText('Submit');
            expect(submitButton).not.toBeDisabled();
        });

        it('should call onSubmit with selected configs when submit is clicked', () => {
            const mockOnSubmit = jest.fn();
            const selectedConfigs = ['1', '2'];
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    selectedConfigs={selectedConfigs}
                    onSubmit={mockOnSubmit}
                />
            );
            
            const submitButton = screen.getByText('Submit');
            fireEvent.click(submitButton);
            
            expect(mockOnSubmit).toHaveBeenCalledWith(selectedConfigs);
        });

        it('should not call onSubmit when submit button is disabled', () => {
            const mockOnSubmit = jest.fn();
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    onSubmit={mockOnSubmit}
                />
            );
            
            const submitButton = screen.getByText('Submit');
            expect(submitButton).toBeDisabled();
            
            fireEvent.click(submitButton);
            expect(mockOnSubmit).not.toHaveBeenCalled();
        });
    });

    describe('Cancel Button Functionality', () => {
        it('should call onClose when cancel button is clicked', () => {
            const mockOnClose = jest.fn();
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    onClose={mockOnClose}
                />
            );
            
            const cancelButton = screen.getByText('Cancel');
            fireEvent.click(cancelButton);
            
            expect(mockOnClose).toHaveBeenCalled();
        });

        it('should call onClose when close button in header is clicked', () => {
            const mockOnClose = jest.fn();
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    onClose={mockOnClose}
                />
            );
            
            const closeButton = screen.getByRole('button', { name: /close/i });
            fireEvent.click(closeButton);
            
            expect(mockOnClose).toHaveBeenCalled();
        });
    });

    describe('Config Options', () => {
        it('should render correct config option values and labels', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const basicSalaryCheckbox = screen.getByDisplayValue('1');
            expect(basicSalaryCheckbox).toBeInTheDocument();
            expect(screen.getByText('Basic Salary')).toBeInTheDocument();
            
            const jobLevelAddOnCheckbox = screen.getByDisplayValue('2');
            expect(jobLevelAddOnCheckbox).toBeInTheDocument();
            expect(screen.getByText('Job Level Add-On')).toBeInTheDocument();
            
            const specialSubsidyCheckbox = screen.getByDisplayValue('3');
            expect(specialSubsidyCheckbox).toBeInTheDocument();
            expect(screen.getByText('Special Subsidy')).toBeInTheDocument();
            
            const managementAllowanceCheckbox = screen.getByDisplayValue('4');
            expect(managementAllowanceCheckbox).toBeInTheDocument();
            expect(screen.getByText('Management Allowance')).toBeInTheDocument();
        });

        it('should have correct checkbox structure', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const checkboxes = screen.getAllByRole('checkbox');
            expect(checkboxes).toHaveLength(4);
            
            checkboxes.forEach(checkbox => {
                expect(checkbox).toHaveAttribute('type', 'checkbox');
            });
        });
    });

    describe('Modal Structure', () => {
        it('should have correct modal classes', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const modals = screen.getAllByRole('dialog');
            const mainModal = modals.find(modal => modal.classList.contains('config-modal'));
            expect(mainModal).toHaveClass('config-modal');
        });

        it('should render modal header with title', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const header = screen.getByText('Basic Salary Configuration').closest('.modal-header');
            expect(header).toBeInTheDocument();
        });

        it('should render modal body with form group', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const formGroup = screen.getByText('Basic Salary').closest('.form-group');
            expect(formGroup).toBeInTheDocument();
        });

        it('should render modal footer with buttons', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const footer = screen.getByText('Submit').closest('.modal-footer');
            expect(footer).toBeInTheDocument();
        });
    });

    describe('Edge Cases', () => {
        it('should handle undefined selectedConfigs', () => {
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    selectedConfigs={undefined}
                />
            );
            
            const checkboxes = screen.getAllByRole('checkbox');
            checkboxes.forEach(checkbox => {
                expect(checkbox).not.toBeChecked();
            });
        });

        it('should handle empty string in selectedConfigs', () => {
            const selectedConfigs = ['1', '', '2'];
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    selectedConfigs={selectedConfigs}
                />
            );
            
            const basicSalaryCheckbox = screen.getByDisplayValue('1');
            const jobLevelAddOnCheckbox = screen.getByDisplayValue('2');
            
            expect(basicSalaryCheckbox).toBeChecked();
            expect(jobLevelAddOnCheckbox).toBeChecked();
        });

        it('should handle all configs selected', () => {
            const selectedConfigs = ['1', '2', '3', '4'];
            render(
                <BasicSalaryConfigModal 
                    {...defaultProps} 
                    show={true} 
                    selectedConfigs={selectedConfigs}
                />
            );
            
            const checkboxes = screen.getAllByRole('checkbox');
            checkboxes.forEach(checkbox => {
                expect(checkbox).toBeChecked();
            });
            
            const submitButton = screen.getByText('Submit');
            expect(submitButton).not.toBeDisabled();
        });
    });
});
