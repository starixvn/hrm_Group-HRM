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
    const [prepareData, setPrepareData] = useState(null);
    const [formKey, setFormKey] = useState(0);

    const [initialValues, setInitialValues] = useState({
        code: "",
        name: "",
        isActive: true,
        units: [],
        groupDeptId: 0,
    });

    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Department/GetUnits`),
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" },
        })
            .then((response) => {
                setInitialValues((prev) => ({
                    ...prev,
                    units: response.data,
                }));
            })
            .catch(axiosError);
    }, []);

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Department/GetPrepareDataForUnit`),
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" },
        })
            .then((response) => {
                setPrepareData(response.data);
                if (response.data && response.data.nextId) {
                    setInitialValues((prev) => ({
                        ...prev,
                        groupDeptId: response.data.nextId.toString(),
                    }));
                    setFormKey((prev) => prev + 1);
                }
            })
            .catch(axiosError);
    }, []);

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios
            .post(
                resolveClientUrl("Department/Create"),
                {
                    code: values.code.trim(),
                    name: values.name.trim(),
                    isActive: values.isActive,
                    unitId: values.unit,
                    groupDeptId: values.groupDeptId,
                    accountableArea: values.accountableArea,
                    picEmpId: values.picEmpId,
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
                            resolveClientUrl("Department/Index");
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
                                Department ID
                                <span className="require">*</span>
                            </label>
                        </div>
                        <div className="input-form">
                            <Field
                                name="groupDeptId"
                                className="form-control"
                                autoComplete="off"
                            />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Business Unit<span className="require">*</span>
                            </label>
                        </div>
                        <div className="select-form">
                            <Field
                                name="unit"
                                as="select"
                                className="form-control">
                                <option value="" />
                                {initialValues.units
                                    ? initialValues.units.map((unit) => (
                                          <option key={unit.id} value={unit.id}>
                                              {unit.code}-{unit.accountableArea}
                                          </option>
                                      ))
                                    : ""}
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Department Code
                                <span className="require">*</span>
                            </label>
                        </div>
                        <div className="input-form">
                            <Field
                                name="code"
                                className="form-control"
                                autoComplete="off"
                            />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Department Name
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
                                Accountable Area
                            </label>
                        </div>
                        <div className="input-form">
                            <Field
                                name="accountableArea"
                                className="form-control"
                                autoComplete="off"
                            />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">
                                Personal In Charge
                            </label>
                        </div>
                        <div className="select-form">
                            <Field
                                name="picEmpId"
                                as="select"
                                className="form-control">
                                <option value="" />
                                {prepareData &&
                                    prepareData.employees &&
                                    prepareData.employees.map((emp) => (
                                        <option key={emp.id} value={emp.id}>
                                            {emp.displayNameWithCompany}
                                        </option>
                                    ))}
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Is Active</label>
                        </div>
                        <div className="input-form">
                            <Field
                                type="checkbox"
                                name="isActive"
                                style={{ cursor: "pointer" }}
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
