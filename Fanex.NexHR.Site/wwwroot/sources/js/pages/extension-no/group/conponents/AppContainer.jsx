import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Table from './Table';
import CreateEdit from './CreateEdit';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

import { resolveClientUrl, axiosError, toastOptions } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [externalGroups, setExternalGroups] = useState([]);
    const [externalGroupDetail, setExternalGroupDetail] = useState(null);
    const [modalDialog, setModalDialog] = useState({
        create: false,
        edit: false
    });
    const [deleteModalDialog, setDeleteModalDialog] = useState({
        externalId: null,
        show: false
    });

    const getAllEnternalGroup = () => {
        axios({
            url: resolveClientUrl('ExtensionNo/GetAllExternalGroup'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            setExternalGroups(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        getAllEnternalGroup();
    }, []);

    const handleCreate = external => {
        setModalDialog({ create: false });
        axios.post(
            resolveClientUrl('ExtensionNo/CreateExternalGroup'),
            {
                teamName: external.TeamName.trim()
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                getAllEnternalGroup();
                toastr.success('Created successfully!', '', toastOptions());
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleDelete = id => {
        setDeleteModalDialog({ show: true, externalId: id });
    };

    const handleCancel = () => {
        setDeleteModalDialog({ show: false });
    };

    const handleConfirm = () => {
        setDeleteModalDialog({ show: false });
        axios.post(
            resolveClientUrl(`ExtensionNo/DeleteExternalGroup/${deleteModalDialog.externalId}`),
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getAllEnternalGroup();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleSelectExternaGroup = (id, itemName) => {
        setModalDialog({ edit: true })
        setExternalGroupDetail({
            id: id,
            teamName: itemName
        })
    };

    const handleEdit = external => {
        setModalDialog({ edit: false });
        axios.post(
            resolveClientUrl('ExtensionNo/EditExternalGroup'),
            {
                teamName: external.TeamName.trim(),
                teamId: external.TeamId
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Updated successfully!', '', toastOptions());
                getAllEnternalGroup();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleCreateExternal = () => {
        setModalDialog({ create: true });
    };

    const handleCancelExternal = () => {
        setModalDialog({
            create: false,
            edit: false
        });
    };

    return (
        <Fragment>
            <a className="btn btn-request" onClick={handleCreateExternal}>
                <span className="fa fa-plus"></span> New External Group
            </a>
            <CreateEdit
                show={modalDialog.create}
                onHide={handleCancelExternal}
                onSubmit={handleCreate}
                title='Add new'
            />
            <CreateEdit
                show={modalDialog.edit}
                onHide={handleCancelExternal}
                onSubmit={handleEdit}
                data={externalGroupDetail}
                title='Edit'
            />
            <Table data={externalGroups} onDelete={handleDelete} onEdit={handleSelectExternaGroup} />
            <SweetModalDialog
                show={deleteModalDialog.show}
                contents={<p>Are you sure you want to delete this External Group?</p>}
                onSubmit={handleConfirm}
                onHide={handleCancel}
            />
        </Fragment>
    )
};

export default AppContainer;