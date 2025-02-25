import React, { Fragment, useState } from 'react';

import { resolveClientUrl } from '../../../shared/common.functions';

const Filter = ({ onCreate, onSubmit }) => {
    const [filterParams, setFilterParams] = useState({ jobTitle: '' });

    const handleSubmit = () => {
        onSubmit(filterParams);
    };

    const handleChange = event => {
        setFilterParams({
            ...filterParams,
            [event.target.name]: event.target.value
        });
    };

    const handleExportExcel = () => {
        window.location.href = resolveClientUrl('jobTitle/ExportExcel?jobTitle=' + filterParams.jobTitle);
    };

    const handleKeyPress = event => {
        if (event.key === 'Enter') {
            onSubmit(filterParams);
        };
    };

    return (
        <div className="filter">
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Keyword</label>
                    </div>
                    <div className="input-form">
                        <input name="jobTitle"
                            className="form-control"
                            placeholder="job title"
                            autoComplete="off"
                            onChange={handleChange}
                            onKeyPress={handleKeyPress} />
                    </div>
                </div>
                <div className="form-group form-group--m">
                    <button type="submit" className="btn btn-primary" onClick={handleSubmit}>Search</button>
                    {
                        window.hasPermission === 'True' &&
                        <Fragment>
                            <button className="btn btn-excel" onClick={handleExportExcel}>Excel</button>
                            <button className="btn btn-request" type="button" onClick={onCreate}>
                                <span className="fa fa-plus" /> &nbsp;New Job Title
                            </button>
                        </Fragment>
                    }
                </div>
            </div>
        </div>
    );
};

export default Filter;