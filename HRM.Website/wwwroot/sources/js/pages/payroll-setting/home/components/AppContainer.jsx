import React, { Fragment, useEffect, useState } from "react";
import axios from "axios";
import toastr from "toastr";

import Table from "./Table";
import Filter from "./Filter";
import SweetModalDialog from "../../../../shared/components/SweetModalDialog";

import {
    axiosError,
    resolveClientUrl,
    toastOptions,
} from "../../../../shared/common.functions";

const AppContainer = () => {
    const [payrollSettings, setPayrollSettings] = useState([]);
    const [deleteModalDialog, setDeleteModalDialog] = useState({ show: false });
    const [types, setTypes] = useState([]);
    const [currencies, setCurrencies] = useState([]);

    const getPayrollSettings = () => {
        axios
            .get(resolveClientUrl("PayrollSettings/GetPayrollSettings"), {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            })
            .then(response => {
                setPayrollSettings(response.data);
            })
            .catch(axiosError);
    };

    const getPayrollSettingTypes = () => {
        axios
            .get(resolveClientUrl("PayrollSettings/GetPayrollSettingTypes"), {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            })
            .then(response => {
                const types = response.data.map(item => ({ id: item.payrollItemSettingId, name: item.type }));
                setTypes(types);
            })
            .catch(axiosError);
    };

    const getCurrencies = () => {
        axios
            .get(resolveClientUrl("PayrollSettings/GetCurrency"), {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            })
            .then(response => {
                const currencies = response.data.map(item => ({ id: item.id, name: item.currencyCode }));
                setCurrencies(currencies);
            })
            .catch(axiosError);
    };

    useEffect(() => {
        getPayrollSettings();
        getCurrencies();
        getPayrollSettingTypes();
    }, []);

    const handleDelete = item => {
        setDeleteModalDialog({
            show: true,
            item,
        });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios
            .delete(
                resolveClientUrl(`PayrollSettings/Delete/${deleteModalDialog.item.allowanceItemSettingId}`),
                {
                    headers: { "X-Requested-With": "XMLHttpRequest" },
                }
            )
            .then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success("Deleted successfully!", '', toastOptions());
                    getPayrollSettings();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            })
            .catch(axiosError);
    };

    const handleCancel = () => {
        setDeleteModalDialog({ show: false });
    };

    const checkValidation = (isEdit, data) => {
        if (data.itemName === "") {
            toastr.error("Item name is required", '', toastOptions());
            return false;
        }
        if (data.amount === "") {
            toastr.error("Amount is required", '', toastOptions());
            return false;
        }
        if (!isEdit && data.payrollItemSettingId === null) {
            toastr.error("Type is required", '', toastOptions());
            return false;
        }
        return true;
    };

    const saveEditData = async data => {
        const isEdit = data.allowanceItemSettingId !== null;
        const isValid = checkValidation(isEdit, data);
        if (!isValid) {
            return false;
        }
        try {
            const response = await axios({
                url: resolveClientUrl(`PayrollSettings/${isEdit ? 'Edit' : 'Create'}`),
                method: isEdit ? 'PUT' : 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                data
            });
    
            const { data: result } = response;
            if (result.isSuccessful) {
                toastr.success("Saved successfully!", '', toastOptions());
                getPayrollSettings();
                return true;
            } else {
                toastr.error(result.message, '', toastOptions());
                return false;
            }
        } catch (error) {
            toastr.error("Error saving data", '', toastOptions());
            return false;
        }
    };

    return (
        <Fragment>
            {window.hasPermission === 'True' && (
                <Filter saveEditData={saveEditData} types={types} currencies={currencies} />
            )}
            <Table data={payrollSettings} saveEditData={saveEditData} onDelete={handleDelete} types={types} currencies={currencies} />
            {deleteModalDialog.show && (
                <SweetModalDialog
                    show={deleteModalDialog.show}
                    contents={
                        <p>
                            Are you sure you want to delete{" "}
                            <strong>{deleteModalDialog.item.itemName}</strong>
                            ?
                        </p>
                    }
                    onSubmit={handleConfirmDelete}
                    onHide={handleCancel}
                    title="Notify"
                />
            )}
        </Fragment>
    );
};

export default AppContainer;

