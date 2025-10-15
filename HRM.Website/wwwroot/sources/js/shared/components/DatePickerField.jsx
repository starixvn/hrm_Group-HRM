import React from 'react';
import { Field } from 'formik';
import DatePicker from 'react-datepicker';

const DatePickerField = ({ name, ...props }) => (
    <div className='date-picker-wrapper'>
        <Field name={name}>
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
                    {...props}
                />
            )}
        </Field>
        <i className="fa fa-calendar calendar-icon" />
    </div>
);

export default DatePickerField;
