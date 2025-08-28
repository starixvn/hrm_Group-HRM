import React, { useState } from 'react';
import toastr from 'toastr';
import axios from 'axios';
import { resolveClientUrl, toastOptions, axiosError } from '../../../../shared/common.functions';

const ExtensionControl = () => {
    const [selectExtension, setSelectExtension] = useState(null);
    const getGroupExtension = isInternal => {
        axios({
            url: resolveClientUrl('ExtensionNo/GetGroupExtension'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: { isInternal }
        }).then(response => {
            if(response.data.length === 0) {
                toastr.error('No group extension found, Please create group first!', '', toastOptions());
            } else {
                window.location = resolveClientUrl('ExtensionNo/Create');
                return;
            }
            setSelectExtension(response.data);
        }).catch(axiosError);
    };

    const handleCreateExtension = () => {
        if(selectExtension == null) {
            getGroupExtension(false);
        } else if(selectExtension != null && selectExtension.length === 0) {
            toastr.error('No group extension found, Please create group first!', '', toastOptions());
        } else {
            window.location = resolveClientUrl('ExtensionNo/Create');
        }
        
    };

    const handleCreateExtensionGroup = () => {
        window.location = resolveClientUrl('ExtensionNo/ExternalGroup');
    };

    return (
        <>
            {
                window.hasPermission &&
                <div className="gap-btn">
                    <a className="btn btn-request" onClick={handleCreateExtension}>
                        <span className="fa fa-plus"></span> New Extension Number
                    </a>
                    <a className="btn btn-primary" onClick={handleCreateExtensionGroup}> Manage External Group</a>
                </div>
            }
        </>
    );
};

export default ExtensionControl;