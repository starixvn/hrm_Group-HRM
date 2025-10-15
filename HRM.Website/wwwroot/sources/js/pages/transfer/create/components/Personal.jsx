import React, { useState, useEffect } from 'react';
import { Field } from 'formik';
import DatePickerField from '../../../../shared/components/DatePickerField';

const Personal = ({ isOpen, onToggle, employeeData, countries, onGenerateEmployeeId }) => {
    const [filteredCountryCodes, setFilteredCountryCodes] = useState(countries || []);

    useEffect(() => {
        setFilteredCountryCodes(countries || []);
    }, [countries]);

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
                                name='Code'
                                type='text'
                                className='form-control'
                                value={employeeData && employeeData.Code}
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
                            name='EmployeeCard'
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
                            name='DomainUsername'
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
                            name='FullName'
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
                            name='NickName'
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
                        <DatePickerField name='DateOfBirth' />
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
                            name='PlaceOfBirth'
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
                            name='Ethnic'
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
                            name='Native'
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
                            name='Religion'
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
                            name='Gender'
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
                            name='Marriage'
                            as='select'
                            className='form-control'
                            value={employeeData && employeeData.Marriage}
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
                                name='CountryCode'
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
                            name='CardNo'
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
                        <DatePickerField name='DateOfIdCard' />
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
                            name='PlaceOfIdCard'
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
                            name='PassportNo'
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
                        <DatePickerField name='PassportExp' />
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
                            name='Qualification'
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
                            name='Major'
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
