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
    const [secondmentCompanies, setSecondmentCompanies] = useState([]);
    const [filterParams, setFilterParams] = useState({ isActive: true });
    const [deleteModalDialog, setDeleteModalDialog] = useState({ show: false });

    const getSecondmentCompanyList = (params) => {
        setFilterParams(params);
        axios
            .post(resolveClientUrl("SecondmentCompany/GetSecondmentCompanyList"), params, {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            })
            .then((response) => {
                setSecondmentCompanies(response.data);
            })
            .catch(axiosError);
    };

    useEffect(() => {
        getSecondmentCompanyList(filterParams);
    }, []);

    const handleDelete = (secondmentCompany) => {
        setDeleteModalDialog({
            show: true,
            secondmentCompany,
        });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios
            .post(
                resolveClientUrl("SecondmentCompany/Delete"),
                {
                    empId: deleteModalDialog.secondmentCompany.empId,
                },
                {
                    headers: { "X-Requested-With": "XMLHttpRequest" },
                }
            )
            .then((response) => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success("Deleted successfully!", "", toastOptions());
                    getSecondmentCompanyList(filterParams);
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
            <Filter onSubmit={getSecondmentCompanyList} />
            <Table data={secondmentCompanies} onDelete={handleDelete} />
            {deleteModalDialog.show && (
                <SweetModalDialog
                    show={deleteModalDialog.show}
                    contents={
                        <p>
                            Are you sure you want to delete secondment company of{" "}
                            <strong>{deleteModalDialog.secondmentCompany.nickName}</strong>
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
