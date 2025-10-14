import React, { useState } from 'react';
import { Field } from 'formik';
import DatePicker from 'react-datepicker';

const Personal = ({ isOpen, onToggle, employeeData }) => {
    const [nationalityOptions] = useState([
        { id: 'VN', text: 'Vietnam' },
        { id: 'US', text: 'United States' },
        { id: 'UK', text: 'United Kingdom' },
        { id: 'JP', text: 'Japan' },
        { id: 'KR', text: 'South Korea' },
        { id: 'CN', text: 'China' },
        { id: 'TH', text: 'Thailand' },
        { id: 'SG', text: 'Singapore' },
        { id: 'MY', text: 'Malaysia' },
        { id: 'ID', text: 'Indonesia' },
        { id: 'PH', text: 'Philippines' },
        { id: 'IN', text: 'India' },
        { id: 'AU', text: 'Australia' },
        { id: 'CA', text: 'Canada' },
        { id: 'FR', text: 'France' },
        { id: 'DE', text: 'Germany' },
        { id: 'IT', text: 'Italy' },
        { id: 'ES', text: 'Spain' },
        { id: 'NL', text: 'Netherlands' },
        { id: 'BE', text: 'Belgium' }
    ]);
    const [nationalitySearch, setNationalitySearch] = useState('');
    const [filteredNationalities, setFilteredNationalities] = useState(nationalityOptions);

    const handleNationalitySearch = (searchTerm) => {
        setNationalitySearch(searchTerm);
        if (searchTerm === '') {
            setFilteredNationalities(nationalityOptions);
        } else {
            const filtered = nationalityOptions.filter(option =>
                option.text.toLowerCase().includes(searchTerm.toLowerCase())
            );
            setFilteredNationalities(filtered);
        }
    };

    return (
        <div className={`panel-group ${!isOpen ? 'collapsed' : ''}`}>
            <div className='panel-heading' onClick={onToggle}>
                <h4 className='panel-title'>
                    <i className={`fa fa-chevron-${isOpen ? 'down' : 'right'}`}></i>
                    Personal
                </h4>
            </div>
            <div className={`panel-body ${!isOpen ? 'collapsed' : ''}`}>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Employee ID
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='employeeId'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Employee Card
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='employeeCard'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Domain Username
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='domainUsername'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Full Name
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='fullName'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Nickname
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='nickName'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Date Of Birth
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='dateOfBirth'>
                                {({ field, form }) => (
                                    <DatePicker
                                        selected={field.value && field.value !== '' ? new Date(field.value) : null}
                                        onChange={(date) => form.setFieldValue(field.name, date)}
                                        dateFormat="dd MMM, yyyy"
                                        className='form-control'
                                        placeholderText=""
                                        showYearDropdown
                                        showMonthDropdown
                                        dropdownMode="select"
                                    />
                                )}
                            </Field>
                            <i className="fa fa-calendar calendar-icon" />
                        </div>
                    </div>

                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Place Of Birth
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='placeOfBirth'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Ethnic
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='ethnic'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Native
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='native'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Religion
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='religion'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Gender
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='gender'
                            as='select'
                            className='form-control'
                        >
                            <option value='2'>Unknown</option>
                            <option value='0'>Female</option>
                            <option value='1'>Male</option>
                        </Field>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Marriage
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='maritalStatus'
                            as='select'
                            className='form-control'
                        >
                            <option value=''>Select Status</option>
                            <option value='Single'>Single</option>
                            <option value='Married'>Married</option>
                            <option value='Divorced'>Divorced</option>
                            <option value='Widowed'>Widowed</option>
                        </Field>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Nationality
                        </label>
                    </div>
                    <div className='input-form'>
                        <div className='searchable-select'>
                            <Field
                                name='nationality'
                                as='select'
                                className='form-control'
                            >
                                <option value=''>Select Nationality</option>
                                {filteredNationalities.map(option => (
                                    <option key={option.id} value={option.id}>
                                        {option.text}
                                    </option>
                                ))}
                            </Field>
                        </div>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            ID Card No
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='idCardNumber'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            ID Card Issue Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='idCardIssueDate'>
                                {({ field, form }) => (
                                    <DatePicker
                                        selected={field.value && field.value !== '' ? new Date(field.value) : null}
                                        onChange={(date) => form.setFieldValue(field.name, date)}
                                        dateFormat="dd MMM, yyyy"
                                        className='form-control'
                                        placeholderText=""
                                        showYearDropdown
                                        showMonthDropdown
                                        dropdownMode="select"
                                    />
                                )}
                            </Field>
                            <i className="fa fa-calendar calendar-icon" />
                        </div>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            ID Card Place Of Issue
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='idCardIssuePlace'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Passport No
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='passportNumber'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>


                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Passport Exp
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='passportExpiryDate'>
                                {({ field, form }) => (
                                    <DatePicker
                                        selected={field.value && field.value !== '' ? new Date(field.value) : null}
                                        onChange={(date) => form.setFieldValue(field.name, date)}
                                        dateFormat="dd MMM, yyyy"
                                        className='form-control'
                                        placeholderText=""
                                        showYearDropdown
                                        showMonthDropdown
                                        dropdownMode="select"
                                    />
                                )}
                            </Field>
                            <i className="fa fa-calendar calendar-icon" />
                        </div>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Qualification
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='qualification'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Major
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='major'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Personal;
