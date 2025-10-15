import React from "react";
import { Field } from "formik";
import DatePickerField from "../../../../shared/components/DatePickerField";
import TreeMultiselect from "../../../../shared/components/react-tree-multiselect/TreeMultiselect";

const Information = ({
  isOpen,
  onToggle,
  employeeData,
  teams,
  onTeamChange,
  selectedTeam,
  jobLevels,
  managementLevels,
  subCompanies,
  jobTitles,
}) => {
  return (
    <div className={`panel-group ${!isOpen ? "collapsed" : ""}`}>
      <div className="panel-heading" onClick={onToggle}>
        <h4 className="panel-title">
          <i className={`fa fa-chevron-${isOpen ? "down" : "right"}`}></i>
          Information
        </h4>
      </div>

      <div className={`panel-body ${!isOpen ? "collapsed" : ""}`}>
        <SelectField
          name="subCompanyId"
          label="Sub Company"
          options={subCompanies}
          employeeValue={employeeData?.subCompanyId}
        />

        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Team</label>
          </div>
          <div className="select-form">
            <TreeMultiselect
              data={teams || []}
              leafOnly
              singleSelect
              hasSearchBox
              onChange={onTeamChange}
              selectedItems={selectedTeam ? [selectedTeam] : []}
            />
          </div>
        </div>

        <DateFieldGroup name="teamDate" label="Team Date" />
        <SelectField
          name="jobTitle"
          label="Job Title"
          options={jobTitles}
          employeeValue={employeeData?.jobTitle}
        />
        <SelectField
          name="jobLevel"
          label="Job Level"
          options={jobLevels}
          employeeValue={employeeData?.jobLevel}
        />
        <SelectField
          name="managementGrade"
          label="Management Grade"
          options={managementLevels}
          employeeValue={employeeData?.managementGrade}
        />

        <DateFieldGroup name="promotionDate" label="Promotion Date" />
        <DateFieldGroup name="groupDate" label="Group Date" />
        <DateFieldGroup name="companyDate" label="Company Date" />
        <DateFieldGroup name="confirmedDate" label="Confirmed Date" />

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
                {employeeData?.lengthOfServiceByGroup}
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
        {options
          ?.filter((opt) => opt[valueKey])
          .map((opt) => (
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
