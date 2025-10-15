import React from 'react';
import { Field } from 'formik';
import DatePickerField from '../../../../shared/components/DatePickerField';
import TreeMultiselect from '../../../../shared/components/react-tree-multiselect/TreeMultiselect';

const Information = ({ isOpen, onToggle, employeeData, teams, onTeamChange, selectedTeam, jobLevels, managementLevels, subCompanies, jobTitles }) => {
    return (
        <div className={`panel-group ${!isOpen ? 'collapsed' : ''}`}>
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
                            Sub Company
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='subCompanyId'
                            as='select'
                            className='form-control'
                            value={employeeData && employeeData.subCompanyId}
                        >
                            <option value=''>--Select a Sub Company--</option>
                            {subCompanies && subCompanies.filter(company => company.id).map(company => (
                                <option key={`subcompany-${company.id}`} value={company.id}>
                                    {company.name || company.text}
                                </option>
                            ))}
                        </Field>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Team
                        </label>
                    </div>
                    <div className='select-form'>
                        <TreeMultiselect
                            data={teams || []}
                            isSelectedAll={false}
                            hasSearchBox={true}
                            leafOnly={true}
                            singleSelect={true}
                            onChange={onTeamChange}
                            selectedItems={selectedTeam ? [selectedTeam] : []}
                        />
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Team Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <DatePickerField name='teamDate' />
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Job Title
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='jobTitle'
                            as='select'
                            className='form-control'
                            value={employeeData && employeeData.jobTitle}
                        >
                            <option value=''>--Select a Job Title--</option>
                            {jobTitles && jobTitles.filter(jobTitle => jobTitle.id).map(jobTitle => (
                                <option key={`jobtitle-${jobTitle.id}`} value={jobTitle.id}>
                                    {jobTitle.text || jobTitle.name}
                                </option>
                            ))}
                        </Field>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Job Level
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='jobLevel'
                            as='select'
                            className='form-control'
                            value={employeeData && employeeData.jobLevel}
                        >
                            <option value=''>--Select a Job Level--</option>
                            {jobLevels && jobLevels.filter(jobLevel => jobLevel.id).map(jobLevel => (
                                <option key={`joblevel-${jobLevel.id}`} value={jobLevel.id}>
                                    {jobLevel.name || jobLevel.text}
                                </option>
                            ))}
                        </Field>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Management Grade
                        </label>
                    </div>
                    <div className='select-form'>
                        <Field
                            name='managementGrade'
                            as='select'
                            className='form-control'
                            value={employeeData && employeeData.managementGrade}
                        >
                            <option value=''>--Select a M Grade--</option>
                            {managementLevels && managementLevels.filter(level => level.id).map(level => (
                                <option key={`mgmtlevel-${level.id}`} value={level.id}>
                                    {level.name || level.text}
                                </option>
                            ))}
                        </Field>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Promotion Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <DatePickerField name='promotionDate' />
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Group Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <DatePickerField name='groupDate' />
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Company Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        '<DatePickerField name='companyDate' />
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Confirmed Date
                        </label>
                    </div>
                    <div className='calendar-form'>
                        <DatePickerField name='confirmedDate' />
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Length of Service
                        </label>
                    </div>
                    <div className='select-form'>
                        <div>
                            <div style={{ marginBottom: '5px', fontSize: '12px', color: '#666' }}>
                                By Group Date
                            </div>
                            <div className='input-form'>
                                <div className='read-only-field'>
                                    {employeeData && employeeData.lengthOfServiceByGroup}
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};


export default Information;
