import React from 'react';
import { Field } from 'formik';

const Contact = ({ isOpen, onToggle }) => {
    return (
        <div className={`panel-group ${!isOpen ? 'collapsed' : ''}`}>
            <div className='panel-heading' onClick={onToggle}>
                <h4 className='panel-title'>
                    <i className={`fa fa-chevron-${isOpen ? 'down' : 'right'}`}></i>
                    Contact
                </h4>
            </div>
            <div className={`panel-body ${!isOpen ? 'collapsed' : ''}`}>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Cellphone
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='cellphone'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Home Phone
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='homePhone'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Work Email
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='workEmail'
                            type='email'
                            className='form-control'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Permanent Address
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='permanentAddress'
                            as='textarea'
                            className='form-control'
                            rows='3'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Current Address
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='currentAddress'
                            as='textarea'
                            className='form-control'
                            rows='3'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Pickup Remark
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='pickupRemark'
                            as='textarea'
                            className='form-control'
                            rows='3'
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Letstalk
                        </label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='letstalk'
                            type='text'
                            className='form-control'
                        />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Contact;
