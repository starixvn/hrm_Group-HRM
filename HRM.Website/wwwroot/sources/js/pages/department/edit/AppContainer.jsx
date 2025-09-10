import React, { useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { Formik, Form, Field } from 'formik';

import Errors from '../../../shared/components/Errors';
import {
    resolveClientUrl,
    axiosError,
    toastOptions,
} from '../../../shared/common.functions';
import { validationSchema, useDepartmentUnits } from '../common.functions';
import EmployeeAutocomplete from '../../../shared/components/EmployeeAutocomplete';

const AppContainer = () => {
    const [prepareData, setPrepareData] = useState(null);
    const [formKey, setFormKey] = useState(0);
    const units = useDepartmentUnits();
    const [searchPic, setSearchPic] = useState("");
    const [picOptions, setPicOptions] = useState([]);
    const [searchSupporter, setSearchSupporter] = useState("");
    const [supporterOptions, setSupporterOptions] = useState([]);

    const [initialValues, setInitialValues] = useState({
        departmentId: 0,
        code: '',
        name: '',
        isActive: true,
        unit: '',
        accountableArea: '',
        picEmpId: '',
        supporterEmpId: '',
        groupDeptId: 0,
    });

    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Department/getById/${window.departmentId}`),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
        })
            .then(response => {
                const data = response.data;
                setInitialValues(prev => ({
                    ...prev,
                    departmentId: data.departmentId,
                    code: data.code,
                    name: data.name,
                    isActive: data.isActive,
                    unit: data.unitId,
                    accountableArea: data.accountableArea,
                    picEmpId: data.picEmpId,
                    supporterEmpId: data.supporterEmpId,
                    groupDeptId: data.groupDeptId,
                }));
                setFormKey(prev => prev + 1);
            })
            .catch(axiosError);
    }, []);

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Department/GetPrepareDataForUnit`),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
        })
            .then(response => {
                setPrepareData(response.data);
            })
            .catch(axiosError);
    }, []);
    
      useEffect(() => {
        if (prepareData && initialValues.departmentId !== 0) {
          var employeePic =
            prepareData.employees && prepareData.employees.find((e) => e.id === initialValues.picEmpId);
          employeePic && setSearchPic(employeePic.displayNameWithCompany);
          var supporter =
            prepareData.employees && prepareData.employees.find((e) => e.id === initialValues.supporterEmpId);
          supporter && setSearchSupporter(supporter.displayNameWithCompany);
        }
      }, [prepareData, initialValues]);

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        const submitData = {
            code: values.code.trim(),
            name: values.name.trim(),
            isActive: values.isActive,
            unitId: values.unit,
            groupDeptId: values.groupDeptId,
            accountableArea: values.accountableArea,
            supporterEmpId: values.supporterEmpId,
            picEmpId: values.picEmpId,
        };

        if (values.departmentId) {
            submitData.departmentId = values.departmentId;
        }

        const endpoint = values.departmentId ? 'Department/Edit' : 'Department/Create';
        const successMessage = values.departmentId ? 'Updated successfully!' : 'Created successfully!';
        
        axios
            .post(
                resolveClientUrl(endpoint),
                submitData,
                {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                }
            )
            .then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success(successMessage, '', toastOptions());
                    setTimeout(function () {
                        document.location =
                            resolveClientUrl('Department/Index');
                    }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
                    setSubmitting(false);
                }
            })
            .catch(axiosError);
    };

  const getEmployees = (keyword, isPic) => {
    var employees = prepareData.employees;
    if (!employees) return;
    if (keyword.trim().length > 1) {
      const filtered = employees
        .filter((emp) =>
            emp.nickName &&
            emp.nickName
            .toLowerCase()
            .includes(keyword.toLowerCase())
        )
        .map((emp) => ({
          key: emp.id,
          id: emp.id,
          nickname: emp.nickName,
          value: emp.displayNameWithCompany,
          companyName: emp.company || "",
        }));
      isPic ? setPicOptions(filtered) : setSupporterOptions(filtered);
    } else {
      isPic ? setPicOptions([]) : setSupporterOptions([]);
    }
  };

    const renderForm = ({ errors, isSubmitting, setFieldValue }) => {
        const handleSelectPic = (_value = "", item) => {
            setFieldValue("picEmpId", item.id);
        };

        const handleChangePic = (value) => {
            setSearchPic(value);
            if (value.trim() === "") {
                setFieldValue("picEmpId", 0);
            } else {
                var isPic = true;
                getEmployees(value, isPic);
            }
        };

        const handleSelectSupporter = (_value = "", item) => {
        setFieldValue("supporterEmpId", item.id);
        };

        const handleChangeSupporter = (value) => {
            setSearchSupporter(value);
            if (value.trim() === "") {
                setFieldValue("supporterEmpId", 0);
            } else {
                var isPic = false;
                getEmployees(value, isPic);
            }
        };
        return (
            <div className='card'>
                <Errors errors={errors} />
                <Form className='form-horizontal'>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Department ID
                                <span className='require'>*</span>
                            </label>
                        </div>
                        <div className='input-form'>
                            <Field name='groupDeptId' className='form-control' autoComplete='off' />
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Business Unit
                                <span className='require'>*</span>
                            </label>
                        </div>
                        <div className='select-form'>
                            <Field name='unit' as='select' className='form-control'>
                                <option value='' />
                                {
                                    units ? units.map(unit => (
                                        <option key={unit.id} value={unit.id}>
                                            {unit.code}-{unit.accountableArea}
                                        </option>
                                    )) : ''
                                }
                            </Field>
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Department Code
                                <span className='require'>*</span>
                            </label>
                        </div>
                        <div className='input-form'>
                            <Field name='code' className='form-control' autoComplete='off' />
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Department Name
                                <span className='require'>*</span>
                            </label>
                        </div>
                        <div className='input-form'>
                            <Field name='name' className='form-control' autoComplete='off' />
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>Accountable Area</label>
                        </div>
                        <div className='input-form'>
                            <Field name='accountableArea' className='form-control' autoComplete='off' />
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>Personal In Charge</label>
                        </div>
                        <div className='select-form'>
                            <EmployeeAutocomplete
                                value={searchPic}
                                options={picOptions}
                                onChange={handleChangePic}
                                onSelect={handleSelectPic}
                            />
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>Department Support</label>
                        </div>
                        <div className='select-form'>
                            <EmployeeAutocomplete
                                value={searchSupporter}
                                options={supporterOptions}
                                onChange={handleChangeSupporter}
                                onSelect={handleSelectSupporter}
                            />
                        </div>
                    </div>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>Is Active</label>
                        </div>
                        <div className='input-form'>
                            <Field type='checkbox' name='isActive' style={{ cursor: 'pointer' }} />
                        </div>
                    </div>
                    <div className='form-group'>
                        <button type='submit' className='btn btn-primary' id='btnSubmit' disabled={isSubmitting}>Update</button>
                        <a className='btn btn-default' onClick={handleGoBack}>Back</a>
                    </div>
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
            validationSchema={validationSchema}
            onSubmit={handleSubmit}>
            {renderForm}
        </Formik>
    );
};

export default AppContainer;