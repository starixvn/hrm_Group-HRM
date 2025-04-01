import React from 'react';
import { Formik, Form, Field } from 'formik';
import axios from 'axios';
import toastr from 'toastr';

import Errors from '../../../shared/components/Errors';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { validationSchema } from '../common.functions';

const AppContainer = () => {
    const handleGoBack = () => {
        history.back();
    };

    const hanldeSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios.post(
            resolveClientUrl('Unit/Create'),
            {
                code: values.code.trim(),
                name: values.name.trim(),
                isActive: values.isActive
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Created successfully!', '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl('Unit/Index');
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        }).catch(axiosError);
    };

    const renderForm = ({ errors, isSubmitting }) => {
        return (
            <div className="card">
                <Errors errors={errors} />
                <Form className="form-horizontal">
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Code</label>
                        </div>
                        <div className="input-form">
                            <Field name="code" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Name</label>
                        </div>
                        <div className="input-form">
                            <Field name="name" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Is Active</label>
                        </div>
                        <div className="input-form">
                            <Field type="checkbox" name="isActive" />
                        </div>
                    </div>
                    <div className="form-group">
                        <button type="submit" className="btn btn-primary" id="btnSubmit" disabled={isSubmitting}>Add</button>
                        <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        )
    };

    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={{
                code: '',
                name: '',
                isActive: true
            }}
            validationSchema={validationSchema}
            onSubmit={hanldeSubmit}
        >
            {renderForm}
        </Formik>
    );
};

export default AppContainer;