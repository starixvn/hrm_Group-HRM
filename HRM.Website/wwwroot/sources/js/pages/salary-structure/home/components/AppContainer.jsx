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
    const [salaryStructures, setSalaryStructures] = useState([]);
    const [deleteModalDialog, setDeleteModalDialog] = useState({ show: false });

    const getSalaryStructures = (params) => {
        axios
            .post(resolveClientUrl("SalaryStructure/GetSalaryStructures"), params, {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            })
            .then((response) => {
                setSalaryStructures(response.data);
            })
            .catch(axiosError);
    };

    useEffect(() => {
        getSalaryStructures();
    }, []);

    const handleDelete = (salaryStructure) => {
        setDeleteModalDialog({
            show: true,
            salaryStructure,
        });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios
            .post(
                resolveClientUrl("SalaryStructure/Delete"),
                {
                    id: deleteModalDialog.salaryStructure.salaryStructureId,
                },
                {
                    headers: { "X-Requested-With": "XMLHttpRequest" },
                }
            )
            .then((response) => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success("Deleted successfully!", "", toastOptions());
                    getSalaryStructures();
                } else {
                    toastr.error(data.message, "", toastOptions());
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
            <Table data={salaryStructures} onDelete={handleDelete} />
            {deleteModalDialog.show && (
                <SweetModalDialog
                    show={deleteModalDialog.show}
                    contents={
                        <p>
                            Are you sure you want to delete{" "}
                            <strong>{deleteModalDialog.salaryStructure.name}</strong>
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
