import React from 'react';
import { Field } from 'formik';

const Information = ({ isOpen, onToggle }) => {
    return (
        <div className='panel-group'>
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
                                Department
                                <span className='require'>*</span>
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='department'
                                as='select'
                                className='form-control'
                            >
                            </Field>
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Position
                                <span className='require'>*</span>
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='position'
                                type='text'
                                className='form-control'
                            />
                        </div>
                    </div>
                </div>
        </div>
    );
};

export default Information;
