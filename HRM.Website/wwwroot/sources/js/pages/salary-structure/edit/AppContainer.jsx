import React, { useEffect, useState } from "react";
import axios from "axios";
import toastr from "toastr";
import { Formik, Form, Field } from "formik";

import Errors from "../../../shared/components/Errors";

import {
    resolveClientUrl,
    axiosError,
    toastOptions,
} from "../../../shared/common.functions";
import { validationSchema } from "../common.functions";

const AppContainer = () => {
    const [formKey, setFormKey] = useState(0);

  const [jobLevels, setJobLevels] = useState([]);

    const [initialValues, setInitialValues] = useState({
        salaryStructureId: '',
        name: '',
        jobLevelId: '',
        localSalary: '',
        expatSalary: ''
    });

    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios({
            url: resolveClientUrl(`salarystructure/getById/${window.salaryStructureId}`),
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" },
        })
            .then((response) => {
                const data = response.data;
                setInitialValues((prev) => ({
                    ...prev,
                    salaryStructureId: data.salaryStructureId,
                    name: data.name,
                    jobLevelId: data.jobLevelId,
                    localSalary: data.localSalary,
                    expatSalary: data.expatSalary,
                }));
                setFormKey((prev) => prev + 1);
            })
            .catch(axiosError);
    }, []);

    useEffect(() => {
        axios({
            url: resolveClientUrl(`SalaryStructure/GetJobLevels`),
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" },
        })
        .then((res) => setJobLevels(res.data))
        .catch(axiosError);
    }, []);

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios
            .post(
                resolveClientUrl("SalaryStructure/Edit"),
                {
                    salaryStructureId: values.salaryStructureId,
                    name: values.name.trim(),
                    jobLevelId: values.jobLevelId,
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
                    toastr.success("Updated successfully!", "", toastOptions());
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
                                name="name"
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
                                name="jobLevelId"
                                as="select"
                                className="form-control">
                                {jobLevels
                                    ? jobLevels.map((jobLevel) => (
                                          <option key={jobLevel.id} value={jobLevel.id}>
                                              {jobLevel.name}
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
                                step="0.01"
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
                                step="0.01"
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

    const validateFormValues = (values) => {
        try {
          validationSchema.validateSync(values, { abortEarly: false });
          return {};
        } catch (error) {
          const errors = {};
          if (error.inner && Array.isArray(error.inner)) {
            for (const err of error.inner) {
              if (!errors[err.path]) {
                errors[err.path || '_form'] = err.message;
              }
            }
          } else {
            errors._form = error.message;
          }
          return errors;
        }
      };

    return (
        <Formik
            key={formKey}
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={initialValues}
            validationSchema={validationSchema}
            validate={validateFormValues}
            onSubmit={handleSubmit}>
            {renderForm}
        </Formik>
    );
};

export default AppContainer;
