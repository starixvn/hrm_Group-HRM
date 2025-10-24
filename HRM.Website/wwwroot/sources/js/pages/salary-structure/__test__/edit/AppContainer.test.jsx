import React from 'react';
import { render, screen } from '@testing-library/react';
import AppContainer from '../../edit/AppContainer';

// Mock the useSalaryStructure hook
jest.mock('../../shared/useSalaryStructure', () => {
    return jest.fn(() => ({
        jobLevels: [
            { id: 1, name: 'Level 1', basicSalary: '1000', expatSalary: '1500', jobLevelAddOn: '200' },
            { id: 2, name: 'Level 2', basicSalary: '1200', expatSalary: '1800', jobLevelAddOn: '300' }
        ],
        initialValues: {
            salaryStructureId: '123',
            name: 'Test Structure'
        },
        handleChangeSalary: jest.fn(),
        validateFormValues: jest.fn(),
        handleSubmit: jest.fn()
    }));
});

// Mock the SalaryStructureForm component
jest.mock('../../shared/SalaryStructureForm', () => {
    return function MockSalaryStructureForm({ jobLevels, initialValues, isEdit }) {
        return (
            <div data-testid="salary-structure-form">
                <div data-testid="job-levels-count">{jobLevels.length}</div>
                <div data-testid="initial-values">{JSON.stringify(initialValues)}</div>
                <div data-testid="is-edit">{isEdit.toString()}</div>
            </div>
        );
    };
});

describe('Edit AppContainer Component', () => {
    it('should render without crashing', () => {
        render(<AppContainer />);
        
        expect(screen.getByTestId('salary-structure-form')).toBeInTheDocument();
    });

    it('should call useSalaryStructure with true for edit mode', () => {
        const useSalaryStructure = require('../../shared/useSalaryStructure');
        
        render(<AppContainer />);
        
        expect(useSalaryStructure).toHaveBeenCalledWith(true);
    });

    it('should pass correct props to SalaryStructureForm', () => {
        render(<AppContainer />);
        
        expect(screen.getByTestId('job-levels-count')).toHaveTextContent('2');
        expect(screen.getByTestId('initial-values')).toHaveTextContent('{"salaryStructureId":"123","name":"Test Structure"}');
        expect(screen.getByTestId('is-edit')).toHaveTextContent('true');
    });

    it('should pass all required props to SalaryStructureForm', () => {
        const useSalaryStructure = require('../../shared/useSalaryStructure');
        const mockProps = {
            jobLevels: [
                { id: 1, name: 'Level 1', basicSalary: '1000', expatSalary: '1500', jobLevelAddOn: '200' }
            ],
            initialValues: { salaryStructureId: '456', name: 'Updated Structure' },
            handleChangeSalary: jest.fn(),
            validateFormValues: jest.fn(),
            handleSubmit: jest.fn()
        };
        
        useSalaryStructure.mockReturnValue(mockProps);
        
        render(<AppContainer />);
        
        expect(screen.getByTestId('job-levels-count')).toHaveTextContent('1');
        expect(screen.getByTestId('initial-values')).toHaveTextContent('{"salaryStructureId":"456","name":"Updated Structure"}');
    });

    it('should pass isEdit as true to SalaryStructureForm', () => {
        render(<AppContainer />);
        
        expect(screen.getByTestId('is-edit')).toHaveTextContent('true');
    });
});

