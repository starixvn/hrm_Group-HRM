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
                            name='allowClaimOT'
                            type='checkbox'
                            className='form-control'
                            checked={employeeData && employeeData.allowClaimOT || false}
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
                            value={employeeData && employeeData.specialOT || ''}
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
                            name='workingDaysPerMonth'
                            type='text'
                            className='form-control'
                            value={employeeData && employeeData.workingDaysPerMonth || ''}
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
                            name='workingHoursPerWeek'
                            type='text'
                            className='form-control'
                            value={employeeData && employeeData.workingHoursPerWeek || ''}
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
                            name='workingHoursPerDay'
                            type='text'
                            className='form-control'
                            value={employeeData && employeeData.workingHoursPerDay || ''}
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
                            name='numberOfOffday'
                            type='text'
                            className='form-control'
                            value={employeeData && employeeData.numberOfOffday || ''}
                        />
                    </div>
                </div>

                {/* Read-only Leave Balance fields */}
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Leave Balance
                        </label>
                    </div>
                    <div className='input-form'>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                            <span style={{ padding: '8px', backgroundColor: '#f8f9fa', border: '1px solid #dee2e6', borderRadius: '4px', flex: 1 }}>
                                {employeeData && employeeData.leaveBalance || '25.00 day(s)'}
                            </span>
                            <a href="#" style={{ color: '#007bff', textDecoration: 'none', fontSize: '14px' }}>Update</a>
                        </div>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Compensation Leave Balance
                        </label>
                    </div>
                    <div className='input-form'>
                        <div style={{ padding: '8px', backgroundColor: '#f8f9fa', border: '1px solid #dee2e6', borderRadius: '4px' }}>
                            {employeeData && employeeData.compensationLeaveBalance || '0.00 day(s)'}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Policy;
