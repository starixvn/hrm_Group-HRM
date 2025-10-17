import { render, fireEvent, screen } from '@testing-library/react';
import React from 'react';
import { Formik, Form } from 'formik';
import Contact from '../../components/Contact';

jest.spyOn(console, 'warn').mockImplementation(() => {});

describe('Contact Component', () => {
    const mockOnToggle = jest.fn();
    const mockEmployeeData = {
        Phone: '0123456789',
        HomePhone: '0987654321',
        Email: 'test@example.com',
        Address: '123 Main St',
        CurrentAddress: '456 Current St',
        PickupRemark: 'Near the park',
        LetsTalk: 'letstalk123',
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
                    <Contact {...defaultProps} />
                </Form>
            </Formik>
        );
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('should render contact panel', () => {
        renderWithFormik();

        expect(screen.getByText('Contact')).toBeInTheDocument();
        expect(screen.getByText('Cellphone')).toBeInTheDocument();
        expect(screen.getByText('Home Phone')).toBeInTheDocument();
        expect(screen.getByText('Work Email')).toBeInTheDocument();
    });

    it('should toggle panel when header is clicked', () => {
        renderWithFormik();

        const panelHeader = screen.getByText('Contact').closest('.panel-heading');
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

    it('should render all contact fields', () => {
        renderWithFormik();

        expect(screen.getByLabelText('Cellphone')).toBeInTheDocument();
        expect(screen.getByLabelText('Home Phone')).toBeInTheDocument();
        expect(screen.getByLabelText('Work Email')).toBeInTheDocument();
        expect(screen.getByLabelText('Permanent Address')).toBeInTheDocument();
        expect(screen.getByLabelText('Current Address')).toBeInTheDocument();
        expect(screen.getByLabelText('Pickup Remark')).toBeInTheDocument();
        expect(screen.getByLabelText('Letstalk')).toBeInTheDocument();
    });

    it('should render phone field with correct type', () => {
        renderWithFormik();

        const phoneField = screen.getByLabelText('Cellphone');
        expect(phoneField).toHaveAttribute('type', 'text');
        expect(phoneField).toHaveAttribute('name', 'Phone');
    });

    it('should render email field with correct type', () => {
        renderWithFormik();

        const emailField = screen.getByLabelText('Work Email');
        expect(emailField).toHaveAttribute('type', 'email');
        expect(emailField).toHaveAttribute('name', 'Email');
    });

    it('should render textarea fields', () => {
        renderWithFormik();

        const addressField = screen.getByLabelText('Permanent Address');
        expect(addressField.tagName).toBe('TEXTAREA');
        expect(addressField).toHaveAttribute('name', 'Address');

        const currentAddressField = screen.getByLabelText('Current Address');
        expect(currentAddressField.tagName).toBe('TEXTAREA');
        expect(currentAddressField).toHaveAttribute('name', 'CurrentAddress');

        const pickupRemarkField = screen.getByLabelText('Pickup Remark');
        expect(pickupRemarkField.tagName).toBe('TEXTAREA');
        expect(pickupRemarkField).toHaveAttribute('name', 'PickupRemark');
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

    it('should allow input in text fields', () => {
        renderWithFormik();

        const phoneField = screen.getByLabelText('Cellphone');
        fireEvent.change(phoneField, { target: { value: '9999999999' } });

        expect(phoneField.value).toBe('9999999999');
    });

    it('should allow input in email field', () => {
        renderWithFormik();

        const emailField = screen.getByLabelText('Work Email');
        fireEvent.change(emailField, { target: { value: 'newemail@test.com' } });

        expect(emailField.value).toBe('newemail@test.com');
    });

    it('should allow input in textarea fields', () => {
        renderWithFormik();

        const addressField = screen.getByLabelText('Permanent Address');
        fireEvent.change(addressField, { target: { value: 'New address' } });

        expect(addressField.value).toBe('New address');
    });
});

