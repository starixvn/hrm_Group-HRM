import React from 'react';
import { Field } from 'formik';
import DatePicker from 'react-datepicker';

const Military = ({ isOpen, onToggle, employeeData }) => {
    return (
        <div className={`panel-group ${!isOpen ? 'collapsed' : ''}`}>
            <div className='panel-heading' onClick={onToggle}>
                <h4 className='panel-title'>
                    <i className={`fa fa-chevron-${isOpen ? 'down' : 'right'}`}></i>
                    Military
                </h4>
            </div>
            <div className={`panel-body ${!isOpen ? 'collapsed' : ''}`}>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            From
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='militaryFrom'>
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
                            To
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <div className='date-picker-wrapper'>
                            <Field name='militaryTo'>
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
            </div>
        </div>
    );
};

export default Military;
