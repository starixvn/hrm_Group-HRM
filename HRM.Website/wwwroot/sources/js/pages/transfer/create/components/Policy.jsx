import React from "react";
import { Field } from "formik";

const Policy = ({ isOpen, onToggle, employeeData }) => {
  return (
    <div className={`panel-group ${!isOpen ? "collapsed" : ""}`}>
      <div className="panel-heading" onClick={onToggle}>
        <h4 className="panel-title">
          <i className={`fa fa-chevron-${isOpen ? "down" : "right"}`}></i>
          Policy
        </h4>
      </div>
      <div className={`panel-body ${!isOpen ? "collapsed" : ""}`}>
        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Allow Claim OT</label>
          </div>
          <div className="input-form">
            <Field name="CanOT" type="checkbox" />
          </div>
        </div>

        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Special OT</label>
          </div>
          <div className="input-form">
            <Field name="SpecialOT" type="text" className="form-control" />
          </div>
        </div>

        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Working days per Month</label>
          </div>
          <div className="input-form">
            <Field name="WorkingDays" type="text" className="form-control" />
          </div>
        </div>

        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Working hours per week</label>
          </div>
          <div className="input-form">
            <Field
              name="WorkingHoursbyWeek"
              type="text"
              className="form-control"
            />
          </div>
        </div>

        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Working hours per day</label>
          </div>
          <div className="input-form">
            <Field name="WorkingHours" type="text" className="form-control" />
          </div>
        </div>

        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Number of Offday</label>
          </div>
          <div className="input-form">
            <Field name="OffDaysbyWeek" type="text" className="form-control" />
          </div>
        </div>

        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Leave Balance</label>
          </div>
          <div className="input-form">
            <span className="read-only-field">
              {((employeeData && employeeData.AnnualLeaveBalance) || 0) +
                " day(s)"}
            </span>
          </div>
        </div>

        <div className="form-group">
          <div className="label-form">
            <label className="control-label">Compensation Leave Balance</label>
          </div>
          <div className="input-form">
            <div className="read-only-field">
              {((employeeData && employeeData.KeepInViewBalance) || 0) +
                " day(s)"}
            </div>
          </div>
        </div>
        {employeeData && employeeData.EnableSickLeaveBalance && (
          <div className="form-group">
            <div className="label-form">
              <label className="control-label">Sick Leave Balance</label>
            </div>
            <div className="input-form">
              <div className="read-only-field">
                {((employeeData && employeeData.SickLeaveBalance) || 0) +
                  " day(s)"}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Policy;
