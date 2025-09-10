import React, { useEffect, useState } from "react";
import axios from "axios";
import { Formik, Form, Field } from "formik";

import Errors from "../../../shared/components/Errors";

import { resolveClientUrl, axiosError } from "../../../shared/common.functions";
import { validationSchema, submitUnit } from "../common.functions";
import EmployeeAutocomplete from "../../../shared/components/EmployeeAutocomplete";

const AppContainer = () => {
  const [unit, setUnit] = useState(null);
  const [employees, setEmployees] = useState(null);
  const [searchPic, setSearchPic] = useState("");
  const [picOptions, setPicOptions] = useState([]);
  const [searchSupporter, setSearchSupporter] = useState("");
  const [supporterOptions, setSupporterOptions] = useState([]);

  const handleGoBack = () => {
    history.back();
  };

  useEffect(() => {
    axios({
      url: resolveClientUrl(`Unit/getById/${window.unitId}`),
      method: "GET",
      headers: { "X-Requested-With": "XMLHttpRequest" },
    })
      .then((response) => {
        setUnit(response.data);
      })
      .catch(axiosError);

    axios({
      url: resolveClientUrl(`Unit/GetEmployeeForPIC`),
      method: "GET",
      headers: { "X-Requested-With": "XMLHttpRequest" },
    })
      .then((response) => {
        setEmployees(response.data);
      })
      .catch(axiosError);
  }, []);

  useEffect(() => {
    if (unit && employees) {
      var employeePic =
        employees && employees.find((e) => e.id === unit.picEmpId);
      employeePic && setSearchPic(employeePic.displayNameWithCompany);
      var supporter =
        employees && employees.find((e) => e.id === unit.supporterEmpId);
      supporter && setSearchSupporter(supporter.displayNameWithCompany);
    }
  }, [unit, employees]);

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
      isActive: values.isActive,
    };
    submitUnit("Unit/Edit", model, setSubmitting);
  };

  const getEmployees = (keyword, isPic) => {
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
      isPic ? setPicOptions(filtered) : setSupporterOptions(filtered);
    } else {
      isPic ? setPicOptions([]) : setSupporterOptions([]);
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

    const handleSelectSupporter = (_value = "", item) => {
      setFieldValue("supporterEmpId", item.id);
    };

    const handleChangeSupporter = (value) => {
        setSearchSupporter(value);
        if (value.trim() === "") {
            setFieldValue("supporterEmpId", 0);
        } else {
            var isPic = false;
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
              <Field
                name="groupUnitId"
                className="form-control"
                type="number"
                autoComplete="off"
              />
            </div>
          </div>
          <div className="form-group">
            <div className="label-form">
              <label className="control-label">
                Business Unit Code<span className="require">*</span>
              </label>
            </div>
            <div className="input-form">
              <Field className="form-control" name="code" autoComplete="off" />
            </div>
          </div>
          <div className="form-group">
            <div className="label-form">
              <label className="control-label">Business Unit Category</label>
            </div>
            <div className="input-form">
              <Field
                className="form-control"
                name="category"
                autoComplete="off"
              />
            </div>
          </div>
          <div className="form-group">
            <div className="label-form">
              <label className="control-label">Business Unit Type</label>
            </div>
            <div className="input-form">
              <Field className="form-control" name="type" autoComplete="off" />
            </div>
          </div>
          <div className="form-group">
            <div className="label-form">
              <label className="control-label">Accountable Area</label>
            </div>
            <div className="input-form">
              <Field
                className="form-control"
                name="accountableArea"
                autoComplete="off"
              />
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
              <label className="control-label">BU Support</label>
            </div>
            <div className="select-form">
              <EmployeeAutocomplete
                value={searchSupporter}
                options={supporterOptions}
                onChange={handleChangeSupporter}
                onSelect={handleSelectSupporter}
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
            <button
              type="submit"
              className="btn btn-primary"
              id="btnSubmit"
              disabled={isSubmitting}
            >
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

const CustomFormik = ({ unit, validationSchema, handleSubmit, renderForm }) => {
  return (
    <Formik
      validateOnBlur={false}
      validateOnChange={false}
      enableReinitialize={true}
      initialValues={{
        id: unit.id || "",
        code: unit.code || "",
        name: unit.name || "",
        groupUnitId: unit.groupUnitId || 0,
        category: unit.category || "",
        type: unit.type || "",
        accountableArea: unit.accountableArea || "",
        picEmpId: unit.picEmpId || 0,
        supporterEmpId: unit.supporterEmpId,
        isActive: unit.isActive || false,
      }}
      validationSchema={validationSchema}
      onSubmit={handleSubmit}
    >
      {renderForm}
    </Formik>
  );
};

export default AppContainer;
