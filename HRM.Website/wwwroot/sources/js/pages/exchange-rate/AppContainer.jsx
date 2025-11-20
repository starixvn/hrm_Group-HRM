import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import moment from 'moment';

import ExchangeRateTable from './components/ExchangeRateTable';
import CurrencyRoundingTable from './components/CurrencyRoundingTable';
import ExchangeRateModal from './components/ExchangeRateModal';
import SweetModalDialog from '../../shared/components/SweetModalDialog';

import { axiosError, toastOptions, resolveClientUrl } from '../../shared/common.functions';

const AppContainer = () => {
    const [exchangeRates, setExchangeRates] = useState([]);
    const [roundingRules, setRoundingRules] = useState([]);
    const [showRoundingInfo, setShowRoundingInfo] = useState(false);
    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [editData, setEditData] = useState(null);
    const [deleteModalDialog, setDeleteModalDialog] = useState({ show: false });

    const formatDate = date => {
        if (!date) {
            return '';
        }
        const dateMoment = moment(date);
        if (!dateMoment.isValid()) {
            return '';
        }
        return dateMoment.format('DD MMM, YYYY');
    };

    useEffect(() => {
        fetchExchangeRates();
    }, []);

    const fetchExchangeRates = () => {
        axios({
            url: resolveClientUrl('ExchangeRate/GetExchangeRates'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'json',
        }).then(response => {
            const data = response.data;
            let rates = [];
            if (Array.isArray(data)) {
                rates = data;
            } else if (data && data.exchangeRates) {
                rates = data.exchangeRates;
            }
            setExchangeRates(rates);
        }).catch(axiosError);
    };

    const fetchRoundingRules = () => {
        axios({
            url: resolveClientUrl('ExchangeRate/GetRoundingRules'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'json',
        }).then(response => {
            const data = response.data || [];
            setRoundingRules(data);
        }).catch(axiosError);
    };

    const handleShowRoundingInfo = () => {
        fetchRoundingRules();
        setShowRoundingInfo(true);
    };

    const handleHideRoundingInfo = () => {
        setShowRoundingInfo(false);
    };

    const handleShowCreateModalDialog = () => setShowAddModal(true);
    const handleHideCreateModalDialog = () => setShowAddModal(false);

    const handleAddSuccess = () => {
        fetchExchangeRates();
    };

    const handleEdit = item => {
        setEditData(item);
        setShowEditModal(true);
    };

    const handleEditClose = () => {
        setShowEditModal(false);
        setEditData(null);
    };

    const handleEditSuccess = () => {
        fetchExchangeRates();
        handleEditClose();
    };

    const handleDelete = item => {
        setDeleteModalDialog({
            show: true,
            exchangeRate: item
        });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        const item = deleteModalDialog.exchangeRate;
        const { currencyId, effectiveDate } = item;

        if (!currencyId || !effectiveDate) {
            toastr.error('Cannot delete: Currency ID or Effective date not found', '', toastOptions());
            return;
        }

        axios.post(
            resolveClientUrl('ExchangeRate/Delete'),
            {
                currencyId: currencyId,
                effectiveDate: effectiveDate
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Exchange Rate deleted successfully', '', toastOptions());
                fetchExchangeRates();
            } else {
                toastr.error(data.message || 'Failed to delete exchange rate', '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleCancel = () => {
        setDeleteModalDialog({ show: false });
    };

    const handleUpdateRounding = item => {
        const { currencyId, rounding } = item;

        if (!currencyId) {
            toastr.error('Cannot update: Currency ID not found', '', toastOptions());
            return;
        }

        const roundingValue = parseInt(rounding, 10);
        if (isNaN(roundingValue)) {
            toastr.error('Rounding value must be a valid number', '', toastOptions());
            return;
        }

        axios.post(
            resolveClientUrl('ExchangeRate/UpdateRounding'),
            {
                currencyId: currencyId,
                rounding: roundingValue
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success(data.message, '', toastOptions());
                fetchRoundingRules();
            } else {
                toastr.error(data.message || 'Failed to update currency rounding', '', toastOptions());
            }
        }).catch(axiosError);
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
                        className="fa fa-cog info-icon"
                        onClick={handleShowRoundingInfo}
                        title="Click to show Currency Rounding info"
                    ></i>
                </div>
                <ExchangeRateTable
                    data={exchangeRates}
                    onEdit={handleEdit}
                    onDelete={handleDelete}
                />
            </div>
            {showRoundingInfo && (
                <>
                    <div className="popup-overlay" onClick={handleHideRoundingInfo}></div>
                    <div className="rounding-info-popup">
                        <div className="rounding-info-header">
                            <h4>Currency Rounding</h4>
                            <i
                                className="fa fa-times close-icon"
                                onClick={handleHideRoundingInfo}
                            ></i>
                        </div>
                        <CurrencyRoundingTable
                            data={roundingRules}
                            onEdit={handleUpdateRounding}
                        />
                    </div>
                </>
            )}
            <ExchangeRateModal
                show={showAddModal}
                onClose={handleHideCreateModalDialog}
                onSuccess={handleAddSuccess}
            />
            <ExchangeRateModal
                show={showEditModal}
                onClose={handleEditClose}
                onSuccess={handleEditSuccess}
                editData={editData}
                isEdit={true}
            />
            <SweetModalDialog
                show={deleteModalDialog.show}
                contents={
                    <p>
                        Are you sure you want to delete <strong>{deleteModalDialog.exchangeRate ? deleteModalDialog.exchangeRate.baseCurrency : ''}</strong>
                        {deleteModalDialog.exchangeRate && deleteModalDialog.exchangeRate.effectiveDate && (
                            <> with effective date <strong>{formatDate(deleteModalDialog.exchangeRate.effectiveDate)}</strong></>
                        )}
                        ?
                    </p>
                }
                onSubmit={handleConfirmDelete}
                onHide={handleCancel}
            />
        </Fragment>
    );
};

export default AppContainer;

