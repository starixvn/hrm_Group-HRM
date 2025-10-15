import React, { useState, useEffect } from 'react';
import { Field } from 'formik';
import DatePicker from 'react-datepicker';

const Personal = ({ isOpen, onToggle, employeeData, countries, onGenerateEmployeeId }) => {
    const [countryCodeOptions] = useState(countries || []);
    const [countryCodeSearch, setCountryCodeSearch] = useState('');
    const [filteredCountryCodes, setFilteredCountryCodes] = useState(countries || []);

    useEffect(() => {
        setFilteredCountryCodes(countries || []);
    }, [countries]);

    const handleCountryCodeSearch = searchTerm => {
        setCountryCodeSearch(searchTerm);
        if (searchTerm === '') {
            setFilteredCountryCodes(countryCodeOptions);
        } else {
            const filtered = countryCodeOptions.filter(option =>
                option.countryName.toLowerCase().includes(searchTerm.toLowerCase())
            );
            setFilteredCountryCodes(filtered);
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
                    <div className='input-form'>
                        <div className='input-group-generate'>
                            <Field
                                name='code'
                                type='text'
                                className='form-control'
                                value={employeeData && employeeData.code}
                            />
                            <button
                                type='button'
                                className='btn btn-primary btn-generate'
                                onClick={onGenerateEmployeeId}
                            >
                                Generate
                            </button>
                        </div>
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
                                        onChange={date => form.setFieldValue(field.name, date)}
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
                            <option key='gender-2' value='2'>Unknown</option>
                            <option key='gender-0' value='0'>Female</option>
                            <option key='gender-1' value='1'>Male</option>
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
                            name='marriage'
                            as='select'
                            className='form-control'
                            value={employeeData && employeeData.marriage}
                        >
                            <option key='marriage-0' value='0'>--Select Marriage Status--</option>
                            <option key='marriage-1' value='1'>Married</option>
                            <option key='marriage-2' value='2'>Divorced</option>
                            <option key='marriage-3' value='3'>Single</option>
                            <option key='marriage-4' value='4'>Widow</option>
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
                                name='countryCode'
                                as='select'
                                className='form-control'
                            >
                                <option value=''>Select Nationality</option>
                                {filteredCountryCodes.map(option => (
                                    <option key={option.countryCode} value={option.countryCode}>
                                        {option.countryName}
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
                                        onChange={date => form.setFieldValue(field.name, date)}
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
                                        onChange={date => form.setFieldValue(field.name, date)}
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
