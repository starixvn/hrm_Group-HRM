import React from "react";
import axios from "axios";
import toastr from "toastr";

import UploadSalaryModal from "./components/UploadSalaryModal";
import ViewUploadHistoryModal from "./components/ViewUploadHistoryModal";
import SalaryTable from "./components/SalaryTable";
import { resolveClientUrl, toastOptions } from "../../shared/common.functions";
import Filter from "./components/Filter";

const AppContainer = () => {
    const [salaryData, setSalaryData] = React.useState([]);
    const [uploadHistory, setUploadHistory] = React.useState([]);

    React.useEffect(() => {
        fetchSalaryData();
        fetchUploadHistory();
    }, []);

    const fetchSalaryData = () => {
        axios
            .get(resolveClientUrl("SalaryManagement/GetSalaryData"))
            .then((response) => {
                setSalaryData(response.data);
            })
            .catch((error) => {
                toastr.error("Failed to fetch salary data", "", toastOptions());
            });
    };

    const fetchUploadHistory = () => {
        axios
            .get(resolveClientUrl("SalaryManagement/GetUploadHistory"))
            .then((response) => {
                setUploadHistory(response.data);
            })
            .catch((error) => {
                toastr.error(
                    "Failed to fetch upload history",
                    "",
                    toastOptions()
                );
            });
    };

    const handleSearch = (filters) => {
        fetchSalaryData(filters);
    };

    return (
        <div className="salary-management-container">
            <div className="filter">
                <Filter onSearch={handleSearch} />
            </div>
            <div className="salary-table">
                <SalaryTable data={salaryData} />
            </div>
        </div>
    );
};

export default AppContainer;
