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

const AppContainer = () => {
    const [formKey, setFormKey] = useState(0);
    const [initialValues, setInitialValues] = useState({
        companyId: '',
        name: '',
        hasBIO: false,
        whitelistIPs: '',
        enableLetstalk: false,
        currency: ''
    });

    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Company/GetCompanyById/${window.companyId}`),
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" },
        })
            .then((response) => {
                const data = response.data;
                setInitialValues({
                    companyId: data.companyId || '',
                    name: data.name || '',
                    hasBIO: String(data.hasBIO).toLowerCase() === 'true',
                    whitelistIPs: data.whitelistIPs || '',
                    enableLetstalk: String(data.enableLetstalk).toLowerCase() === 'true',
                    currency: data.currency || '',
                });
                setFormKey(prev => prev + 1);
            })
            .catch(axiosError);
    }, []);

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios.post(
            resolveClientUrl("company/edit"),
            {
                companyId: values.companyId,
                name: values.name.trim(),
                hasBIO: values.hasBIO ? "true" : "false",
                whitelistIPs: values.whitelistIPs.trim(),
                enableLetstalk: values.enableLetstalk ? "true" : "false",
                currency: values.currency.trim(),
            },
            {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            }
        )
        .then((response) => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success("Updated successfully!", "", toastOptions());
                setTimeout(() => {
                    document.location = resolveClientUrl("company/index");
                }, 3000);
            } else {
                toastr.error(data.message, "", toastOptions());
                setSubmitting(false);
            }
        })
        .catch(axiosError);
    };

    const renderForm = ({ errors, isSubmitting, values, setFieldValue }) => (
        <div className="card">
            <Errors errors={errors} />
            <Form className="form-horizontal">
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Company</label>
                    </div>
                    <div className="input-form">
                        <Field
                            name="name"
                            className="form-control"
                            autoComplete="off"
                            readOnly
                        />
                    </div>
                </div>

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">BIO</label>
                    </div>
                    <div className="input-form">
                        <Field name="hasBIO">
                            {({ field }) => (
                                <input
                                    type="checkbox"
                                    className="checkbox"
                                    checked={field.value}
                                    onChange={(e) =>
                                        setFieldValue("hasBIO", e.target.checked)
                                    }
                                />
                            )}
                        </Field>
                    </div>
                </div>

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Let's Talk</label>
                    </div>
                    <div className="input-form">
                        <Field name="enableLetstalk">
                            {({ field }) => (
                                <input
                                    type="checkbox"
                                    className="checkbox"
                                    checked={field.value}
                                    onChange={(e) =>
                                        setFieldValue("enableLetstalk", e.target.checked)
                                    }
                                />
                            )}
                        </Field>
                    </div>
                </div>

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Whitelist IPs</label>
                    </div>
                    <div className="input-form">
                        <Field
                            name="whitelistIPs"
                            className="form-control"
                            autoComplete="off"
                        />
                    </div>
                </div>

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Currency</label>
                    </div>
                    <div className="input-form">
                        <Field
                            name="currency"
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

    return (
        <Formik
            key={formKey}
            enableReinitialize
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={initialValues}
            onSubmit={handleSubmit}>
            {renderForm}
        </Formik>
    );
};

export default AppContainer;
