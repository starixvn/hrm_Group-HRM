import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Table from './Table';
import Filter from './Filter';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

import {
    axiosError,
    resolveClientUrl,
    toastOptions,
} from '../../../../shared/common.functions';

const AppContainer = () => {
    const [departments, setDepartments] = useState([]);
    const [allUnits, setAllUnits] = useState([]);
    const [filterParams, setFilterParams] = useState({
        isActive: true,
        unitId: '',
    });
    const [deleteModalDialog, setDeleteModalDialog] = useState({ show: false });

    const getDepartments = params => {
        const processedParams = {
            ...params,
            unitId: params.unitId === '' ? null : params.unitId,
        };
        setFilterParams(processedParams);
        axios
            .post(
                resolveClientUrl('Department/GetDepartments'),
                processedParams,
                {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                }
            )
            .then(response => {
                setDepartments(response.data);
            })
            .catch(axiosError);
    };

    const loadAllUnits = () => {
        axios
            .get(resolveClientUrl('Department/GetUnits'), {
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
            })
            .then(response => {
                setAllUnits(response.data);
            })
            .catch(axiosError);
    };

    useEffect( => {
        loadAllUnits();
        getDepartments(filterParams);
    }, []);

    const handleDelete = department => {
        setDeleteModalDialog({
            show: true,
            department,
        });
    };

    const handleConfirmDelete = () => {
        setDeleteModalDialog({ show: false });
        axios
            .post(
                resolveClientUrl('Department/Delete'),
                {
                    id: deleteModalDialog.department.id,
                },
                {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                }
            )
            .then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success('Deleted successfully!', '', toastOptions());
                    getDepartments(filterParams);
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            })
            .catch(axiosError);
    };

    const handleCancel = () => {
        setDeleteModalDialog({ show: false });
    };

    function exportExcel() {
        let query = 'isActive=' + filterParams.isActive;
        if (filterParams.unitId) {
            query += '&unitId=' + filterParams.unitId;
        }

        location.href = resolveClientUrl('Department/ExportDepartmentList?' + query);
    }

    return (
        <Fragment>
            <Filter
                onSubmit={getDepartments}
                exportExcel={exportExcel}
                allUnits={allUnits}
            />
            <Table data={departments} onDelete={handleDelete} />
            {deleteModalDialog.show && (
                <SweetModalDialog
                    show={deleteModalDialog.show}
                    contents={
                        <p>
                            Are you sure you want to delete{' '}
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

