import React from 'react';
import { Field } from 'formik';

const Policy = ({ isOpen, onToggle }) => {
    return (
        <div className='panel-group'>
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
                                Health Insurance
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='healthInsurance'
                                as='select'
                                className='form-control'
                            >
                            </Field>
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Social Insurance
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='socialInsurance'
                                as='select'
                                className='form-control'
                            >
                            </Field>
                        </div>
                    </div>
                </div>
        </div>
    );
};

export default Policy;
