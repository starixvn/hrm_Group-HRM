import React from 'react';
import { Field } from 'formik';

const Personal = ({ isOpen, onToggle }) => {
    return (
        <div className='panel-group'>
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
                            <span className='require'>*</span>
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
                            Full Name
                            <span className='require'>*</span>
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
                            Nick Name
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
                            Gender
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='gender'
                            as='select'
                            className='form-control'
                        >
                            <option value=''>Select Gender</option>
                            <option value='Male'>Male</option>
                            <option value='Female'>Female</option>
                        </Field>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Date of Birth
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <Field
                            name='dateOfBirth'
                            type='date'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Marital Status
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
                        <Field
                            name='nationality'
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
                            ID Card Number
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
                        <Field
                            name='idCardIssueDate'
                            type='date'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            ID Card Issue Place
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
                            Passport Number
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
                            Passport Issue Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <Field
                            name='passportIssueDate'
                            type='date'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Passport Expiry Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <Field
                            name='passportExpiryDate'
                            type='date'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Passport Issue Place
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='passportIssuePlace'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Tax Code
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='taxCode'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Social Insurance Number
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='socialInsuranceNumber'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Health Insurance Number
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='healthInsuranceNumber'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Bank Account Number
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='bankAccountNumber'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Bank Name
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='bankName'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Bank Branch
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='bankBranch'
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
