import React from 'react';
import { Field } from 'formik';

const Military = ({ isOpen, onToggle }) => {
    return (
        <div className='panel-group'>
            <div className='panel-heading' onClick={onToggle}>
                <h4 className='panel-title'>
                    <i className={`fa fa-chevron-${isOpen ? 'down' : 'right'}`}></i>
                    Military
                </h4>
            </div>
            <div className={`panel-body ${!isOpen ? 'collapsed' : ''}`}>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Military Service
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='militaryService'
                                as='select'
                                className='form-control'
                            >
                            </Field>
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Military ID
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='militaryId'
                                type='text'
                                className='form-control'
                            />
                        </div>
                    </div>
                </div>
        </div>
    );
};

export default Military;
