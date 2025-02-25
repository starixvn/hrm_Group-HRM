import React, { useState } from 'react';

import { resolveClientUrl } from '../../../../shared/common.functions';

const Filter = ({ onSubmit }) => {
    const [filterParams, setFilterParams] = useState({ isActive: true });

    const handleCreate = () => {
        window.location = resolveClientUrl('Section/Create');
    };

    const handleChange = event => {
        setFilterParams({
            ...filterParams,
            [event.target.name]: event.target.value
        });
    };

    const handleSubmit = event => {
        event.preventDefault();
        onSubmit(filterParams)
    };

    return (
        <form className="filter" onSubmit={handleSubmit}>
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form label--xs">
                        <label className="control-label">Status</label>
                    </div>
                    <div className="select-form">
                        <select className="form-control" name="isActive" defaultValue={filterParams.isActive} onChange={handleChange}>
                            <option value=''>All</option>
                            <option value='true'>Active</option>
                            <option value='false'>Inactive</option>
                        </select>
                    </div>
                </div>
                <div className="form-group form-group--m">
                    <button type="submit" className="btn btn-primary">Search</button>
                    {
                        window.hasPermission === 'True' &&
                        <a className="btn btn-request" onClick={handleCreate}>
                            <span className="fa fa-plus" />
                            &nbsp;New Section
                        </a>
                    }
                </div>
            </div>
        </form>
    );
};

export default Filter;