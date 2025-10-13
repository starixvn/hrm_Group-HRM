import React from 'react';
import { Field } from 'formik';

const Contact = ({ isOpen, onToggle }) => {
    return (
        <div className='panel-group'>
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
                                Email
                                <span className='require'>*</span>
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='email'
                                type='email'
                                className='form-control'
                            />
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Phone Number
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='phone'
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
