import React from 'react';
import { Field } from 'formik';

const Permission = ({ isOpen, onToggle }) => {
    return (
        <div className='panel-group'>
            <div className='panel-heading' onClick={onToggle}>
                <h4 className='panel-title'>
                    <i className={`fa fa-chevron-${isOpen ? 'down' : 'right'}`}></i>
                    Permission
                </h4>
            </div>
            <div className={`panel-body ${!isOpen ? 'collapsed' : ''}`}>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Access Level
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='accessLevel'
                                as='select'
                                className='form-control'
                            >
                            </Field>
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                System Permissions
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field
                                name='systemPermissions'
                                type='text'
                                className='form-control'
                            />
                        </div>
                    </div>
                </div>
        </div>
    );
};

export default Permission;
