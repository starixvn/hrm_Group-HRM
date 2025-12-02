import React, { useState, useEffect } from "react";
import { CATEGORY, Category, CategoryLabel } from "../constants/constants";
import GenerateSalaryModal from "./GenerateSalaryModal";
import MonthPicker from "../../../shared/components/MonthPicker";
import TreeMultiselect from "../../../shared/components/react-tree-multiselect/TreeMultiselect";
import axios from "axios";
import { axiosError, resolveClientUrl } from "../../../shared/common.functions";

const Filter = ({ onSearch, latestLog }) => {
  const [month, setMonth] = useState(() => {
    const now = new Date();
    return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(
      2,
      "0"
    )}`;
  });
  const [category, setCategory] = useState(Category.LOCAL);
  const [teams, setTeams] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [units, setUnits] = useState([]);
  const [selectedTeams, setSelectedTeams] = useState([]);
  const [selectedDepartments, setSelectedDepartments] = useState([]);
  const [selectedUnits, setSelectedUnits] = useState([]);

  useEffect(() => {
    handleSearch();
    loadFilterCharts();
  }, []);

  const loadFilterCharts = () => {
    axios({
      url: resolveClientUrl("PayrollManagement/GetOrgChart"),
      method: "GET",
      headers: { "X-Requested-With": "XMLHttpRequest" },
    })
      .then(response => {
        const { data } = response;
        const { team, department, unit } = data;
        setTeams(team);
        setDepartments(department);
        setUnits(unit);
      })
      .catch(axiosError);
  };

  const handleSearch = () => {
    onSearch({
      month: `${month}-01`,
      isLocal: category === Category.LOCAL,
      teamIds: selectedTeams.join(","),
      departmentIds: selectedDepartments.join(","),
      unitIds: selectedUnits.join(","),
    });
  };

  const handleGenerateSuccess = generatedMonth => {
    setMonth(generatedMonth);
    onSearch({
      month: `${generatedMonth}-01`,
      isLocal: category === Category.LOCAL,
      teamIds: selectedTeams.join(","),
      departmentIds: selectedDepartments.join(","),
      unitIds: selectedUnits.join(","),
    });
  };

  const handleTeamChange = selectedItems => {
    const ids = selectedItems.map(item => item.id);
    setSelectedTeams(ids);
  };

  const handleUnitChange = selectedItems => {
    const ids = selectedItems.map(item => item.id);
    setSelectedUnits(ids);
  };

  const handleDepartmentChange = selectedItems => {
    const ids = selectedItems.map(item => item.id);
    setSelectedDepartments(ids);
  };

  return (
    <div className="form-horizontal filter-container">
      <div>
        <div className="form-horizontal clearfix">
          <div className="form-group form-group--m">
            <div className="label-form">
              <label className="control-label">Month</label>
            </div>
            <div className="input-form">
              <MonthPicker
                value={month}
                onChange={setMonth}
                minDate="2020/01/01"
                maxDate={new Date()}
              />
            </div>
          </div>
          <div className="form-group form-group--m">
            <div className="label-form">
              <label className="control-label">Category</label>
            </div>
            <div className="input-form">
              <select
                className="form-control"
                value={category}
                onChange={e => setCategory(Number(e.target.value))}
              >
                {CATEGORY.map((type) => (
                  <option key={type} value={type}>
                    {CategoryLabel[type]}
                  </option>
                ))}
              </select>
            </div>
          </div>
          <div className="form-group form-group--m">
            <div className="label-form">
              <label className="control-label">Unit</label>
            </div>
            <div className="select-form">
              <TreeMultiselect
                data={units}
                isSelectedAll={true}
                hasSearchBox={true}
                onChange={handleUnitChange}
              />
            </div>
          </div>
        </div>
        <div className="form-horizontal clearfix">
          <div className="form-group form-group--m">
            <div className="label-form">
              <label className="control-label">Department</label>
            </div>
            <div className="select-form">
              <TreeMultiselect
                data={departments}
                isSelectedAll={true}
                hasSearchBox={true}
                onChange={handleDepartmentChange}
              />
            </div>
          </div>
          <div className="form-group form-group--m">
            <div className="label-form">
              <label className="control-label">Team</label>
            </div>
            <div className="select-form">
              <TreeMultiselect
                data={teams}
                isSelectedAll={true}
                hasSearchBox={true}
                onChange={handleTeamChange}
              />
            </div>
          </div>
          <div className="form-group form-group--m">
            <button className="btn btn-primary" onClick={handleSearch}>
              Search
            </button>
          </div>
        </div>
      </div>
      <div className="form-group action-section">
        {window.hasPermissionToUpload === "True" && (
          <GenerateSalaryModal onSuccess={handleGenerateSuccess} />
        )}
        {latestLog &&
          (() => {
            const formattedDate = new Date(latestLog.value).toLocaleString(
              "en-US",
              {
                month: "short",
                day: "numeric",
                hour: "numeric",
                minute: "2-digit",
                hour12: true,
              }
            );

            return (
              <p style={{ marginBottom: 0 }}>
                Last generated: {formattedDate} by {latestLog.key}
              </p>
            );
          })()}
      </div>
    </div>
  );
};

export default Filter;
