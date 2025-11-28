import React, { useState } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError } from '../../../../shared/common.functions';
import BasicSalaryConfigModal from './BasicSalaryConfigModal';

const API_ENDPOINTS = {
    GET_CONFIG: 'SalaryStructure/GetBasicSalaryConfig',
    UPDATE_CONFIG: 'SalaryStructure/EditBasicSalaryConfig',
    CREATE: 'salarystructure/create'
};

const HTTP_HEADERS = {
    XML_REQUEST: { "X-Requested-With": "XMLHttpRequest" },
    JSON_REQUEST: {
        "X-Requested-With": "XMLHttpRequest",
        "Content-Type": "application/json"
    }
};

const Filter = () => {
    const [showConfigModal, setShowConfigModal] = useState(false);
    const [selectedConfigs, setSelectedConfigs] = useState([]);
    const [configOptions, setConfigOptions] = useState([]);

    const parseConfigValue = configValue => {
        if (Array.isArray(configValue)) {
            return configValue;
        }
        
        if (!configValue) {
            return [];
        }
        
        return String(configValue)
            .split(',')
            .map(item => item.trim())
            .filter(item => item);
    };

    const resetModal = () => {
        setShowConfigModal(false);
        setSelectedConfigs([]);
    };

    const handleCreate = () => {
        window.location.href = resolveClientUrl(API_ENDPOINTS.CREATE);
    };

    const handleConfig = async () => {
        try {
            const response = await axios.get(
                resolveClientUrl(API_ENDPOINTS.GET_CONFIG),
                { headers: HTTP_HEADERS.XML_REQUEST }
            );
            
            const parsedConfigs = parseConfigValue(response.data.config);
            setSelectedConfigs(parsedConfigs);
            setConfigOptions(response.data.configOptions || []);
        } catch (error) {
            axiosError(error);
        } finally {
            setShowConfigModal(true);
        }
    };

    const handleConfigClose = resetModal;

    return (
        <div className="filter">
            <div className="form-horizontal clearfix">
                <div className="form-group form-group--m">
                    <button
                        type="button"
                        className="btn btn-request"
                        onClick={handleCreate}
                    >
                        <span className="fa fa-plus" />
                        &nbsp;New Salary Structure
                    </button>
                </div>

                <div className="filter-right-button">
                    <button
                        type="button"
                        className="btn btn-primary"
                        onClick={handleConfig}
                    >
                        Basic Salary Configuration
                    </button>
                </div>
            </div>

            <BasicSalaryConfigModal
                show={showConfigModal}
                onClose={handleConfigClose}
                selectedConfigs={selectedConfigs}
                configOptions={configOptions}
            />
        </div>
    );
};

export default Filter;