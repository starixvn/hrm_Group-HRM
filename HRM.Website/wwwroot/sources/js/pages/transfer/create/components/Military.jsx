import React from 'react';
import { Field } from 'formik';

const Military = ({ isOpen, onToggle, employeeData }) => {
    return (
        <div className={`panel-group ${!isOpen ? 'collapsed' : ''}`}>
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
                            From
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <Field
                            name='militaryFrom'
                            type='date'
                            className='form-control'
                            value={employeeData && employeeData.militaryFrom || ''}
                        />
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            To
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <Field
                            name='militaryTo'
                            type='date'
                            className='form-control'
                            value={employeeData && employeeData.militaryTo || ''}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Military;
