import React, { useState, useEffect } from "react";
import { Formik, Form, Field } from "formik";
import axios from "axios";
import Errors from "../../../shared/components/Errors";
import EmployeeAutocomplete from '../../extension-no/EmployeeAutocomplete';

import {
    resolveClientUrl,
    axiosError,
} from "../../../shared/common.functions";
import { validationSchema, submitSecondmentCompany } from "../common.functions";

const AppContainer = () => {
    const [companies, setCompanies] = useState(null);
    const [employees, setEmployees] = useState([]);
    const [formKey] = useState(0);
    const [initialValues, setInitialValues] = useState({
        empId: 0,
        employee: '',
        companyId: ''
    });

    const getEmployees = query => {
        setInitialValues(prev => ({
            ...prev,
            employee: ''
        }));
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
                        nickName: item.nickName
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
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" },
        })
            .then(response => {
                setCompanies(response.data);
            })
            .catch(axiosError);
    }, []);

    const handleSelectEmployee = selectedItem => {
        setInitialValues(prev => ({
            ...prev,
            empId: selectedItem.id,
            employee: selectedItem.value
        }));
    };

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        const model = {
            empId: isNaN(parseInt(values.empId, 10)) ? 0 : values.empId,
            companyId: values.companyId
        };

        submitSecondmentCompany("SecondmentCompany/Create", model, setSubmitting);
    };

    const renderForm = ({ errors, isSubmitting }) => {
        return (
            <div className="card">
                <Errors errors={errors} />
                <Form className="form-horizontal">
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Employee
                                <span className="require">*</span>
                            </label>
                        </div>
                        <div className="select-form">
                            <Field type="text" name="employee" className="form-control">
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
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Company<span className="require">*</span>
                            </label>
                        </div>
                        <div className="select-form">
                            <Field
                                name="companyId"
                                as="select"
                                className="form-control">
                                <option value='' />
                                {companies
                                    ? companies.map(company =>(
                                          <option key={company.id} value={company.id}>
                                              {company.text}
                                          </option>
                                      ))
                                    : ''}
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <button
                            type="submit"
                            className="btn btn-primary"
                            id="btnSubmit"
                            disabled={isSubmitting}>
                            Add
                        </button>
                        <a className="btn btn-default" onClick={handleGoBack}>
                            Back
                        </a>
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
            enableReinitialize={true}
            validationSchema={validationSchema}
            onSubmit={handleSubmit}>
            {renderForm}
        </Formik>
    );
};

export default AppContainer;

