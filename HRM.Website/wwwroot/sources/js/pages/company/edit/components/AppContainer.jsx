import React, { useEffect, useState, useCallback } from "react";
import axios from "axios";
import toastr from "toastr";
import { Formik, Form, Field } from "formik";

import Errors from "../../../../shared/components/Errors";
import {
    resolveClientUrl,
    axiosError,
    toastOptions,
} from "../../../../shared/common.functions";

import TagInput from "./TagInput";
import CurrencyMultiSelect from "./CurrencyMultiSelect";

const AppContainer = () => {
    const [formKey, setFormKey] = useState(0);
    const [tagItems, setTagItems] = useState([]);
    const [currencyOptions, setCurrencyOptions] = useState([]);

    const [initialValues, setInitialValues] = useState({
        companyId: "",
        name: "",
        hasBIO: false,
        enableLetstalk: false,
        currency: "",
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
    
            let parsedIPs = [];
            if (data.whitelistIPs) {
                parsedIPs = data.whitelistIPs
                    .split(',')
                    .map(ip => ip.trim())
                    .filter(ip => !!ip)
                    .map((ip, index) => ({
                        id: index.toString(),
                        text: ip,
                    }));
            }
    
            let parsedCurrency = [];
            if (data.currency) {
                parsedCurrency = data.currency.split(',').map(c => c.trim()).filter(c => c);
            }
    
            setInitialValues({
                companyId: data.companyId || "",
                name: data.name || "",
                hasBIO: String(data.hasBIO).toLowerCase() === "true",
                enableLetstalk: String(data.enableLetstalk).toLowerCase() === "true",
                currency: parsedCurrency,
            });
    
            setTagItems(parsedIPs);
            setFormKey((prev) => prev + 1);
        })
        .catch(axiosError);
    }, []);
    

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Company/GetCurrency`),
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" },
        })
        .then((res) => {
            setCurrencyOptions(res.data);
        })
        .catch(axiosError);
    }, []);

    const handleSubmit = useCallback((values, { setSubmitting }) => {
        setSubmitting(true);

        const settingsJson = JSON.stringify({
            hasBIO: values.hasBIO ? "true" : "false",
            enableLetstalk: values.enableLetstalk ? "true" : "false",
            currency: Array.isArray(values.currency) && values.currency.length > 0 ? values.currency.join(',') : '',
            whitelistIPs: tagItems.map((tag) => tag.text).join(','),
        });

        axios.post(
            resolveClientUrl("company/edit"),
            {
                companyId: values.companyId,
                settingsJson: settingsJson
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
    }, [tagItems]);

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
                            className="form-control field-name"
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
                        <TagInput
                            tags={tagItems}
                            onChange={(newTags) => setTagItems(newTags)}
                        />
                    </div>
                </div>

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Currency</label>
                    </div>
                    <div className="input-form">
                        <CurrencyMultiSelect
                            options={currencyOptions || []}
                            value={values.currency || []}
                            onChange={(selectedValues) => setFieldValue("currency", selectedValues)}
                            isDisabled={isSubmitting}
                        />
                    </div>
                </div>

                <div className="form-group">
                    <button
                        type="submit"
                        className="btn btn-primary"
                        id="btnSubmit"
                        disabled={isSubmitting}
                    >
                        Update
                    </button>
                    <button
                        type="button"
                        className="btn btn-default"
                        onClick={handleGoBack}
                    >
                        Back
                    </button>
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
            onSubmit={handleSubmit}
        >
            {renderForm}
        </Formik>
    );
};

export default AppContainer;
