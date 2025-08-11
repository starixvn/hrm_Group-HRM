import React, { useState, useEffect } from "react";
import { Formik, Form, Field } from "formik";
import axios from "axios";
import toastr from "toastr";

import Errors from "../../../shared/components/Errors";

import {
    resolveClientUrl,
    axiosError,
    toastOptions,
} from "../../../shared/common.functions";
import { validationSchema } from "../common.functions";

const AppContainer = () => {
    const [companies, setCompanies] = useState(null);
    const [formKey, setFormKey] = useState(0);
    const [initialValues, setInitialValues] = useState({
        empId: 0,
        employee: '',
        companyId: ''
    });


    const handleGoBack = () => {
        history.back();
    };
    

    const getCompanies = async () => {
        try {
            const response = await axios({
                url: resolveClientUrl(`GroupEmployeeList/GetCompanyDropdownList`),
                method: "GET",
                headers: { "X-Requested-With": "XMLHttpRequest" },
            });
            setCompanies(response.data);
        } catch (error) {
            axiosError(error);
        }
    };

    const getInitialData = async () => {
        try {
            const response = await axios({
                url: resolveClientUrl(`SecondmentCompany/GetById/${window.empId}`),
                method: "GET", 
                headers: { "X-Requested-With": "XMLHttpRequest" },
            });
            const data = response.data;
            setInitialValues({
                empId: data.empId,
                employee: `${data.nickName} ( ${data.currentCompanyName})`,
                companyId: data.secondCompanyId
            });
            setFormKey(prev => prev + 1);
        } catch (error) {
            axiosError(error);
        }
    };

    useEffect( => {
        getCompanies();
        getInitialData();
    }, []);


    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        const model = {
            oldEmpId: window.empId,
            empId: isNaN(parseInt(values.empId, 10)) ? 0 : values.empId,
            companyId: values.companyId
        };

        axios
            .post(
                resolveClientUrl("SecondmentCompany/Edit"),
                model,
                {
                    headers: { "X-Requested-With": "XMLHttpRequest" },
                }
            )
            .then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success("Updated successfully!", "", toastOptions());
                    setTimeout(function () {
                        document.location =
                            resolveClientUrl("secondmentcompany/index");
                    }, 3000);
                } else {
                    toastr.error(data.message, "", toastOptions());
                    setSubmitting(false);
                }
            })
            .catch(axiosError);
    };

    const renderForm = ({ errors, isSubmitting }) => {
        return 
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
                            <Field type="text" name="employee" className="form-control" readOnly>
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Second Company<span className="require">*</span>
                            </label>
                        </div>
                        <div className="select-form">
                            <Field
                                name="companyId"
                                as="select"
                                className="form-control">
                                <option value="" />
                                {companies
                                    ? companies.map((company =>(
                                          <option key={company.id} value={company.id}>
                                              {company.text}
                                          </option>
                                      ))
                                    : ""}
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <button
                            type="submit"
                            className="btn btn-primary"
                            id="btnSubmit"
                            disabled={isSubmitting}>
                            Update
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
            initialValues={{
                empId: initialValues.empId,
                employee: initialValues.employee,
                companyId: initialValues.companyId
            }}
            enableReinitialize={true}
            validationSchema={validationSchema}
            onSubmit={handleSubmit}>
            {renderForm}
        </Formik>
    );
};

export default AppContainer;

