import React, { useEffect, useState } from 'react';
import { Formik, Form, Field } from 'formik';
import axios from 'axios';
import toastr from 'toastr';

import Errors from '../../../shared/components/Errors';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { validationSchema } from '../common.functions';

const AppContainer = () => {
    const [deparments, setDeparments] = useState([]);
    const handleGoBack = () => {
        history.back();
    };

    const getDepartments = () => {
        axios({
            url: resolveClientUrl('Section/GetDepartments'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            setDeparments(response.data);
        }).catch(axiosError);
    };

    useEffect(() => {
        getDepartments();
    }, []);

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios.post(
            resolveClientUrl('Section/Create'),
            {
                Code: values.name.trim(),
                Name: values.code.trim(),
                IsActive: values.isActive,
                DepartmentId: values.department
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Created successfully!', '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl('Section/Index');
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
                            <label className="control-label">Department</label>
                        </div>
                        <div className="input-form">
                            <Field name="department" as="select" className="form-control">
                                <option defaultChecked></option>
                                {
                                    deparments.map((item, i) => (
                                        <option value={item.id} key={item.id}>{item.name}</option>
                                    ))
                                }
                            </Field>
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
                isActive: true,
                department: ''
            }}
            validationSchema={validationSchema}
            onSubmit={handleSubmit}
        >
            {renderForm}
        </Formik>
    );
};

export default AppContainer;