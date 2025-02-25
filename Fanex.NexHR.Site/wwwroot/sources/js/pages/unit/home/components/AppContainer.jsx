import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Filter from './Filter';
import Table from './Table';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [filterParams, setFilterParams] = useState({ isActive: true });
    const [units, setUnits] = useState([]);
    const [deleteModalDialog, setDeleteModalDialog] = useState({
        unit: null,
        show: false
    });

    const getUnits = params => {
        setFilterParams(params);

        axios.post(
            resolveClientUrl('Unit/GetUnits'),
            params,
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            setUnits(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        getUnits(filterParams);
    }, []);

    const handleDelete = unit => {
        setDeleteModalDialog({ show: true, unit: unit });
    };

    const handleCancel = () => {
        setDeleteModalDialog({ show: false });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios.post(
            resolveClientUrl('Unit/Delete'),
            {
                unitId: deleteModalDialog.unit.id
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getUnits(filterParams);
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    return (
        <Fragment>
            <Filter onSubmit={getUnits} />
            <Table data={units} onDelete={handleDelete} />
            {
                deleteModalDialog.show &&
                <SweetModalDialog
                    show={deleteModalDialog.show}
                    contents={<p>Are you sure you want to delete <strong>{deleteModalDialog.unit.name}</strong>?</p>}
                    onSubmit={handleConfirmDelete}
                    onHide={handleCancel}
                />
            }
        </Fragment>
    );
};

export default AppContainer;