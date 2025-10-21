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
                            name='Phone'
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
                            name='HomePhone'
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
                            name='Email'
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
                            name='Address'
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
                            name='CurrentAddress'
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
                            name='PickupRemark'
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
                            name='LetsTalk'
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
