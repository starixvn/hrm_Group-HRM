import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Table from './Table';
import CreateEdit from './CreateEdit';
import SweetModalDialog from '../../../shared/components/SweetModalDialog';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';

const AppContainer = () => {
    const [jobLevels, setJobLevels] = useState([]);
    const [jobLevelEdit, setJobLevelEdit] = useState(null);
    const [deleteModalDialog, setDeleteModalDialog] = useState({ show: false });
    const [showModalDialog, setShowModalDialog] = useState({
        create: false,
        edit: false
    });

    const getJobLevels = () => {
        axios.post(
            resolveClientUrl('JobLevel/GetJobLevels'),
            {},
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            setJobLevels(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        getJobLevels();
    }, []);

    const handleShowCreateModalDialog = () => setShowModalDialog({ create: true });
    const handleHideCreateModalDialog = () => setShowModalDialog({ create: false });

    const handleCreate = jobLevel => {
        axios.post(
            resolveClientUrl(`JobLevel/Insert`),
            {
                name: jobLevel.name.trim(),
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            setShowModalDialog({ create: false });
            if (data.isSuccessful) {
                toastr.success('Create successfully!', '', toastOptions());
                getJobLevels();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleEditInfo = jobLevel => {
        setShowModalDialog({ edit: true });
        setJobLevelEdit({ id: jobLevel.id, name: jobLevel.name });
    };

    const handleEdit = jobLevel => {
        axios.post(
            resolveClientUrl(`JobLevel/Update`),
            {
                id: jobLevelEdit.id,
                name: jobLevel.name.trim(),
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            setShowModalDialog({ edit: false });
            if (data.isSuccessful) {
                toastr.success('Edit successfully!', '', toastOptions());
                getJobLevels();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleHideEditModalDialog = () => setShowModalDialog({ edit: false });

    const handleDelete = jobLevel => {
        setDeleteModalDialog({
            show: true,
            jobLevel: jobLevel
        });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios.post(
            resolveClientUrl('JobLevel/Delete'),
            {
                id: deleteModalDialog.jobLevel.id
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getJobLevels();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
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
                        <span className="fa fa-plus" /> New Job Level
                    </button>
                </div>
            }
            <CreateEdit
                show={showModalDialog.create}
                onHide={handleHideCreateModalDialog}
                onSubmit={handleCreate}
                title='Add' />
            <CreateEdit
                show={showModalDialog.edit}
                onHide={handleHideEditModalDialog}
                onSubmit={handleEdit}
                data={jobLevelEdit}
                title='Edit' />
            <Table
                data={jobLevels}
                onEdit={handleEditInfo}
                onDelete={handleDelete} />
            <SweetModalDialog
                show={deleteModalDialog.show}
                contents={<p>Are you sure you want to delete this Job Level?</p>}
                onSubmit={handleConfirmDelete}
                onHide={handleCancel} />
        </Fragment>
    );
};

export default AppContainer;