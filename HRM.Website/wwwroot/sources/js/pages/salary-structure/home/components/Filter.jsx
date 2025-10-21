import React, { useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { resolveClientUrl, axiosError, toastOptions } from '../../../../shared/common.functions';
import BasicSalaryConfigModal from './BasicSalaryConfigModal';

const Filter = () => {
    const [showConfigModal, setShowConfigModal] = useState(false);
    const [selectedConfig, setSelectedConfig] = useState('');

    const handleCreate = () => {
        window.location.href = resolveClientUrl('salarystructure/create');
    };

    const handleConfig = () => {
        axios
            .get(resolveClientUrl("SalaryStructure/GetBasicSalaryConfig"), {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            })
            .then(response => {
                const configValue = response.data ? String(response.data) : null;
                setSelectedConfig(configValue || '');
            })
            .catch(axiosError)
            .finally(() => {
                setShowConfigModal(true);
            });
    };

    const handleConfigSubmit = (config) => {
        const configValue = config ? String(config) : null;

        const configData = {
            BasicSalaryConfig: configValue
        };

        axios
            .post(resolveClientUrl("SalaryStructure/EditBasicSalaryConfig"), configData, {
                headers: {
                    "X-Requested-With": "XMLHttpRequest",
                    "Content-Type": "application/json"
                },
            })
            .then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success("Updated successfully!", '', toastOptions());
                } else {
                    toastr.error(data.message);
                }
            })
            .catch(axiosError);

        setShowConfigModal(false);
        setSelectedConfig('');
    };

    const handleConfigClose = () => {
        setShowConfigModal(false);
        setSelectedConfig('');
    };

    const handleConfigChange = (value) => {
        setSelectedConfig(value);
    };

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
                onSubmit={handleConfigSubmit}
                selectedConfig={selectedConfig}
                onConfigChange={handleConfigChange}
            />
        </div>
    );
};

export default Filter;