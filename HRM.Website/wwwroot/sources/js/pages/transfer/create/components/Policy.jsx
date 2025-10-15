import React from 'react';
import { Field } from 'formik';

const Policy = ({ isOpen, onToggle, employeeData }) => {
    return (
        <div className={`panel-group ${!isOpen ? 'collapsed' : ''}`}>
            <div className='panel-heading' onClick={onToggle}>
                <h4 className='panel-title'>
                    <i className={`fa fa-chevron-${isOpen ? 'down' : 'right'}`}></i>
                    Policy
                </h4>
            </div>
            <div className={`panel-body ${!isOpen ? 'collapsed' : ''}`}>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Allow Claim OT
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='canOT'
                            type='checkbox'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Special OT
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='specialOT'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Working days per Month
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='workingDays'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Working hours per week
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='workingHoursbyWeek'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Working hours per day
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='workingHours'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Number of Offday
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='offDaysbyWeek'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Leave Balance
                        </label>
                    </div>
                    <div className='input-form'>
                        <span className='read-only-field'>
                            {(employeeData && employeeData.annualLeaveBalance || 0) + ' day(s)'}
                        </span>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Compensation Leave Balance
                        </label>
                    </div>
                    <div className='input-form'>
                        <div className='read-only-field'>
                            {(employeeData && employeeData.keepInViewBalance || 0) + ' day(s)'}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Policy;
