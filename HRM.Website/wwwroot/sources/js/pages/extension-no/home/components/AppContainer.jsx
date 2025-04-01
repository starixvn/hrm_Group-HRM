import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import ExtensionControl from './ExtensionControl';
import Table from './Table';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

import { resolveClientUrl, axiosError, toastOptions } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [extensions, setExtensions] = useState([]);
    const [deleteModalDialog, setDeleteModalDialog] = useState({
        empId: null,
        show: false
    });

    const getExtensionNo = () => {
        axios({
            url: resolveClientUrl('ExtensionNo/GetExtensionNo'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            setExtensions(response.data)
        }).catch(axiosError);
    };

    const handleDeleteExtention = empId => {
        setDeleteModalDialog({ show: true, empId });
    };

    const handleCancel = () => {
        setDeleteModalDialog({ show: false });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios.post(
            resolveClientUrl(`ExtensionNo/Delete/${deleteModalDialog.empId}`),
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getExtensionNo();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    useEffect(() => {
        getExtensionNo();
    }, []);

    return (
        <Fragment>
            <ExtensionControl data={extensions} />
            <Table data={extensions} onDelete={handleDeleteExtention} />
            <SweetModalDialog
                show={deleteModalDialog.show}
                contents={<p>Are you sure you want to delete this Extension?</p>}
                onSubmit={handleConfirmDelete}
                onHide={handleCancel}
            />
        </Fragment>
    );
};

export default AppContainer;