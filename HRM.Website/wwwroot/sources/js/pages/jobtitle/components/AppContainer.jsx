import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Table from './Table';
import CreateEdit from './CreateEdit';
import Filter from './Filter';
import SweetModalDialog from '../../../shared/components/SweetModalDialog';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';

const AppContainer = () => {
    const [jobTitles, setJobTitles] = useState([]);
    const [jobCategories, setJobCategories] = useState([]);
    const [filterParams, setFilterParams] = useState({ jobTitle: '' });
    const [jobTitleEdit, setJobTitleEdit] = useState(null);
    const [deleteModalDialog, setDeleteModalDialog] = useState({
        show: false
    });
    const [showModalDialog, setShowModalDialog] = useState({
        create: false,
        edit: false
    });

    const getJobTitles = params => {
        setFilterParams(params);
        axios.post(
            resolveClientUrl('JobTitle/GetJobTitles'),
            params,
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            setJobTitles(response.data)
        }).catch(axiosError);
    };

    const getJobCategory = () => {
        axios.post(
            resolveClientUrl('JobTitle/GetJobCategory'),
            {},
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            setJobCategories(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        getJobTitles(filterParams);
        getJobCategory();
    }, []);

    const handleShowCreateModalDialog = () => setShowModalDialog({ create: true });
    const handleHideCreateModalDialog = () => setShowModalDialog({ create: false });

    const handleCreate = jobTitle => {
        axios.post(
            resolveClientUrl(`JobTitle/Insert`),
            {
                name: jobTitle.name.trim(),
                categoryId: jobTitle.categoryId ? jobTitle.categoryId : jobCategories[0].value
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            setShowModalDialog({ create: false });
            if (data.isSuccessful) {
                toastr.success('Created successfully!', '', toastOptions());
                getJobTitles(filterParams);
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleEditInfo = jobTitle => {
        setShowModalDialog({ edit: true });
        setJobTitleEdit({
            id: jobTitle.id,
            name: jobTitle.name,
            categoryId: jobTitle.categoryId
        });
    };

    const handleEdit = jobTitle => {
        axios.post(
            resolveClientUrl(`JobTitle/Update`),
            {
                id: jobTitleEdit.id,
                name: jobTitle.name.trim(),
                categoryId: jobTitle.categoryId
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            setShowModalDialog({ edit: false });
            if (data.isSuccessful) {
                toastr.success('Edit successfully!', '', toastOptions());
                getJobTitles(filterParams);
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleHideEditModalDialog = () => {
        setShowModalDialog({ edit: false });
    };

    const handleDelete = jobTitle => {
        setDeleteModalDialog({
            show: true,
            jobTitle
        });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios.post(
            resolveClientUrl('JobTitle/Delete'),
            {
                id: deleteModalDialog.jobTitle.id
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getJobTitles(filterParams);
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
            <Filter
                onSubmit={getJobTitles}
                onCreate={handleShowCreateModalDialog} />
            <CreateEdit
                show={showModalDialog.create}
                onHide={handleHideCreateModalDialog}
                onSubmit={handleCreate}
                title='Add'
                jobCategories={jobCategories} />
            <CreateEdit
                show={showModalDialog.edit}
                onHide={handleHideEditModalDialog}
                onSubmit={handleEdit}
                title='Edit'
                jobCategories={jobCategories}
                data={jobTitleEdit} />
            <Table
                data={jobTitles}
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