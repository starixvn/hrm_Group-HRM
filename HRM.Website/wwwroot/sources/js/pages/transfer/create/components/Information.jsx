import React from 'react';
import { Field } from 'formik';

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
                            value={employeeData && employeeData.subCompany || ''}
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
                            value={employeeData && employeeData.team || ''}
                        >
                            <option value=''>--Select a Team--</option>
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
                        <Field
                            name='teamDate'
                            type='date'
                            className='form-control'
                            value={employeeData && employeeData.teamDate || ''}
                        />
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
                            value={employeeData && employeeData.jobTitle || ''}
                        >
                            <option value=''>--Select a Job Title--</option>
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
                            value={employeeData && employeeData.jobLevel || ''}
                        >
                            <option value=''>--Select a Job Level--</option>
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
                            value={employeeData && employeeData.managementGrade || ''}
                        >
                            <option value=''>--Select a M Grade--</option>
                        </Field>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Salary Structure
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='salaryStructure'
                            as='select'
                            className='form-control'
                            value={employeeData && employeeData.salaryStructure || ''}
                        >
                            <option value=''>--Select a Salary Structure--</option>
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
                        <Field
                            name='promotionDate'
                            type='date'
                            className='form-control'
                            value={employeeData && employeeData.promotionDate || ''}
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Group Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <Field
                            name='groupDate'
                            type='date'
                            className='form-control'
                            value={employeeData && employeeData.groupDate || ''}
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Company Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <Field
                            name='companyDate'
                            type='date'
                            className='form-control'
                            value={employeeData && employeeData.companyDate || ''}
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Confirmed Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <Field
                            name='confirmedDate'
                            type='date'
                            className='form-control'
                            value={employeeData && employeeData.confirmedDate || ''}
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Resigned Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <Field
                            name='resignedDate'
                            type='date'
                            className='form-control'
                            value={employeeData && employeeData.resignedDate || ''}
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Resigned Reason
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='resignedReason'
                            as='textarea'
                            className='form-control'
                            rows='3'
                            value={employeeData && employeeData.resignedReason || ''}
                        />
                    </div>
                </div>

                {/* Read-only Length of Service section */}
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Length of Service
                        </label>
                    </div>
                    <div className='input-form'>
                        <div style={{ display: 'flex', gap: '20px' }}>
                            <div>
                                <label style={{ fontSize: '12px', color: '#666' }}>By Group Date:</label>
                                <div style={{ padding: '8px', backgroundColor: '#f8f9fa', border: '1px solid #dee2e6', borderRadius: '4px' }}>
                                    {employeeData && employeeData.lengthOfServiceByGroup || '21 YEARS 0 MONTHS'}
                                </div>
                            </div>
                            <div>
                                <label style={{ fontSize: '12px', color: '#666' }}>By Company Date:</label>
                                <div style={{ padding: '8px', backgroundColor: '#f8f9fa', border: '1px solid #dee2e6', borderRadius: '4px' }}>
                                    {employeeData && employeeData.lengthOfServiceByCompany || '21 YEARS 0 MONTHS'}
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
