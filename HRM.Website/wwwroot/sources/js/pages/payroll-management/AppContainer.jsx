import React from "react";
import SalaryTable from "./components/SalaryTable";
import Filter from "./components/Filter";
import PayrollService from "./service";

const AppContainer = () => {
  const [salaryData, setSalaryData] = React.useState([]);
  const [latestLog, setLatestLog] = React.useState(null);

  const fetchSalaryData = async (filters = {}) => {
    const response = await PayrollService.getSalaryData(filters);
    const { data, latestLog } = response;
    setSalaryData(data);
    setLatestLog(latestLog);
  };

  const handleSearch = async (filters) => {
    await fetchSalaryData(filters);
  };

  return (
    <div className="salary-management-container">
      <div className="filter">
        <Filter
          onSearch={handleSearch}
          latestLog={latestLog}
        />
      </div>
      <div className="salary-table">
        <SalaryTable data={salaryData} />
      </div>
    </div>
  );
};

export default AppContainer;
