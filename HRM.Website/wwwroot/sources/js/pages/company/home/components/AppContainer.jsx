import React, { Fragment, useEffect, useState } from "react";
import axios from "axios";

import Table from "./Table";

import {
    axiosError,
    resolveClientUrl
} from "../../../../shared/common.functions";

const AppContainer = () => {
    const [companies, setCompanies] = useState([]);
    const [currencyMapping, setCurrencyMapping] = useState({});

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

    const getCurrencies = () => {
        axios
            .get(resolveClientUrl("Company/GetCurrency"), {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            })
            .then((response) => {
                const currencies = response.data;
                const mapping = {};
                currencies.forEach(currency => {
                    mapping[currency.id] = currency.currencyCode;
                });
                setCurrencyMapping(mapping);
            })
            .catch(axiosError);
    };

    const mapCompaniesCurrency = (companiesData, currencyMap) => {
        if (!companiesData || Object.keys(currencyMap).length === 0) {
            return companiesData;
        }

        return companiesData.map(company => ({
            ...company,
            currency: company.currency ? 
                company.currency.split(',').map(id => id.trim()).filter(id => id)
                    .map(id => currencyMap[id] || id).join(', ') 
                : ""
        }));
    };

    useEffect(() => {
        getCompanies();
        getCurrencies();
    }, []);

    const mappedCompanies = mapCompaniesCurrency(companies, currencyMapping);

    return (
        <Fragment>
            <Table data={mappedCompanies}/>
        </Fragment>
    );
};

export default AppContainer;
