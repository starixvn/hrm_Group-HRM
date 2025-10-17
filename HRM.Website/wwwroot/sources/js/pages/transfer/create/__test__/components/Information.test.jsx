import { render, fireEvent, screen } from '@testing-library/react';
import React from 'react';
import { Formik, Form } from 'formik';
import Information from '../../components/Information';

jest.spyOn(console, 'warn').mockImplementation(() => {});

jest.mock('../../../../../shared/components/DatePickerField', () => {
    return function DatePickerField({ name }) {
        return <input data-testid={`datepicker-${name}`} name={name} />;
    };
});

jest.mock('../../../../../shared/components/SelectField', () => {
    return function SelectField({ name, options, isSearchable, isClearable }) {
        return (
            <select 
                data-testid={`select-${name}`} 
                name={name}
                data-searchable={isSearchable}
                data-clearable={isClearable}
            >
                <option value="">Select...</option>
                {options && options.map(opt => (
                    <option key={opt.value} value={opt.value}>
                        {opt.label}
                    </option>
                ))}
            </select>
        );
    };
});

describe('Information Component', () => {
    const mockOnToggle = jest.fn();
    const mockEmployeeData = {
        SubCompanyId: 1,
        TeamDate: '2024-01-01',
        JobTitleId: 1,
        JobLevelId: 1,
        ManagementLevelId: 1,
        PromotionDate: '2023-01-01',
        JoinedDate: '2020-01-01',
        CompanyDate: '2024-01-01',
        ConfirmedDate: '2020-06-01',
        LengthOfServiceByGroup: '4 years 3 months',
    };

    const mockJobLevels = [
        { id: 1, name: 'Junior' },
        { id: 2, name: 'Senior' },
    ];

    const mockManagementLevels = [
        { id: 1, name: 'Manager' },
        { id: 2, name: 'Director' },
    ];

    const mockSubCompanies = [
        { subCompanyId: 1, name: 'Sub Company A' },
        { subCompanyId: 2, name: 'Sub Company B' },
    ];

    const mockJobTitles = [
        { id: 1, name: 'Developer' },
        { id: 2, name: 'Designer' },
    ];

    const renderWithFormik = (props = {}) => {
        const defaultProps = {
            isOpen: true,
            onToggle: mockOnToggle,
            employeeData: mockEmployeeData,
            jobLevels: mockJobLevels,
            managementLevels: mockManagementLevels,
            subCompanies: mockSubCompanies,
            jobTitles: mockJobTitles,
            ...props,
        };

        return render(
            <Formik
                initialValues={mockEmployeeData}
                onSubmit={jest.fn()}
            >
                <Form>
                    <Information {...defaultProps} />
                </Form>
            </Formik>
        );
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('should render information panel', () => {
        renderWithFormik();

        expect(screen.getByText('Information')).toBeInTheDocument();
        expect(screen.getByText('Sub Company')).toBeInTheDocument();
        expect(screen.getByText('Job Title')).toBeInTheDocument();
        expect(screen.getByText('Job Level')).toBeInTheDocument();
    });

    it('should toggle panel when header is clicked', () => {
        renderWithFormik();

        const panelHeader = screen.getByText('Information').closest('.panel-heading');
        fireEvent.click(panelHeader);

        expect(mockOnToggle).toHaveBeenCalled();
    });

    it('should render collapsed when isOpen is false', () => {
        const { container } = renderWithFormik({ isOpen: false });

        const panelGroup = container.querySelector('.panel-group');
        expect(panelGroup).toHaveClass('collapsed');

        const panelBody = container.querySelector('.panel-body');
        expect(panelBody).toHaveClass('collapsed');
    });

    it('should render expanded when isOpen is true', () => {
        const { container } = renderWithFormik({ isOpen: true });

        const panelGroup = container.querySelector('.panel-group');
        expect(panelGroup).not.toHaveClass('collapsed');
    });

    it('should render all select fields', () => {
        renderWithFormik();

        expect(screen.getByTestId('select-SubCompanyId')).toBeInTheDocument();
        expect(screen.getByTestId('select-JobTitleId')).toBeInTheDocument();
        expect(screen.getByTestId('select-JobLevelId')).toBeInTheDocument();
        expect(screen.getByTestId('select-ManagementLevelId')).toBeInTheDocument();
    });

    it('should render all date fields', () => {
        renderWithFormik();

        expect(screen.getByTestId('datepicker-TeamDate')).toBeInTheDocument();
        expect(screen.getByTestId('datepicker-PromotionDate')).toBeInTheDocument();
        expect(screen.getByTestId('datepicker-JoinedDate')).toBeInTheDocument();
        expect(screen.getByTestId('datepicker-CompanyDate')).toBeInTheDocument();
        expect(screen.getByTestId('datepicker-ConfirmedDate')).toBeInTheDocument();
    });

    it('should render sub company select with options', () => {
        renderWithFormik();

        const subCompanySelect = screen.getByTestId('select-SubCompanyId');
        expect(subCompanySelect).toBeInTheDocument();
        
        const options = subCompanySelect.querySelectorAll('option');
        expect(options.length).toBeGreaterThan(0);
    });

    it('should render job title select with options', () => {
        renderWithFormik();

        const jobTitleSelect = screen.getByTestId('select-JobTitleId');
        expect(jobTitleSelect).toBeInTheDocument();
        
        const options = jobTitleSelect.querySelectorAll('option');
        expect(options.length).toBeGreaterThan(0);
    });

    it('should render job level select with options', () => {
        renderWithFormik();

        const jobLevelSelect = screen.getByTestId('select-JobLevelId');
        expect(jobLevelSelect).toBeInTheDocument();
        
        const options = jobLevelSelect.querySelectorAll('option');
        expect(options.length).toBeGreaterThan(0);
    });

    it('should render management level select with options', () => {
        renderWithFormik();

        const managementLevelSelect = screen.getByTestId('select-ManagementLevelId');
        expect(managementLevelSelect).toBeInTheDocument();
        
        const options = managementLevelSelect.querySelectorAll('option');
        expect(options.length).toBeGreaterThan(0);
    });

    it('should display length of service by group', () => {
        renderWithFormik();

        expect(screen.getByText('Length of Service')).toBeInTheDocument();
        expect(screen.getByText('By Group Date')).toBeInTheDocument();
        expect(screen.getByText('4 years 3 months')).toBeInTheDocument();
    });

    it('should show chevron-down icon when panel is open', () => {
        const { container } = renderWithFormik({ isOpen: true });

        const chevronIcon = container.querySelector('.fa-chevron-down');
        expect(chevronIcon).toBeInTheDocument();
    });

    it('should show chevron-right icon when panel is closed', () => {
        const { container } = renderWithFormik({ isOpen: false });

        const chevronIcon = container.querySelector('.fa-chevron-right');
        expect(chevronIcon).toBeInTheDocument();
    });

    it('should handle empty arrays for dropdowns', () => {
        renderWithFormik({
            jobLevels: [],
            managementLevels: [],
            subCompanies: [],
            jobTitles: [],
        });

        expect(screen.getByTestId('select-SubCompanyId')).toBeInTheDocument();
        expect(screen.getByTestId('select-JobTitleId')).toBeInTheDocument();
        expect(screen.getByTestId('select-JobLevelId')).toBeInTheDocument();
        expect(screen.getByTestId('select-ManagementLevelId')).toBeInTheDocument();
    });

    it('should handle null employeeData', () => {
        renderWithFormik({ employeeData: null });

        expect(screen.getByText('Information')).toBeInTheDocument();
    });

    it('should render select fields as searchable and clearable', () => {
        renderWithFormik();

        const jobTitleSelect = screen.getByTestId('select-JobTitleId');
        expect(jobTitleSelect).toHaveAttribute('data-searchable', 'true');
        expect(jobTitleSelect).toHaveAttribute('data-clearable', 'true');
    });

    it('should render date field labels correctly', () => {
        renderWithFormik();

        expect(screen.getByText('Team Date')).toBeInTheDocument();
        expect(screen.getByText('Promotion Date')).toBeInTheDocument();
        expect(screen.getByText('Group Date')).toBeInTheDocument();
        expect(screen.getByText('Company Date')).toBeInTheDocument();
        expect(screen.getByText('Confirmed Date')).toBeInTheDocument();
    });

    it('should filter out options without value key', () => {
        const invalidSubCompanies = [
            { subCompanyId: 0, name: 'Invalid' },
            { subCompanyId: 1, name: 'Valid Company' },
        ];

        renderWithFormik({ subCompanies: invalidSubCompanies });

        const subCompanySelect = screen.getByTestId('select-SubCompanyId');
        const options = Array.from(subCompanySelect.querySelectorAll('option'))
            .filter(opt => opt.value !== '');
        
        // Should only have 1 valid option (excluding the empty option)
        expect(options.length).toBe(1);
    });
});

