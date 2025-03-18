import React, { Fragment } from 'react';
import toastr from 'toastr';
import { resolveClientUrl, toastOptions } from '../../../../shared/common.functions';

const ExtensionControl = ({ data }) => {
    const handleCreateExtension = () => {
        if(data.length === 0) {
            toastr.error('No group extension found, Please create group first!', '', toastOptions());
        } else {
            window.location = resolveClientUrl('ExtensionNo/Create');
        }
        
    };

    const handleCreateExtensionGroup = () => {
        window.location = resolveClientUrl('ExtensionNo/ExternalGroup');
    };

    return (
        <Fragment>
            {
                window.hasPermission &&
                <div className="gap-btn">
                    <a className="btn btn-request" onClick={handleCreateExtension}>
                        <span className="fa fa-plus"></span> New Extension Number
                    </a>
                    <a className="btn btn-primary" onClick={handleCreateExtensionGroup}> Manage External Group</a>
                </div>
            }
        </Fragment>
    );
};

export default ExtensionControl;