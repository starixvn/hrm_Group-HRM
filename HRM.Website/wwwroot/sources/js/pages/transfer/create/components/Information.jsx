import React from "react";
import DatePickerField from "../../../../shared/components/DatePickerField";
import SelectField from "../../../../shared/components/SelectField";

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
        <InformationSelectField
          name="SubCompanyId"
          label="Sub Company"
          options={subCompanies}
          valueKey="subCompanyId"
          textKey="name"
        />

        <DateFieldGroup name="TeamDate" label="Team Date" />
        <InformationSelectField
          name="JobTitleId"
          label="Job Title"
          options={jobTitles}
        />
        <InformationSelectField
          name="JobLevelId"
          label="Job Level"
          options={jobLevels}
        />
        <InformationSelectField
          name="ManagementLevelId"
          label="Management Grade"
          options={managementLevels}
        />

        <DateFieldGroup name="PromotionDate" label="Promotion Date" />
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

const InformationSelectField = ({
  name,
  label,
  options,
  _placeholder = "",
  valueKey = "id",
  textKey = "name",
}) => {
  const selectOptions = options ? options
    .filter(opt => opt[valueKey])
    .map(opt => ({
      value: opt[valueKey],
      label: opt[textKey] || opt.name || opt.text
    })) : [];

  return (
    <div className="form-group">
      <div className="label-form">
        <label className="control-label">{label}</label>
      </div>
      <div className="select-form">
         <SelectField
           name={name}
           options={selectOptions}
           isSearchable={true}
           isClearable={true}
         />
      </div>
    </div>
  );
};

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
