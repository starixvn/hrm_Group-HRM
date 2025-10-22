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

    useEffect(() => {
        getPayrollSettings();
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


    return (
        <Fragment>
            {window.hasPermission === 'True' && (
                <Filter/>
            )}
            <Table data={payrollSettings} onDelete={handleDelete} />
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
                />
            )}
        </Fragment>
    );
};

export default AppContainer;

