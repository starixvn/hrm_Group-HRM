import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import BasicSalaryConfigModal from '../../../home/components/BasicSalaryConfigModal';

describe('BasicSalaryConfigModal Component', () => {
    const defaultConfigOptions = [
        { value: '1', label: 'Basic Salary' },
        { value: '2', label: 'Job Level Add-On' },
        { value: '3', label: 'Special Subsidy' }
    ];

    const defaultProps = {
        show: false,
        onClose: jest.fn(),
        onSubmit: jest.fn(),
        selectedConfigs: [],
        onConfigChange: jest.fn(),
        configOptions: defaultConfigOptions
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
        });


        it('should render close button in header', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const closeButton = screen.getByRole('button', { name: /close/i });
            expect(closeButton).toBeInTheDocument();
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
        });

        it('should have correct checkbox structure', () => {
            render(<BasicSalaryConfigModal {...defaultProps} show={true} />);
            
            const checkboxes = screen.getAllByRole('checkbox');
            expect(checkboxes).toHaveLength(3);
            
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
            const selectedConfigs = ['1', '2', '3'];
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
            
        });
    });
});
