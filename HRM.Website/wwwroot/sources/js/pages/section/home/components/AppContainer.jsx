import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Filter from './Filter';
import Table from './Table';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [section, setSection] = useState([]);
    const [filterParams, setFilterParams] = useState({ isActive: true });
    const [deleteModalDialog, setDeleteModalDialog] = useState({
        section: null,
        show: false
    });

    const getSections = params => {
        setFilterParams(params);
        axios.post(
            resolveClientUrl('Section/GetSections'),
            params,
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            setSection(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        getSections(filterParams);
    }, []);

    const handleDelete = section => {
        setDeleteModalDialog({ show: true, section });
    };

    const handleCancel = () => {
        setDeleteModalDialog({ show: false });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios.post(
            resolveClientUrl(`Section/Delete/${deleteModalDialog.section.id}`),
            {},
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getSections(filterParams);
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    return (
        <Fragment>
            <Filter onSubmit={getSections} />
            <Table data={section} onDelete={handleDelete} />
            {
                deleteModalDialog.show &&
                <SweetModalDialog
                    show={deleteModalDialog.show}
                    contents={<p>Are you sure you want to delete <strong>{deleteModalDialog.section.name}</strong>?</p>}
                    onSubmit={handleConfirmDelete}
                    onHide={handleCancel}
                />
            }
        </Fragment>
    );
};

export default AppContainer;