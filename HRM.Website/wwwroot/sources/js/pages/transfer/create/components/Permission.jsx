import React from 'react';
import { Field } from 'formik';

const Permission = ({ isOpen, onToggle, employeeData }) => {
    return (
        <div className={`panel-group ${!isOpen ? 'collapsed' : ''}`}>
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
                            Roles
                        </label>
                    </div>
                    <div className='input-form'>
                        <div className='roles-grid'>
                            <div className='role-item'>
                                <Field
                                    name='member'
                                    type='checkbox'
                                    id='member'
                                    checked={employeeData && employeeData.member || true}
                                />
                                <label htmlFor='member'>Member</label>
                            </div>
                            
                            <div className='role-item'>
                                <Field
                                    name='verifier'
                                    type='checkbox'
                                    id='verifier'
                                    checked={employeeData && employeeData.verifier || true}
                                />
                                <label htmlFor='verifier'>Verifier</label>
                            </div>
                            
                            <div className='role-item disabled'>
                                <Field
                                    name='approver'
                                    type='checkbox'
                                    id='approver'
                                    checked={employeeData && employeeData.approver || false}
                                    disabled={true}
                                />
                                <label htmlFor='approver'>Approver</label>
                            </div>
                            
                            <div className='role-item'>
                                <Field
                                    name='shiftCurrentWeekEditor'
                                    type='checkbox'
                                    id='shiftCurrentWeekEditor'
                                    checked={employeeData && employeeData.shiftCurrentWeekEditor || true}
                                />
                                <label htmlFor='shiftCurrentWeekEditor'>Shift Current Week Editor</label>
                            </div>

                            <div className='role-item'>
                                <Field
                                    name='onBehalf'
                                    type='checkbox'
                                    id='onBehalf'
                                    checked={employeeData && employeeData.onBehalf || true}
                                />
                                <label htmlFor='onBehalf'>On Behalf</label>
                            </div>
                            
                            <div className='role-item'>
                                <Field
                                    name='hr'
                                    type='checkbox'
                                    id='hr'
                                    checked={employeeData && employeeData.hr || true}
                                />
                                <label htmlFor='hr'>HR</label>
                            </div>
                            
                            <div className='role-item'>
                                <Field
                                    name='disciplineManager'
                                    type='checkbox'
                                    id='disciplineManager'
                                    checked={employeeData && employeeData.disciplineManager || true}
                                />
                                <label htmlFor='disciplineManager'>Discipline Manager</label>
                            </div>
                            
                            <div className='role-item'>
                                <Field
                                    name='shiftArrange'
                                    type='checkbox'
                                    id='shiftArrange'
                                    checked={employeeData && employeeData.shiftArrange || true}
                                />
                                <label htmlFor='shiftArrange'>Shift Arrange</label>
                            </div>

                            <div className='role-item'>
                                <Field
                                    name='localHr'
                                    type='checkbox'
                                    id='localHr'
                                    checked={employeeData && employeeData.localHr || true}
                                />
                                <label htmlFor='localHr'>Local HR</label>
                            </div>
                            
                            <div className='role-item'>
                                <Field
                                    name='expatHr'
                                    type='checkbox'
                                    id='expatHr'
                                    checked={employeeData && employeeData.expatHr || true}
                                />
                                <label htmlFor='expatHr'>Expat HR</label>
                            </div>
                            
                            <div className='role-item'>
                                <Field
                                    name='hrAssistant'
                                    type='checkbox'
                                    id='hrAssistant'
                                    checked={employeeData && employeeData.hrAssistant || false}
                                />
                                <label htmlFor='hrAssistant'>HR Assistant</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Permission;
