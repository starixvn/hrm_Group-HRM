import React, { Fragment, useState, useEffect } from 'react';
import toastr from 'toastr';

import ExchangeRateTable from './components/ExchangeRateTable';
import CurrencyRoundingTable from './components/CurrencyRoundingTable';
import AddExchangeRateModal from './components/AddExchangeRateModal';
import SweetModalDialog from '../../shared/components/SweetModalDialog';
import ExchangeRateService from './service';

import { axiosError, toastOptions } from '../../shared/common.functions';

const AppContainer = () => {
    const [exchangeRates, setExchangeRates] = useState([]);
    const [roundingRules, setRoundingRules] = useState([]);
    const [showRoundingInfo, setShowRoundingInfo] = useState(false);
    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [editData, setEditData] = useState(null);
    const [deleteModalDialog, setDeleteModalDialog] = useState({ show: false });

    useEffect(() => {
        fetchExchangeRates();
    }, []);

    const fetchExchangeRates = () => {
        ExchangeRateService.getExchangeRates()
            .then(data => {
                const rates = Array.isArray(data) ? data : (data && data.exchangeRates ? data.exchangeRates : []);
                setExchangeRates(rates);
            })
            .catch(axiosError);
    };

    const fetchRoundingRules = () => {
        ExchangeRateService.getRoundingRules()
            .then(data => {
                setRoundingRules(data || []);
            })
            .catch(axiosError);
    };

    const toggleRoundingInfo = () => {
        if (!showRoundingInfo && roundingRules.length === 0) {
            fetchRoundingRules();
        }
        setShowRoundingInfo(!showRoundingInfo);
    };

    const handleShowCreateModalDialog = () => setShowAddModal(true);
    const handleHideCreateModalDialog = () => setShowAddModal(false);

    const handleAddSuccess = () => {
        fetchExchangeRates();
    };

    const handleEdit = (item) => {
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

    const handleDelete = (item) => {
        setDeleteModalDialog({
            show: true,
            exchangeRate: item
        });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        const item = deleteModalDialog.exchangeRate;
        const currencyId = item.currencyId;
        const effectiveDate = item.effectiveDate;

        if (!currencyId || !effectiveDate) {
            toastr.error('Cannot delete: Currency ID or Effective date not found', '', toastOptions());
            return;
        }

        ExchangeRateService.deleteExchangeRate(currencyId, effectiveDate)
            .then((response) => {
                const data = response.data;
                if (data.code === 0 || data.isSuccessful) {
                    toastr.success('Exchange rate deleted successfully', '', toastOptions());
                    fetchExchangeRates();
                } else {
                    toastr.error(data.message || 'Failed to delete exchange rate', '', toastOptions());
                }
            })
            .catch((error) => {
                const errorMessage = (error.response && error.response.data && (error.response.data.message || error.response.data.Message)) || 'Failed to delete exchange rate';
                toastr.error(errorMessage, '', toastOptions());
            });
    };

    const handleCancel = () => {
        setDeleteModalDialog({ show: false });
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
                <ExchangeRateTable
                    data={exchangeRates}
                    onEdit={handleEdit}
                    onDelete={handleDelete}
                />
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
            <AddExchangeRateModal
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
                        Are you sure you want to delete this <strong>{deleteModalDialog.exchangeRate ? deleteModalDialog.exchangeRate.baseCurrency : ''}</strong>
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

