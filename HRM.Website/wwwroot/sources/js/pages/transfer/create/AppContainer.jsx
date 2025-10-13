import React, { useState, useEffect } from 'react';
import { Formik, Form, Field } from 'formik';
import axios from 'axios';

import Errors from '../../../shared/components/Errors';
import EmployeeAutocomplete from '../../../shared/components/EmployeeAutocomplete';
import TreeMultiselect from '../../../shared/components/react-tree-multiselect/TreeMultiselect';

import Personal from './components/Personal';
import Military from './components/Military';
import Contact from './components/Contact';
import Information from './components/Information';
import Policy from './components/Policy';
import Permission from './components/Permission';

import {
    resolveClientUrl,
    axiosError,
} from '../../../shared/common.functions';
import { validationSchema, submitTransfer } from '../common.functions';

const AppContainer = () => {
    const [companies, setCompanies] = useState(null);
    const [employees, setEmployees] = useState([]);
    const [teams, setTeams] = useState([]);
    const [selectedTeam, setSelectedTeam] = useState(null);
    const [formKey] = useState(0);
    const [currentStep, setCurrentStep] = useState(1);
    const [panelStates, setPanelStates] = useState({
        personal: true,
        military: true,
        contact: true,
        information: true,
        policy: true,
        permission: true
    });
    const [initialValues, setInitialValues] = useState({
        empId: 0,
        employee: '',
        transferOutCompanyId: '',
        transferInCompanyId: '',
        teamId: '',
        remark: ''
    });

    const getEmployees = query => {
        if (query.trim().length >= 2) {
            axios({
                url: resolveClientUrl('GroupEmployeeList/GroupEmployeeList'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: { keyword: query, isDelete: false, isEnable: true }
            }).then(response => {
                const { data } = response;
                const employeeOptions = data.map(item => {
                    return {
                        id: item.empId,
                        value: `${item.nickName} ( ${item.company})`,
                        fullName: item.fullName,
                        nickName: item.nickName,
                        companyId: item.companyId
                    };
                });

                setEmployees(employeeOptions);
            }).catch(axiosError);
        } else {
            setEmployees([]);
        }
    };

    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios({
            url: resolveClientUrl(`GroupEmployeeList/GetCompanyDropdownList`),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
        })
            .then(response => {
                setCompanies(response.data);
            })
            .catch(axiosError);
    }, []);

    const handleSelectEmployee = (_value, selectedItem) => {
        setInitialValues(prev => ({
            ...prev,
            empId: selectedItem.id,
            employee: selectedItem.value,
            transferOutCompanyId: selectedItem.companyId
        }));
    };

    const handleCompanyChange = (value) => {
        setInitialValues(prev => ({
            ...prev,
            transferInCompanyId: value,
            teamId: '' // Reset team when company changes
        }));

        // Load teams based on selected company
        if (value) {
            loadTeamsByCompany(value);
        } else {
            setTeams([]);
        }
    };

    const loadTeamsByCompany = (companyId) => {
        axios({
            url: resolveClientUrl('Team/GetOrgChart'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                companyId: companyId || null,
            }
        })
            .then(response => {
                const processedData = processTreeDataForLeafOnly(response.data);
                setTeams(processedData);
            })
            .catch(axiosError);
    };

    const processTreeDataForLeafOnly = (data) => {
        return data.map(node => {
            const hasChildren = node.items && node.items.length > 0;
            return {
                ...node,
                enable: node.isLeaf,
                items: hasChildren ? processTreeDataForLeafOnly(node.items) : undefined
            };
        });
    };

    const handleTeamChange = (selectedTeams) => {
        const team = selectedTeams.length > 0 ? selectedTeams[0] : null;
        setSelectedTeam(team);
        setInitialValues(prev => ({
            ...prev,
            teamId: team ? team.id : ''
        }));
    };

    const handleRemarkChange = (value) => {
        setInitialValues(prev => ({
            ...prev,
            remark: value
        }));
    };


    const togglePanel = (panelName) => {
        setPanelStates(prev => ({
            ...prev,
            [panelName]: !prev[panelName]
        }));
    };

    const goToStep1 = () => {
        setCurrentStep(1);
    };

    const validateStep1 = (values) => {
        const errors = {};
        
        if (!values.employee || values.employee.trim() === '') {
            errors.employee = 'Employee is required';
        }
        
        if (!values.transferInCompanyId || values.transferInCompanyId === '') {
            errors.transferInCompanyId = 'Target Company is required';
        }
        
        if (!values.teamId || values.teamId === '') {
            errors.teamId = 'Team is required';
        }
        
        if (values.transferOutCompanyId && values.transferInCompanyId && 
            String(values.transferOutCompanyId) === String(values.transferInCompanyId)) {
            errors.transferInCompanyId = 'Target Company must be different from current company';
        }
        
        return errors;
    };

    const handleNext = (values, { setErrors, setSubmitting }) => {
        setSubmitting(true);
        
        const errors = validateStep1(values);
        
        if (Object.keys(errors).length > 0) {
            setErrors(errors);
            setSubmitting(false);
            return;
        }
        
        setCurrentStep(2);
        setSubmitting(false);
    };

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        if (currentStep === 1) {
            setCurrentStep(2);
            setSubmitting(false);
        } else {
            const model = {
                empId: isNaN(parseInt(values.empId, 10)) ? 0 : values.empId,
                transferOutCompanyId: values.transferOutCompanyId,
                transferInCompanyId: values.transferInCompanyId,
                teamId: values.teamId,
                remark: values.remark
            };

            submitTransfer('Transfer/Create', model, setSubmitting);
        }
    };

    const renderForm = ({ errors, isSubmitting, values, setErrors }) => {
        return (
            <div className='card'>
                <Errors errors={errors} />
                <Form className='form-horizontal'>
                    {currentStep === 1 && (
                    <div className='transfer-step-1 transfer-info'>
                        <div className='form-group'>
                            <div className='label-form'>
                                <label className='control-label'>
                                    Employee
                                    <span className='require'>*</span>
                                </label>
                            </div>
                            <div className='select-form'>
                                <Field type='text' name='employee' className='form-control'>
                                    {({ field }) => (
                                        <EmployeeAutocomplete
                                            options={employees}
                                            onChange={getEmployees}
                                            onSelect={handleSelectEmployee}
                                            value={field.value}
                                        />
                                    )}
                                </Field>
                            </div>
                        </div>
                        <div className='form-group'>
                            <div className='label-form'>
                                <label className='control-label'>
                                    Target Company<span className='require'>*</span>
                                </label>
                            </div>
                            <div className='select-form'>
                                <Field
                                    name='transferInCompanyId'
                                    as='select'
                                    className='form-control'
                                    value={initialValues.transferInCompanyId}
                                    onChange={(e) => handleCompanyChange(e.target.value)}>
                                    <option value='' />
                                    {companies
                                        ? companies.map(company => (
                                            <option key={company.id} value={company.id}>
                                                {company.text}
                                            </option>
                                        ))
                                        : ''}
                                </Field>
                            </div>
                        </div>
                        <div className='form-group'>
                            <div className='label-form'>
                                <label className='control-label'>
                                    Team<span className='require'>*</span>
                                </label>
                            </div>
                            <div className='select-form'>
                                <TreeMultiselect
                                    data={teams}
                                    isSelectedAll={false}
                                    hasSearchBox={true}
                                    leafOnly={true}
                                    onChange={handleTeamChange}
                                />
                            </div>
                        </div>
                        <div className='form-group'>
                            <div className='label-form'>
                                <label className='control-label'>
                                    Remark
                                </label>
                            </div>
                            <div className='select-form'>
                                <Field
                                    name='remark'
                                    as='textarea'
                                    className='form-control'
                                    rows='3'
                                    value={initialValues.remark}
                                    onChange={(e) => handleRemarkChange(e.target.value)}
                                />
                            </div>
                        </div>

                        <div className='form-group'>
                            <button
                                type='button'
                                className='btn btn-primary'
                                id='btnNext'
                                onClick={() => handleNext(values, { setErrors, setSubmitting: () => {} })}>
                                Next
                            </button>
                            <a className='btn btn-default' onClick={handleGoBack}>
                                Cancel
                            </a>
                        </div>
                    </div>
                    )}

                    {currentStep === 2 && (
                    <div className='transfer-info'>
                        <div className='form-group'>
                            <button
                                type='button'
                                className='btn btn-default'
                                onClick={goToStep1}>
                                Back
                            </button>
                        </div>
                        <div className='transfer-step-2'>
                            <Personal 
                                isOpen={panelStates.personal} 
                                onToggle={() => togglePanel('personal')} 
                            />
                            <Military 
                                isOpen={panelStates.military} 
                                onToggle={() => togglePanel('military')} 
                            />
                            <Contact 
                                isOpen={panelStates.contact} 
                                onToggle={() => togglePanel('contact')} 
                            />
                            <Information 
                                isOpen={panelStates.information} 
                                onToggle={() => togglePanel('information')} 
                            />
                            <Policy 
                                isOpen={panelStates.policy} 
                                onToggle={() => togglePanel('policy')} 
                            />
                            <Permission 
                                isOpen={panelStates.permission} 
                                onToggle={() => togglePanel('permission')} 
                            />
                        </div>
                        
                        <div className='form-group'>
                            <button
                                type='submit'
                                className='btn btn-primary'
                                id='btnSubmit'
                                disabled={isSubmitting}>
                                Submit
                            </button>
                            <a className='btn btn-default' onClick={handleGoBack}>
                                Cancel
                            </a>
                        </div>
                    </div>
                    )}
                </Form>
            </div>
        );
    };


    return (
        <Formik
            key={formKey}
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={initialValues}
            enableReinitialize={true}
            validationSchema={validationSchema}
            onSubmit={handleSubmit}>
            {renderForm}
        </Formik>
    );
};

export default AppContainer;
