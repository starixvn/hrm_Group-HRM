import { render, fireEvent, screen } from '@testing-library/react';
import React from 'react';
import { Formik, Form } from 'formik';
import Personal from '../../components/Personal';

jest.spyOn(console, 'warn').mockImplementation(() => {});

jest.mock('../../../../../shared/components/DatePickerField', () => {
    return function DatePickerField({ name }) {
        return <input data-testid={`datepicker-${name}`} name={name} />;
    };
});

jest.mock('../../../../../shared/components/SelectField', () => {
    return function SelectField({ name, options }) {
        return (
            <select data-testid={`select-${name}`} name={name}>
                {options.map(opt => (
                    <option key={opt.value} value={opt.value}>
                        {opt.label}
                    </option>
                ))}
            </select>
        );
    };
});

describe('Personal Component', () => {
    const mockEmployeeData = {
        Code: 'EMP001',
        EmployeeCard: 'CARD001',
        DomainUsername: 'testuser',
        FullName: 'John Doe',
        NickName: 'Johnny',
        DateOfBirth: '1990-01-01',
        PlaceOfBirth: 'Hanoi',
        Ethnic: 'Kinh',
        Native: 'Vietnamese',
        Religion: 'None',
        Gender: 1,
        Marriage: 3,
        CountryCode: 'VN',
        CardNo: '123456789',
        DateOfIdCard: '2015-01-01',
        PlaceOfIdCard: 'Hanoi',
        PassportNo: 'P123456',
        PassportExp: '2025-01-01',
        Qualification: 'Bachelor',
        Major: 'Computer Science',
    };

    const mockCountries = [
        { countryCode: 'VN', countryName: 'Vietnam' },
        { countryCode: 'US', countryName: 'United States' },
    ];

    const mockOnToggle = jest.fn();
    const mockOnGenerateEmployeeId = jest.fn();

    const renderWithFormik = (props = {}) => {
        const defaultProps = {
            isOpen: true,
            onToggle: mockOnToggle,
            employeeData: mockEmployeeData,
            countries: mockCountries,
            onGenerateEmployeeId: mockOnGenerateEmployeeId,
            ...props,
        };

        return render(
            <Formik
                initialValues={mockEmployeeData}
                onSubmit={jest.fn()}
            >
                <Form>
                    <Personal {...defaultProps} />
                </Form>
            </Formik>
        );
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('should render personal panel', () => {
        renderWithFormik();

        expect(screen.getByText('Personal')).toBeInTheDocument();
        expect(screen.getByText('Employee ID')).toBeInTheDocument();
        expect(screen.getByText('Full Name')).toBeInTheDocument();
        expect(screen.getByText('Nickname')).toBeInTheDocument();
    });

    it('should toggle panel when header is clicked', () => {
        renderWithFormik();

        const panelHeader = screen.getByText('Personal').closest('.panel-heading');
        fireEvent.click(panelHeader);

        expect(mockOnToggle).toHaveBeenCalled();
    });

    it('should render collapsed when isOpen is false', () => {
        const { container } = renderWithFormik({ isOpen: false });

        const panelGroup = container.querySelector('.panel-group');
        expect(panelGroup).toHaveClass('collapsed');
    });

    it('should render employee data fields', () => {
        renderWithFormik();

        expect(screen.getByDisplayValue('EMP001')).toBeInTheDocument();
        expect(screen.getByDisplayValue('John Doe')).toBeInTheDocument();
        expect(screen.getByDisplayValue('Johnny')).toBeInTheDocument();
    });

    it('should call onGenerateEmployeeId when generate button is clicked', () => {
        renderWithFormik();

        const generateButton = screen.getByText('Generate');
        fireEvent.click(generateButton);

        expect(mockOnGenerateEmployeeId).toHaveBeenCalled();
    });

    it('should render gender select field', () => {
        renderWithFormik();

        const genderSelect = screen.getByTestId('select-Gender');
        expect(genderSelect).toBeInTheDocument();
        expect(genderSelect.querySelectorAll('option').length).toBeGreaterThan(0);
    });

    it('should render marriage select field', () => {
        renderWithFormik();

        const marriageSelect = screen.getByTestId('select-Marriage');
        expect(marriageSelect).toBeInTheDocument();
        expect(marriageSelect.querySelectorAll('option').length).toBeGreaterThan(0);
    });

    it('should render country select field with options', () => {
        renderWithFormik();

        const countrySelect = screen.getByTestId('select-CountryCode');
        expect(countrySelect).toBeInTheDocument();
    });

    it('should render date picker fields', () => {
        renderWithFormik();

        expect(screen.getByTestId('datepicker-DateOfBirth')).toBeInTheDocument();
        expect(screen.getByTestId('datepicker-DateOfIdCard')).toBeInTheDocument();
        expect(screen.getByTestId('datepicker-PassportExp')).toBeInTheDocument();
    });

    it('should render all personal information fields', () => {
        renderWithFormik();

        expect(screen.getByText('Employee Card')).toBeInTheDocument();
        expect(screen.getByText('Domain Username')).toBeInTheDocument();
        expect(screen.getByText('Place Of Birth')).toBeInTheDocument();
        expect(screen.getByText('Ethnic')).toBeInTheDocument();
        expect(screen.getByText('Native')).toBeInTheDocument();
        expect(screen.getByText('Religion')).toBeInTheDocument();
        expect(screen.getByText('ID Card No')).toBeInTheDocument();
        expect(screen.getByText('Passport No')).toBeInTheDocument();
        expect(screen.getByText('Qualification')).toBeInTheDocument();
        expect(screen.getByText('Major')).toBeInTheDocument();
    });

    it('should handle empty countries array', () => {
        renderWithFormik({ countries: [] });

        const countrySelect = screen.getByTestId('select-CountryCode');
        expect(countrySelect).toBeInTheDocument();
    });

    it('should handle null employeeData', () => {
        renderWithFormik({ employeeData: null });

        expect(screen.getByText('Personal')).toBeInTheDocument();
    });
});

