import React from "react";
import { Field } from "formik";
import DatePickerField from "../../../../shared/components/DatePickerField";

const Information = ({
  isOpen,
  onToggle,
  employeeData,
  jobLevels,
  managementLevels,
  subCompanies,
  jobTitles,
}) => {
  return (
    <div className={`panel-group panel-body-information ${!isOpen ? "collapsed" : ""}`}>
      <div className="panel-heading" onClick={onToggle}>
        <h4 className="panel-title">
          <i className={`fa fa-chevron-${isOpen ? "down" : "right"}`}></i>
          Information
        </h4>
      </div>

      <div className={`panel-body ${!isOpen ? "collapsed" : ""}`}>
        <SelectField
          name="SubCompanyId"
          label="Sub Company"
          options={subCompanies}
          valueKey="subCompanyId"
          textKey="name"
          employeeValue={employeeData && employeeData.SubCompanyId}
        />

        <DateFieldGroup name="TeamDate" label="Team Date" />
        <SelectField
          name="JobTitleId"
          label="Job Title"
          options={jobTitles}
          employeeValue={employeeData && employeeData.JobTitleId}
        />
        <SelectField
          name="JobLevelId"
          label="Job Level"
          options={jobLevels}
          employeeValue={employeeData && employeeData.JobLevelId}
        />
        <SelectField
          name="ManagementLevelId"
          label="Management Grade"
          options={managementLevels}
          employeeValue={employeeData && employeeData.ManagementLevelId}
        />

        <DateFieldGroup name="PromotionDate" label="Promotion Date" />
        <DateFieldGroup name="JoinedDate" label="Group Date" />
        <DateFieldGroup name="CompanyDate" label="Company Date" />
        <DateFieldGroup name="ConfirmedDate" label="Confirmed Date" />

        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Length of Service</label>
          </div>
          <div className="select-form">
            <div
              style={{ marginBottom: "5px", fontSize: "12px", color: "#666" }}
            >
              By Group Date
            </div>
            <div className="input-form">
              <div className="read-only-field">
                {employeeData && employeeData.LengthOfServiceByGroup}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

const SelectField = ({
  name,
  label,
  options,
  placeholder = "",
  valueKey = "id",
  textKey = "name",
  employeeValue,
}) => (
  <div className="form-group">
    <div className="label-form">
      <label className="control-label">{label}</label>
    </div>
    <div className="select-form">
      <Field
        name={name}
        as="select"
        className="form-control"
        value={employeeValue}
      >
        <option value="">--{placeholder || `Select ${label}`}--</option>
        {options && options
          .filter(opt => opt[valueKey])
          .map(opt => (
            <option key={`${name}-${opt[valueKey]}`} value={opt[valueKey]}>
              {opt[textKey] || opt.name || opt.text}
            </option>
          ))}
      </Field>
    </div>
  </div>
);

const DateFieldGroup = ({ name, label }) => (
  <div className="form-group">
    <div className="label-form">
      <label className="control-label">{label}</label>
    </div>
    <div className="calendar-form">
      <DatePickerField name={name} />
    </div>
  </div>
);

export default Information;
