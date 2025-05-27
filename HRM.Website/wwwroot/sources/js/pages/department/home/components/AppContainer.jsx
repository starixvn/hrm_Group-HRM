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
    const [departments, setDepartments] = useState([]);
    const [filterParams, setFilterParams] = useState({ isActive: true });
    const [deleteModalDialog, setDeleteModalDialog] = useState({ show: false });

    const getDepartments = (params) => {
        setFilterParams(params);
        axios
            .post(resolveClientUrl("Department/GetDepartments"), params, {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            })
            .then((response) => {
                setDepartments(response.data);
            })
            .catch(axiosError);
    };

    useEffect(() => {
        getDepartments(filterParams);
    }, []);

    const handleDelete = (department) => {
        setDeleteModalDialog({
            show: true,
            department,
        });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios
            .post(
                resolveClientUrl("Department/Delete"),
                {
                    id: deleteModalDialog.department.id,
                },
                {
                    headers: { "X-Requested-With": "XMLHttpRequest" },
                }
            )
            .then((response) => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success("Deleted successfully!", "", toastOptions());
                    getDepartments(filterParams);
                } else {
                    toastr.error(data.message, "", toastOptions());
                }
            })
            .catch(axiosError);
    };

    const handleCancel = () => {
        setDeleteModalDialog({ show: false });
    };

    function exportExcel() {
        var url = null,
            controllerPath;

        controllerPath = "Department/ExportDepartmentList?";

        var query = "isActive=" + filterParams.isActive;

        url = resolveClientUrl(controllerPath + query);
        location.href = url;
    }

    return (
        <Fragment>
            <Filter onSubmit={getDepartments} exportExcel={exportExcel} />
            <Table data={departments} onDelete={handleDelete} />
            {deleteModalDialog.show && (
                <SweetModalDialog
                    show={deleteModalDialog.show}
                    contents={
                        <p>
                            Are you sure you want to delete{" "}
                            <strong>{deleteModalDialog.department.name}</strong>
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
