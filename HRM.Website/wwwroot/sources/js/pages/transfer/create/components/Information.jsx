import React from 'react';
import { Field } from 'formik';
import DatePicker from 'react-datepicker';

const Information = ({ isOpen, onToggle, employeeData }) => {
    return (
        <div className={`panel-group ${!isOpen ? 'collapsed' : ''}`}>
            <div className='panel-heading' onClick={onToggle}>
                <h4 className='panel-title'>
                    <i className={`fa fa-chevron-${isOpen ? 'down' : 'right'}`}></i>
                    Information
                </h4>
            </div>
            <div className={`panel-body ${!isOpen ? 'collapsed' : ''}`}>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Sub Company
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='subCompany'
                            as='select'
                            className='form-control'
                        >
                            <option value=''>--Select a Sub Company--</option>
                        </Field>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Team
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='team'
                            as='select'
                            className='form-control'
                        >
                            <option value=''>Select Team</option>
                        </Field>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Team Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='teamDate'>
                                {({ field, form }) => (
                                    <DatePicker
                                        selected={field.value ? new Date(field.value) : new Date()}
                                        onChange={(date) => form.setFieldValue(field.name, date)}
                                        dateFormat="dd MMM, yyyy"
                                        className='form-control'
                                        placeholderText=""
                                    />
                                )}
                            </Field>
                            <i className='fa fa-calendar calendar-icon'></i>
                        </div>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Job Title
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='jobTitle'
                            as='select'
                            className='form-control'
                        >
                            <option value=''>Select Job Title</option>
                        </Field>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Job Level
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='jobLevel'
                            as='select'
                            className='form-control'
                        >
                            <option value=''>Select Job Level</option>
                        </Field>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Management Grade
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='managementGrade'
                            as='select'
                            className='form-control'
                        >
                            <option value=''>Select Management Grade</option>
                        </Field>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Promotion Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='promotionDate'>
                                {({ field, form }) => (
                                    <DatePicker
                                        selected={field.value && field.value !== '' ? new Date(field.value) : null}
                                        onChange={(date) => form.setFieldValue(field.name, date)}
                                        dateFormat="dd MMM, yyyy"
                                        className='form-control'
                                        placeholderText=""
                                    />
                                )}
                            </Field>
                            <i className='fa fa-calendar calendar-icon'></i>
                        </div>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Group Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='groupDate'>
                                {({ field, form }) => (
                                    <DatePicker
                                        selected={field.value && field.value !== '' ? new Date(field.value) : null}
                                        onChange={(date) => form.setFieldValue(field.name, date)}
                                        dateFormat="dd MMM, yyyy"
                                        className='form-control'
                                        placeholderText=""
                                    />
                                )}
                            </Field>
                            <i className='fa fa-calendar calendar-icon'></i>
                        </div>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Company Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='companyDate'>
                                {({ field, form }) => (
                                    <DatePicker
                                        selected={field.value ? new Date(field.value) : new Date()}
                                        onChange={(date) => form.setFieldValue(field.name, date)}
                                        dateFormat="dd MMM, yyyy"
                                        className='form-control'
                                        placeholderText=""
                                    />
                                )}
                            </Field>
                            <i className='fa fa-calendar calendar-icon'></i>
                        </div>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Confirmed Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='confirmedDate'>
                                {({ field, form }) => (
                                    <DatePicker
                                        selected={field.value && field.value !== '' ? new Date(field.value) : null}
                                        onChange={(date) => form.setFieldValue(field.name, date)}
                                        dateFormat="dd MMM, yyyy"
                                        className='form-control'
                                        placeholderText=""
                                    />
                                )}
                            </Field>
                            <i className='fa fa-calendar calendar-icon'></i>
                        </div>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Length of Service
                        </label>
                    </div>
                    <div className='select-form'>
                        <div>
                            <div style={{ marginBottom: '5px', fontSize: '12px', color: '#666' }}>
                                By Group Date
                            </div>
                            <div className='input-form'>
                                <div className='read-only-field'>
                                    {employeeData && employeeData.lengthOfServiceByGroup}
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};


export default Information;
