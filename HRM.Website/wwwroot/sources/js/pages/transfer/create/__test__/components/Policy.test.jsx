import { render, fireEvent, screen } from '@testing-library/react';
import React from 'react';
import { Formik, Form } from 'formik';
import Policy from '../../components/Policy';

jest.spyOn(console, 'warn').mockImplementation(() => {});

describe('Policy Component', () => {
    const mockOnToggle = jest.fn();
    const mockEmployeeData = {
        CanOT: true,
        SpecialOT: '1.5',
        WorkingDays: 22,
        WorkingHoursbyWeek: 40,
        WorkingHours: 8,
        OffDaysbyWeek: 1,
        AnnualLeaveBalance: 12,
        KeepInViewBalance: 5,
    };

    const renderWithFormik = (props = {}) => {
        const defaultProps = {
            isOpen: true,
            onToggle: mockOnToggle,
            employeeData: mockEmployeeData,
            ...props,
        };

        return render(
            <Formik
                initialValues={mockEmployeeData}
                onSubmit={jest.fn()}
            >
                <Form>
                    <Policy {...defaultProps} />
                </Form>
            </Formik>
        );
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('should render policy panel', () => {
        renderWithFormik();

        expect(screen.getByText('Policy')).toBeInTheDocument();
        expect(screen.getByText('Allow Claim OT')).toBeInTheDocument();
        expect(screen.getByText('Special OT')).toBeInTheDocument();
        expect(screen.getByText('Working days per Month')).toBeInTheDocument();
    });

    it('should toggle panel when header is clicked', () => {
        renderWithFormik();

        const panelHeader = screen.getByText('Policy').closest('.panel-heading');
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

    it('should render all policy fields', () => {
        renderWithFormik();

        expect(screen.getByLabelText('Allow Claim OT')).toBeInTheDocument();
        expect(screen.getByLabelText('Special OT')).toBeInTheDocument();
        expect(screen.getByLabelText('Working days per Month')).toBeInTheDocument();
        expect(screen.getByLabelText('Working hours per week')).toBeInTheDocument();
        expect(screen.getByLabelText('Working hours per day')).toBeInTheDocument();
        expect(screen.getByLabelText('Number of Offday')).toBeInTheDocument();
        expect(screen.getByText('Leave Balance')).toBeInTheDocument();
        expect(screen.getByText('Compensation Leave Balance')).toBeInTheDocument();
    });

    it('should render checkbox for CanOT', () => {
        renderWithFormik();

        const canOTCheckbox = screen.getByLabelText('Allow Claim OT');
        expect(canOTCheckbox).toHaveAttribute('type', 'checkbox');
        expect(canOTCheckbox).toHaveAttribute('name', 'CanOT');
    });

    it('should render text inputs for editable fields', () => {
        renderWithFormik();

        expect(screen.getByLabelText('Special OT')).toHaveAttribute('type', 'text');
        expect(screen.getByLabelText('Working days per Month')).toHaveAttribute('type', 'text');
        expect(screen.getByLabelText('Working hours per week')).toHaveAttribute('type', 'text');
        expect(screen.getByLabelText('Working hours per day')).toHaveAttribute('type', 'text');
        expect(screen.getByLabelText('Number of Offday')).toHaveAttribute('type', 'text');
    });

    it('should display annual leave balance as read-only', () => {
        renderWithFormik();

        expect(screen.getByText('12 day(s)')).toBeInTheDocument();
    });

    it('should display compensation leave balance as read-only', () => {
        renderWithFormik();

        expect(screen.getByText('5 day(s)')).toBeInTheDocument();
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

    it('should handle null employeeData for annual leave balance', () => {
        renderWithFormik({ employeeData: null });

        expect(screen.getByText('0 day(s)')).toBeInTheDocument();
    });

    it('should handle undefined annual leave balance', () => {
        const dataWithoutLeave = { ...mockEmployeeData, AnnualLeaveBalance: undefined };
        renderWithFormik({ employeeData: dataWithoutLeave });

        expect(screen.getByText('0 day(s)')).toBeInTheDocument();
    });

    it('should handle undefined compensation leave balance', () => {
        const dataWithoutCompLeave = { ...mockEmployeeData, KeepInViewBalance: undefined };
        renderWithFormik({ employeeData: dataWithoutCompLeave });

        expect(screen.getByText('0 day(s)')).toBeInTheDocument();
    });

    it('should allow input in text fields', () => {
        renderWithFormik();

        const specialOTField = screen.getByLabelText('Special OT');
        fireEvent.change(specialOTField, { target: { value: '2.0' } });

        expect(specialOTField.value).toBe('2.0');
    });

    it('should toggle checkbox', () => {
        renderWithFormik();

        const canOTCheckbox = screen.getByLabelText('Allow Claim OT');
        
        // Initial state is checked (from mockEmployeeData)
        expect(canOTCheckbox.checked).toBe(true);

        // Toggle to unchecked
        fireEvent.click(canOTCheckbox);
        expect(canOTCheckbox.checked).toBe(false);

        // Toggle back to checked
        fireEvent.click(canOTCheckbox);
        expect(canOTCheckbox.checked).toBe(true);
    });

    it('should render all field names correctly', () => {
        renderWithFormik();

        const specialOTField = screen.getByLabelText('Special OT');
        expect(specialOTField).toHaveAttribute('name', 'SpecialOT');

        const workingDaysField = screen.getByLabelText('Working days per Month');
        expect(workingDaysField).toHaveAttribute('name', 'WorkingDays');

        const workingHoursByWeekField = screen.getByLabelText('Working hours per week');
        expect(workingHoursByWeekField).toHaveAttribute('name', 'WorkingHoursbyWeek');

        const workingHoursField = screen.getByLabelText('Working hours per day');
        expect(workingHoursField).toHaveAttribute('name', 'WorkingHours');

        const offDaysField = screen.getByLabelText('Number of Offday');
        expect(offDaysField).toHaveAttribute('name', 'OffDaysbyWeek');
    });
});

