import React, { useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { Formik, Form, Field } from 'formik';

import Errors from '../../../shared/components/Errors';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { validationSchema } from '../common.functions';

const AppContainer = () => {
    const [unit, setUnit] = useState(null);
    const [emps, setEmps] = useState(null);

    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Unit/getById/${window.unitId}`),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            setUnit(response.data);
        }).catch(axiosError);

        axios({
            url: resolveClientUrl(`Employee/GetEmployees`),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            setEmps(response.data);
        }).catch(axiosError);

    }, []);

    const hanldeSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios.post(
            resolveClientUrl('Unit/Edit'),
            {
                unitId: values.id,
                code: values.code.trim(),
                groupUnitId: parseInt(values.groupUnitId, 10),
                category: values.category,
                type: values.type,
                accountableArea: values.accountableArea,
                picEmpId: parseInt(values.picEmpId, 10) || 0,
                isActive: values.isActive
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Updated successfully!', '', toastOptions());
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
                            <label className="control-label">Business Unit ID<span className="require">*</span></label>
                        </div>
                        <div className="input-form">
                            <Field name="groupUnitId" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Business Unit Code<span className="require">*</span></label>
                        </div>
                        <div className="input-form">
                            <Field name="code" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Business Unit Category</label>
                        </div>
                        <div className="input-form">
                            <Field name="category" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Business Unit Type</label>
                        </div>
                        <div className="input-form">
                            <Field name="type" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Accountable Area</label>
                        </div>
                        <div className="input-form">
                            <Field name="accountableArea" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Personal In Charge</label>
                        </div>
                        <div className="select-form">
                            <Field name="picEmpId" as="select" className="form-control">
                                <option value="" />
                                {
                                    emps ? emps.map(emp => (
                                        <option key={emp.Id} value={emp.Id}>
                                            {emp.DisplayNameWithCompany}
                                        </option>
                                    )) : ''
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
                        <button type="submit" className="btn btn-primary" id="btnSubmit" disabled={isSubmitting}>Update</button>
                        <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        );
    };

    if (unit === null) {
        return null;
    }

    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            enableReinitialize={true}
            initialValues={{
                id: unit.id || '',
                code: unit.code || '',
                name: unit.name || '',
                groupUnitId: unit.groupUnitId || 0,
                category: unit.category || '',
                type: unit.type || '',
                accountableArea: unit.accountableArea || '',
                picEmpId: unit.picEmpId || 0,
                isActive: unit.isActive || false
            }}
            validationSchema={validationSchema}
            onSubmit={hanldeSubmit}
        >
            {renderForm}
        </Formik>
    );
};

export default AppContainer;