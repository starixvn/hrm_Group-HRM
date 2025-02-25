import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Filter from './Filter';
import Table from './Table';

import { axiosError, blockUI, unblockUI, resolveClientUrl, toastOptions } from '../../../shared/common.functions';

const AppContainer = () => {
    const [roleId, setRoleId] = useState('');
    const [systemRoles, setSystemRoles] = useState([]);
    const [rolePermission, setRolePermission] = useState(null);

    const getSystemRoles = () => {
        axios.post(
            resolveClientUrl('Function/GetSystemRoles'),
            {},
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            setSystemRoles(response.data)
        }).catch(axiosError);
    };

    const getRolePermission = () => {
        blockUI();
        axios.post(
            resolveClientUrl('Function/GetRolePermission'),
            {
                roleId: roleId
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            setRolePermission(response.data);
        }).catch(axiosError).finally(unblockUI);
    };

    useEffect(() => {
        getSystemRoles();
    }, []);

    useEffect(() => {
        getRolePermission();
    }, [roleId]);

    const handleChangePermission = permissions => {
        setRolePermission(permissions);
    };

    const handleSubmit = () => {
        axios.post(
            resolveClientUrl(`Function/AssignPermission`),
            {
                roleId: rolePermission.roleId,
                permissions: rolePermission.permissions
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        ).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Updated successfully!', '', toastOptions());
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    return (
        <Fragment>
            <Filter systemRoles={systemRoles} onChange={setRoleId} />
            {
                rolePermission &&
                <Table rolePermission={rolePermission} onChangePermission={handleChangePermission} />
            }
            {
                rolePermission && rolePermission.isHost && rolePermission.functions.length > 0 &&
                <div className="form-group">
                    <button type="button" className="btn btn-primary" onClick={handleSubmit}>Save</button>
                </div>
            }
        </Fragment>
    );
};

export default AppContainer;