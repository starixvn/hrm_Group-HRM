import React, { useState, useEffect } from 'react';
import { Formik, Form, Field } from 'formik';
import axios from 'axios';

import Errors from '../../../shared/components/Errors';
import EmployeeAutocomplete from '../../../shared/components/EmployeeAutocomplete';
import TreeMultiselect from '../../../shared/components/react-tree-multiselect/TreeMultiselect';

import Personal from './components/Personal';
import Contact from './components/Contact';
import Information from './components/Information';
import Policy from './components/Policy';

import {
    resolveClientUrl,
    axiosError,
} from '../../../shared/common.functions';
import { validationSchema, submitTransfer } from '../common.functions';
import Permission from './components/Permission';

const AppContainer = () => {

    const EXCLUDED_COMPANIES = [
        { id: 1, name: 'NexCompanyId' },
        { id: 2, name: 'LumiCompanyId' },
        { id: 34, name: 'NexVisionCompanyId' }
    ];
    const EXCLUDED_COMPANY_IDS = EXCLUDED_COMPANIES.map(company => company.id);

    const [companies, setCompanies] = useState(null);
    const [targetCompanyIds, setTargetCompanyIds] = useState([]);
    const [employees, setEmployees] = useState([]);
    const [teams, setTeams] = useState([]);
    const [selectedTeam, setSelectedTeam] = useState(null);
    const [employeeData, setEmployeeData] = useState(null);
    const [jobLevels, setJobLevels] = useState([]);
    const [managementLevels, setManagementLevels] = useState([]);
    const [subCompanies, setSubCompanies] = useState([]);
    const [jobTitles, setJobTitles] = useState([]);
    const [countries, setCountries] = useState([]);
    const [formKey] = useState(0);
    const [currentStep, setCurrentStep] = useState(1);
    const [panelStates, setPanelStates] = useState({
        personal: true,
        contact: true,
        information: true,
        policy: true,
        permission: true
    });
    const [initialValues, setInitialValues] = useState({
        EmpId: 0,
        Employee: '',
        TransferOutCompanyId: 0,
        TransferInCompanyId: 0,
        TeamId: 0,
        TransferRemark: '',
        Code: '',
        FullName: '',
        UserId: 0,
        NickName: '',
        EmployeeCard: '',
        DomainUsername: '',
        Gender: null,
        DateOfBirth: null,
        PlaceOfBirth: '',
        Ethnic: '',
        Native: '',
        Marriage: 0,
        CountryCode: '',
        Religion: '',
        CardNo: '',
        DateOfIdCard: null,
        PlaceOfIdCard: '',
        PassportNo: '',
        PassportExp: null,
        Qualification: '',
        Major: '',
        Phone: '',
        HomePhone: '',
        Email: '',
        Address: '',
        CurrentAddress: '',
        PickupRemark: '',
        LetsTalk: '',
        SubCompanyId: null,
        Team: '',
        TeamDate: new Date(),
        JobTitleId: null,
        JobLevelId: null,
        ManagementLevelId: null,
        PromotionDate: null,
        JoinedDate: null,
        CompanyDate: new Date(),
        ConfirmedDate: null,
        LengthOfServiceByGroup: '',
        CanOT: false,
        SpecialOT: null,
        WorkingDays: 0,
        WorkingHoursbyWeek: null,
        WorkingHours: 0,
        OffDaysbyWeek: 0,
        AnnualLeaveBalance: 0,
        KeepInViewBalance: 0,
        SickLeaveBalance: 0,
        EnableSickLeaveBalance: false,
        IsLocal: true,
    });

    const getEmployees = query => {
        if (query.trim().length >= 2) {
            axios({
                url: resolveClientUrl('EmployeeTransfer/GroupEmployeeListCanTransfer'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: { keyword: query, isDelete: false, isEnable: true, companyIds: targetCompanyIds }
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
                const filteredCompanies = response.data.filter(company =>
                    !EXCLUDED_COMPANY_IDS.includes(company.id)
                );
                const companyIds = filteredCompanies.map(company => company.id).join(',');
                setCompanies(filteredCompanies);
                setTargetCompanyIds(companyIds);
            })
            .catch(axiosError);
    }, []);

    const handleSelectEmployee = (_value, selectedItem) => {
        setInitialValues(prev => ({
            ...prev,
            EmpId: selectedItem.id,
            Employee: selectedItem.value,
            TransferOutCompanyId: selectedItem.companyId
        }));
    };

    const handleCompanyChange = value => {
        setInitialValues(prev => ({
            ...prev,
            TransferInCompanyId: value,
            TeamId: 0
        }));

        if (value) {
            loadTeamsByCompany(value);
        } else {
            setTeams([]);
        }
    };

    const loadTeamsByCompany = companyId => {
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

    const processTreeDataForLeafOnly = data => {
        return data.map(node => {
            const hasChildren = node.items && node.items.length > 0;
            return {
                ...node,
                enable: node.isLeaf,
                items: hasChildren ? processTreeDataForLeafOnly(node.items) : undefined
            };
        });
    };

    const handleTeamChange = selectedTeams => {
        const team = selectedTeams.length > 0 ? selectedTeams[0] : null;
        setSelectedTeam(team);
        setInitialValues(prev => ({
            ...prev,
            TeamId: team ? team.id : 0
        }));
    };

    const handleGenerateEmployeeId = async () => {
        try {
            const companyId = initialValues.TransferInCompanyId;
            if (!companyId || companyId === 0) {
                return;
            }

            const response = await axios.get(resolveClientUrl(`employee/GenerateEmployeeCode?companyId=${companyId}`));
            if (response.data) {
                const newEmployeeId = response.data;
                setInitialValues(prev => ({
                    ...prev,
                    Code: newEmployeeId
                }));
                setEmployeeData(prev => ({
                    ...prev,
                    Code: newEmployeeId
                }));
            }
        } catch (error) {
            axiosError(error);
        }
    };

    const handleTransferRemarkChange = value => {
        setInitialValues(prev => ({
            ...prev,
            TransferRemark: value
        }));
    };

    const togglePanel = panelName => {
        setPanelStates(prev => ({
            ...prev,
            [panelName]: !prev[panelName]
        }));
    };

    const validateStep1 = values => {
        const errors = {};

        if (!values.Employee || values.Employee.trim() === '') {
            errors.Employee = 'Employee is required!';
        }

        if (!values.TransferInCompanyId || values.TransferInCompanyId === 0) {
            errors.TransferInCompanyId = 'Target Company is required!';
        }

        if (!values.TeamId || values.TeamId === 0) {
            errors.TeamId = 'Team is required!';
        }

        if (values.TransferOutCompanyId && values.TransferInCompanyId &&
            values.TransferOutCompanyId === parseInt(values.TransferInCompanyId, 10)) {
            errors.TransferInCompanyId = 'Target Company must be different from current company';
        }

        return errors;
    };

    function mapIdentity(employee) {
        return {
            UserId: employee.userId || 0,
            Code: employee.code || '',
            FullName: employee.fullName || '',
            NickName: employee.nickName || '',
            EmployeeCard: employee.employeeCard || '',
        };
    }

    function mapBasicDemographics(e) {
        return {
            Gender: e.gender !== undefined ? e.gender : null,
            DateOfBirth: e.dateOfBirth || null,
            PlaceOfBirth: e.placeOfBirth || '',
            Ethnic: e.ethnic || '',
            Native: e.native || '',
            Marriage: e.marriage || 0,
        };
    }

    function mapGovernmentInfo(e) {
        return {
            CountryCode: e.countryCode || '',
            Religion: e.religion || '',
            CardNo: e.cardNo || '',
            DateOfIdCard: e.dateOfIdCard || null,
            PlaceOfIdCard: e.placeOfIdCard || '',
            PassportNo: e.passportNo || '',
            PassportExp: e.passportExp || null,
            Qualification: e.qualification || '',
            Major: e.major || '',
        };
    }

    function mapDemographics(employee) {
        return { ...mapBasicDemographics(employee), ...mapGovernmentInfo(employee) };
    }

    function mapPersonal(employee) {
        return { ...mapIdentity(employee), ...mapDemographics(employee) };
    }

    function mapContact(employee) {
        return {
            Phone: employee.phone || '',
            HomePhone: employee.homePhone || '',
            Email: employee.email || '',
            Address: employee.address || '',
            CurrentAddress: employee.currentAddress || '',
            PickupRemark: employee.pickupRemark || '',
            LetsTalk: employee.letsTalk || '',
        };
    }

    function mapJobStructure(employee) {
        return {
            SubCompanyId: null,
            Team: employee.team || '',
            TeamDate: new Date().toISOString().split('T')[0],
            JobTitleId: null,
            JobLevelId: employee.jobLevelId || null,
            ManagementLevelId: null,
        };
    }

    function mapJobTimeline(employee) {
        return {
            PromotionDate: null,
            JoinedDate: employee.joinedDate || null,
            CompanyDate: new Date().toISOString().split('T')[0],
            ConfirmedDate: employee.confirmedDate || null,
            LengthOfServiceByGroup: employee.lengthOfServiceByGroupDate || '',
        };
    }

    function mapJob(employee) {
        return { ...mapJobStructure(employee), ...mapJobTimeline(employee) };
    }

    function mapPolicy(employee) {
        return {
            CanOT: employee.canOT || false,
            SpecialOT: employee.specialOT || null,
            WorkingDays: employee.workingDays || 0,
            WorkingHoursbyWeek: employee.workingHoursbyWeek || null,
            WorkingHours: employee.workingHours || 0,
            OffDaysbyWeek: employee.offDaysbyWeek || 0,
            AnnualLeaveBalance: employee.annualLeaveBalance || 0,
            KeepInViewBalance: employee.keepInViewBalance || 0,
            SickLeaveBalance: employee.sickLeaveBalance || 0,
            EnableSickLeaveBalance: employee.enableSickLeaveBalance || false,
        };
    }

    function mapEmployeeResponse(employee) {
        return {
            ...mapPersonal(employee),
            ...mapContact(employee),
            ...mapJob(employee),
            ...mapPolicy(employee),
            ...mapDemographics(employee),
            IsLocal: initialValues.IsLocal,
        };
    }

    const handleNext = (values, { setErrors, setSubmitting }) => {
        setSubmitting(true);

        const errors = validateStep1(values);

        if (Object.keys(errors).length > 0) {
            setErrors(errors);
            setSubmitting(false);
            return;
        }

        axios({
            url: resolveClientUrl(`EmployeeTransfer/GetInfoEmployeeTransfer`),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                id: values.EmpId,
                transferInCompanyId: values.TransferInCompanyId || 0,
                transferOutCompanyId: values.TransferOutCompanyId || 0
            }
        })
            .then(response => {

                const employee = response.data;
                const mappedData = mapEmployeeResponse(employee);

                setEmployeeData(mappedData);
                setInitialValues(prev => ({
                    ...prev,
                    ...mappedData,
                    TeamId: prev.TeamId
                }));
                setJobLevels(employee.jobLevels || []);
                setManagementLevels(employee.managementLevels || []);
                setSubCompanies(employee.subCompanies || []);
                setJobTitles(employee.jobTitles || []);
                setCountries(employee.coutries || []);

                setCurrentStep(2);
                setSubmitting(false);
            })
            .catch(_error => {
                setErrors({ general: 'Failed to load employee data' });
                setSubmitting(false);
            });
    };

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const model = {
            ...values,
            Id: isNaN(parseInt(values.EmpId, 10)) ? 0 : values.EmpId,
            TargetCompanyId: parseInt(values.TransferInCompanyId, 10) || 0,
            TransferOutCompanyId: parseInt(values.TransferOutCompanyId, 10) || 0,
            TransferInDate: new Date().toISOString().split('T')[0],
            Team: selectedTeam ? {
                id: selectedTeam.id,
            } : null,
            Enabled: true
        };
        submitTransfer('EmployeeTransfer/TransferEmployee', model, setSubmitting, 'EmployeeTransfer/Index');
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
                                    </label>
                                </div>
                                <div className='select-form'>
                                    <Field type='text' name='Employee' className='form-control'>
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
                                        Target Company
                                    </label>
                                </div>
                                <div className='select-form'>
                                    <Field
                                        name='TransferInCompanyId'
                                        as='select'
                                        className='form-control'
                                        value={initialValues.TransferInCompanyId}
                                        onChange={e => handleCompanyChange(e.target.value)}>
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
                                        Team
                                    </label>
                                </div>
                                <div className='select-form'>
                                    <TreeMultiselect
                                        data={teams}
                                        isSelectedAll={false}
                                        hasSearchBox={true}
                                        leafOnly={true}
                                        singleSelect={true}
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
                                        name='TransferRemark'
                                        as='textarea'
                                        className='form-control'
                                        rows='3'
                                        value={initialValues.TransferRemark}
                                        onChange={e => handleTransferRemarkChange(e.target.value)}
                                    />
                                </div>
                            </div>

                            <div className='form-group'>
                                <button
                                    type='button'
                                    className='btn btn-primary'
                                    id='btnNext'
                                    onClick={() => handleNext(values, { setErrors, setSubmitting: () => { } })}>
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
                            <div className='transfer-step-2'>
                                <div>
                                    <Personal
                                        isOpen={panelStates.personal}
                                        onToggle={() => togglePanel('personal')}
                                        employeeData={employeeData}
                                        countries={countries}
                                        onGenerateEmployeeId={handleGenerateEmployeeId}
                                    />
                                </div>
                                <div>
                                    <Information
                                        isOpen={panelStates.information}
                                        onToggle={() => togglePanel('information')}
                                        employeeData={employeeData}
                                        jobLevels={jobLevels}
                                        managementLevels={managementLevels}
                                        subCompanies={subCompanies}
                                        jobTitles={jobTitles}
                                    />

                                    <Policy
                                        isOpen={panelStates.policy}
                                        onToggle={() => togglePanel('policy')}
                                        employeeData={employeeData}
                                    />
                                </div>
                                <div>
                                    <Contact
                                        isOpen={panelStates.contact}
                                        onToggle={() => togglePanel('contact')}
                                        employeeData={employeeData}
                                    />
                                    <Permission
                                        isOpen={panelStates.permission}
                                        onToggle={() => togglePanel('permission')}
                                    />
                                </div>
                            </div>

                            <div className='form-group'>
                                <button
                                    type='submit'
                                    className='btn btn-primary'
                                    id='btnSubmit'
                                    disabled={isSubmitting}>
                                    Submit
                                </button>
                                <a className='btn btnCancel btn-default' onClick={handleGoBack}>
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
