import React, { useState } from 'react';

import { resolveClientUrl } from '../../../../shared/common.functions';

const Filter = ({ onSubmit }) => {
    const [filterParams] = useState({ isActive: true });

    const handleCreate = () => {
        window.location = resolveClientUrl('secondmentcompany/create');
    };

    const handleSubmit = event => {
        event.preventDefault();
        onSubmit(filterParams);
    };

    return (
        <form className="filter" onSubmit={handleSubmit}>
            <div className="form-horizontal clearfix">
                <div className="form-group form-group--m">
                    {window.hasPermission === 'True' && (
                        <a className="btn btn-request" onClick={handleCreate}>
                            <span className="fa fa-plus" />
                            &nbsp;New Secondment Company
                        </a>
                    )}
                </div>
            </div>
        </form>
    );
};

export default Filter;
