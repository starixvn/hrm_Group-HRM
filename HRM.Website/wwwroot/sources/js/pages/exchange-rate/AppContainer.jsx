import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import ExchangeRateTable from './components/ExchangeRateTable';
import CurrencyRoundingTable from './components/CurrencyRoundingTable';
import AddExchangeRateModal from './components/AddExchangeRateModal';
import ExchangeRateService from './service';

import { resolveClientUrl, axiosError, toastOptions } from '../../shared/common.functions';

const AppContainer = () => {
    const [exchangeRates, setExchangeRates] = useState([]);
    const [roundingRules, setRoundingRules] = useState([]);
    const [showRoundingInfo, setShowRoundingInfo] = useState(false);
    const [showAddModal, setShowAddModal] = useState(false);

    useEffect(() => {
        fetchExchangeRates();
    }, []);

    const fetchExchangeRates = () => {
        ExchangeRateService.getExchangeRates()
            .then(data => {
                // Handle both camelCase and PascalCase property names
                setExchangeRates(data.exchangeRates || data.ExchangeRates || []);
                setRoundingRules(data.roundingRules || data.RoundingRules || []);
            })
            .catch(axiosError);
    };

    const toggleRoundingInfo = () => {
        setShowRoundingInfo(!showRoundingInfo);
    };

    const handleShowCreateModalDialog = () => setShowAddModal(true);
    const handleHideCreateModalDialog = () => setShowAddModal(false);

    const handleAddSuccess = () => {
        fetchExchangeRates();
    };

    return (
        <Fragment>
            {
                window.hasPermission === 'True' &&
                <div className="filter">
                    <button className="btn btn-request" type="button" onClick={handleShowCreateModalDialog}>
                        <span className="fa fa-plus" /> New Exchange Rate
                    </button>
                </div>
            }
            <div className="exchange-rate-table-wrapper">
                <div className="table-header">
                    <h4>1 unit to USD</h4>
                    <i
                        className="fa fa-info-circle info-icon"
                        onClick={toggleRoundingInfo}
                        title="Click to show Currency Rounding info"
                    ></i>
                </div>
                <ExchangeRateTable data={exchangeRates} />
            </div>
            {showRoundingInfo && (
                <>
                    <div className="popup-overlay" onClick={toggleRoundingInfo}></div>
                    <div className="rounding-info-popup">
                        <div className="rounding-info-header">
                            <h4>Currency Rounding</h4>
                            <i 
                                className="fa fa-times close-icon" 
                                onClick={toggleRoundingInfo}
                            ></i>
                        </div>
                        <CurrencyRoundingTable data={roundingRules} />
                    </div>
                </>
            )}
            <AddExchangeRateModal
                show={showAddModal}
                onClose={handleHideCreateModalDialog}
                onSuccess={handleAddSuccess}
            />
        </Fragment>
    );
};

export default AppContainer;

