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
    const [formKey, setFormKey] = useState(0);

    const [initialValues, setInitialValues] = useState({
        salaryStructure: "",
        jobLevel: "",
        localSalary: 0,
        expatSalary: 0,
        jobLevels: []
    });

    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios({
            url: resolveClientUrl(`SalaryStructure/GetJobLevels`),
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" },
        })
            .then((response) => {
                setInitialValues((prev) => ({
                    ...prev,
                    jobLevels: response.data,
                }));
            })
            .catch(axiosError);
    }, []);

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios
            .post(
                resolveClientUrl("SalaryStructure/Create"),
                {
                    salaryStructure: values.salaryStructure.trim(),
                    jobLevel: values.jobLevel,
                    localSalary: values.localSalary,
                    expatSalary: values.expatSalary,
                },
                {
                    headers: { "X-Requested-With": "XMLHttpRequest" },
                }
            )
            .then((response) => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success("Created successfully!", "", toastOptions());
                    setTimeout(function () {
                        document.location =
                            resolveClientUrl("SalaryStructure/Index");
                    }, 3000);
                } else {
                    toastr.error(data.message, "", toastOptions());
                    setSubmitting(false);
                }
            })
            .catch(axiosError);
    };

    const renderForm = ({ errors, isSubmitting }) => {
        return (
            <div className="card">
                <Errors errors={errors} />
                <Form className="form-horizontal">
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Salary Structure
                                <span className="require">*</span>
                            </label>
                        </div>
                        <div className="input-form">
                            <Field
                                name="salaryStructure"
                                className="form-control"
                                autoComplete="off"
                            />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Job Level
                                <span className="require">*</span>
                            </label>
                        </div>
                        <div className="select-form">
                            <Field
                                name="jobLevel"
                                as="select"
                                className="form-control">
                                <option value="" />
                                {initialValues.jobLevels
                                    ? initialValues.jobLevels.map((jobLevel) => (
                                          <option key={jobLevel.id} value={jobLevel.id}>
                                              {jobLevel.code}-{jobLevel.nam}
                                          </option>
                                      ))
                                    : ""}
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Local Salary
                            </label>
                        </div>
                        <div className="input-form">
                            <Field
                                name="localSalary"
                                type="number"
                                min="0"
                                className="form-control"
                                autoComplete="off"
                            />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Expat Salary
                            </label>
                        </div>
                        <div className="input-form">
                            <Field
                                name="expatSalary"
                                type="number"
                                min="0"
                                className="form-control"
                                autoComplete="off"
                            />
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
            validationSchema={validationSchema}
            onSubmit={handleSubmit}>
            {renderForm}
        </Formik>
    );
};

export default AppContainer;
