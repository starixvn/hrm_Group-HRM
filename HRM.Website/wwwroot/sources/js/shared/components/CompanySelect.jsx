import React, { useState, useEffect } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { resolveClientUrl } from '../common.functions';

const CompanySelect = ({ defaultCompanyId = '', roleId = '', onChange }) => {
    const [companyOptions, setCompanyOptions] = useState([]);
    const [selectedCompanyId, setSelectedCompanyId] =
        useState(defaultCompanyId);
    const [isLoading, setIsLoading] = useState(true);
    const [hasNotifiedInitialValue, setHasNotifiedInitialValue] =
        useState(false);

    const loadCompanies = async () => {
        const response = await axios({
            url: resolveClientUrl('Team/GetCompaniesByRoleIdForEmployee'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                roleId: roleId,
            },
        });

        const companies = response.data.map((company) => ({
            id: company.companyId,
            name: company.name,
        }));

        setCompanyOptions(companies);
        setIsLoading(false);
    };

    const handleCompanyChange = (e) => {
        const companyId = e.target.value;
        setSelectedCompanyId(companyId);

        if (onChange) {
            onChange(companyId);
        }
    };

    useEffect(() => {
        loadCompanies();
    }, [roleId]);

    useEffect(() => {
        setSelectedCompanyId(defaultCompanyId);
    }, [defaultCompanyId]);

    useEffect(() => {
        if (
            !isLoading &&
            selectedCompanyId &&
            onChange &&
            !hasNotifiedInitialValue
        ) {
            onChange(selectedCompanyId);
            setHasNotifiedInitialValue(true);
        }
    }, [isLoading, selectedCompanyId, onChange, hasNotifiedInitialValue]);

    if (isLoading || companyOptions.length <= 1) {
        return null;
    }

    return (
        <select
            className="form-control"
            value={selectedCompanyId}
            onChange={handleCompanyChange}
            style={{
                display: 'inline-block',
                width: 'auto',
                fontSize: '13px',
                marginLeft: '15px',
                padding: '4px 8px',
                fontWeight: 'bold',
                border: '2px solid #337ab7',
                borderRadius: '4px',
                boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                cursor: 'pointer',
                transition: 'all 0.2s ease',
            }}>
            {companyOptions.map((company) => (
                <option key={company.id} value={company.id}>
                    {company.name}
                </option>
            ))}
        </select>
    );
};

export const renderCompanySelect = (props = {}) => {
    const element = document.getElementById('company-list');
    if (element) {
        ReactDOM.render(<CompanySelect {...props} />, element);
        return true;
    }
    return false;
};

export default CompanySelect;
