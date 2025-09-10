import React, { useState, useEffect } from 'react';
import { Formik, Form, Field } from 'formik';
import axios from 'axios';

import Errors from '../../../shared/components/Errors';

import { resolveClientUrl, axiosError } from '../../../shared/common.functions';
import { validationSchema, submitUnit } from '../common.functions';
import EmployeeAutocomplete from '../../../shared/components/EmployeeAutocomplete';

const AppContainer = ({ nextId }) => {
  const [employees, setEmployees] = useState(null);
    const [searchPic, setSearchPic] = useState("");
    const [picOptions, setPicOptions] = useState([]);

  useEffect(() => {
    axios({
      url: resolveClientUrl(`Unit/GetEmployeeForPIC`),
      method: 'GET',
      headers: { 'X-Requested-With': 'XMLHttpRequest' }
    })
      .then(response => {
        setEmployees(response.data);
      })
      .catch(axiosError);
  }, []);

  const handleGoBack = () => {
    history.back();
  };

  const handleSubmit = (values, { setSubmitting }) => {
    setSubmitting(true);

    const model = {
      code: values.code.trim(),
      groupUnitId: parseInt(values.groupUnitId, 10),
      category: values.category,
      type: values.type,
      accountableArea: values.accountableArea,
      picEmpId: parseInt(values.picEmpId, 10) || 0,
      isActive: values.isActive
    };

    submitUnit('Unit/Create', model, setSubmitting);
  };

  const getEmployees = (keyword) => {
    if (!employees) return;

    if (keyword.trim().length > 1) {
      const filtered = employees
        .filter((emp) =>
          emp.nickName &&
          emp.nickName
            .toLowerCase()
            .includes(keyword.toLowerCase())
        )
        .map((emp) => ({
          key: emp.id,
          id: emp.id,
          nickname: emp.nickName,
          value: emp.displayNameWithCompany,
          companyName: emp.company || "",
        }));
      setPicOptions(filtered);
    } else {
      setPicOptions([]);
    }
  };

  const renderForm = ({ errors, isSubmitting, setFieldValue }) => {
    const handleSelectPic = (_value = "", item) => {
      setFieldValue("picEmpId", item.id);
    };

    const handleChangePic = (value) => {
        setSearchPic(value);
        if (value.trim() === "") {
            setFieldValue("picEmpId", 0);
        } else {
            var isPic = true;
            getEmployees(value, isPic);
        }
    };
    return (
      <div className="card">
        <Errors errors={errors} />
        <Form className="form-horizontal">
          <div className="form-group">
            <div className="label-form">
              <label className="control-label">
                Business Unit ID<span className="require">*</span>
              </label>
            </div>
            <div className="input-form">
              <Field name="groupUnitId" type="number" className="form-control" autoComplete="off" />
            </div>
          </div>
          <div className="form-group">
            <div className="label-form">
              <label className="control-label">
                Business Unit Code<span className="require">*</span>
              </label>
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
              <EmployeeAutocomplete
                value={searchPic}
                options={picOptions}
                onChange={handleChangePic}
                onSelect={handleSelectPic}
              />
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
            <button type="submit" className="btn btn-primary" id="btnSubmit" disabled={isSubmitting}>
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
      validateOnBlur={false}
      validateOnChange={false}
      initialValues={{
        code: '',
        groupUnitId: nextId,
        category: '',
        type: '',
        accountableArea: '',
        picEmpId: 0,
        isActive: true
      }}
      validationSchema={validationSchema}
      onSubmit={handleSubmit}
    >
      {renderForm}
    </Formik>
  );
};

export default AppContainer;

