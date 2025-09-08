import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Formik, Form, Field } from 'formik';

import Errors from '../../../shared/components/Errors';

import { resolveClientUrl, axiosError } from '../../../shared/common.functions';
import { validationSchema, submitUnit } from '../common.functions';

const AppContainer = () => {
  const [unit, setUnit] = useState(null);
  const [employees, setEmployees] = useState(null);

  const handleGoBack = () => {
    history.back();
  };

  useEffect(() => {
    axios({
      url: resolveClientUrl(`Unit/getById/${window.unitId}`),
      method: 'GET',
      headers: { 'X-Requested-With': 'XMLHttpRequest' }
    })
      .then(response => {
        setUnit(response.data);
      })
      .catch(axiosError);

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

  const handleSubmit = (values, { setSubmitting }) => {
    setSubmitting(true);

    const model = {
      unitId: values.id,
      code: values.code.trim(),
      groupUnitId: parseInt(values.groupUnitId, 10),
      category: values.category,
      type: values.type,
      accountableArea: values.accountableArea,
      picEmpId: parseInt(values.picEmpId, 10) || 0,
      supporterEmpId: parseInt(values.supporterEmpId, 10) || 0,
      isActive: values.isActive
    };

    submitUnit('Unit/Edit', model, setSubmitting);
  };

  const renderForm = ({ errors, isSubmitting }) => {
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
              <Field name="picEmpId" as="select" className="form-control">
                <option value='' />
                {employees &&
                  employees.map(emp =>(
                    <option key={emp.id} value={emp.id}>
                      {emp.displayNameWithCompany}
                    </option>
                  ))}
              </Field>
            </div>
          </div>
          <div className="form-group">
            <div className="label-form">
              <label className="control-label">BU Support</label>
            </div>
            <div className="select-form">
              <Field name="supporterEmpId" as="select" className="form-control">
                <option value='' />
                {employees &&
                  employees.map(emp =>(
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
              <Field type="checkbox" name="isActive" />
            </div>
          </div>
          <div className="form-group">
            <button type="submit" className="btn btn-primary" id="btnSubmit" disabled={isSubmitting}>
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

  if (unit === null) {
    return null;
  }

  return (
    <CustomFormik 
      unit={unit}
      validationSchema={validationSchema}
      handleSubmit={handleSubmit}
      renderForm={renderForm}
    />
  );
};

const CustomFormik = ({unit, validationSchema, handleSubmit, renderForm}) => {
  return <Formik
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
              supporterEmpId: unit.supporterEmpId,
              isActive: unit.isActive || false
            }}
            validationSchema={validationSchema}
            onSubmit={handleSubmit}
          >
          {renderForm}
        </Formik>
}

export default AppContainer;

