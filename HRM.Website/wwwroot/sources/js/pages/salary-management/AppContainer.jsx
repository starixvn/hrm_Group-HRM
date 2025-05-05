import React from "react";
import axios from "axios";
import toastr from "toastr";

import Action from "./components/Action";
import SalaryTable from "./components/SalaryTable";
import { resolveClientUrl, toastOptions } from "../../shared/common.functions";

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

    return (
        <div className="salary-management-container">
            <Action
                onUploadSuccess={fetchSalaryData}
                onUploadHistorySuccess={fetchUploadHistory}
            />
            <SalaryTable data={salaryData} />
        </div>
    );
};

export default AppContainer;
