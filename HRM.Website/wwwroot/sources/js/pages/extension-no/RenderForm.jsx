import React from 'react';
import { Form, Field } from 'formik';

import Errors from '../../shared/components/Errors';
import EmployeeAutocomplete from '../../shared/components/EmployeeAutocomplete';

const RenderForm = ({
    errors, isInternal, onChangeInternal, selectExtension, employees,
    onSearchEmployee, onSelectEmployee, isSubmitting, title, setSelectGroup
}) => {
    const handleGoBack = () => {
        history.back();
    };

    const handleSelectGroup = event => {
        setSelectGroup(event.target.value);
    }

    return (
        <div className="card">
            <Errors errors={errors} />
            <Form className="form-horizontal">
                <input name="EmpId" hidden />
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Is Internal</label>
                    </div>
                    <div className="input-form">
                        <Field name="IsInternal" className="form-control">
                            {({
                                field
                            }) => (
                                <input
                                    checked={isInternal}
                                    type="checkbox"
                                    {...field}
                                    onChange={onChangeInternal}
                                />
                            )}
                        </Field>
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Group</label>
                    </div>
                    <div className="input-form">
                        <Field as="select" name="TeamId" className="form-control" onChange={handleSelectGroup}>
                            {
                                selectExtension && selectExtension.map(item => (
                                    <option defaultValue={item.value} value={item.value} key={item.value}>{item.text}</option>
                                ))
                            }
                        </Field>
                    </div>
                </div>
                {
                    isInternal && <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Employee</label>
                        </div>
                        <div className="select-form">
                            <Field type="text" name="Employee" className="form-control">
                                {() => (
                                    <EmployeeAutocomplete
                                        options={employees}
                                        onChange={onSearchEmployee}
                                        onSelect={onSelectEmployee}
                                    />
                                )}
                            </Field>
                        </div>
                    </div>
                }

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Full Name</label>
                    </div>
                    <div className="input-form">
                        <Field name="FullName">
                            {({
                                field
                            }) => (
                                <input
                                    autoComplete='off'
                                    className="form-control"
                                    type="text"
                                    {...field}
                                    readOnly={isInternal || false}
                                />
                            )}
                        </Field>
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Nickname</label>
                    </div>
                    <div className="input-form">
                        <Field name="NickName" className="form-control">
                            {({
                                field
                            }) => (
                                <input
                                    autoComplete='off'
                                    className="form-control"
                                    type="text"
                                    {...field}
                                    readOnly={isInternal || false}
                                />
                            )}
                        </Field>
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Extension No</label>
                    </div>
                    <div className="input-form">
                        <Field type="text" name="ExtensionNo" className="form-control" autoComplete='off' />
                    </div>
                </div>
                <div className="form-group">
                    <button type="submit" id="btnSubmit" className="btn btn-primary" disabled={isSubmitting} style={{ marginRight: "5px" }}>{title}</button>
                    <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                </div>
            </Form>
        </div>
    );
};

export default RenderForm;