import React, { Fragment, useEffect, useState } from "react";
import axios from "axios";

import Table from "./Table";

import {
    axiosError,
    resolveClientUrl
} from "../../../../shared/common.functions";

const AppContainer = () => {
    const [companies, setCompanies] = useState([]);

    const getCompanies = (params = {}) => {
        axios
            .post(resolveClientUrl("Company/GetSettingCompany"), params, {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            })
            .then((response) => {
                setCompanies(response.data);
            })
            .catch(axiosError);
    };

    useEffect(() => {
        getCompanies();
    }, []);


    return (
        <Fragment>
            <Table data={companies} />
        </Fragment>
    );
};

export default AppContainer;
