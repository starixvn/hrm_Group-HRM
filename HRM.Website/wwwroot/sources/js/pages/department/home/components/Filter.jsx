import React, { useState } from "react";

import { resolveClientUrl } from "../../../../shared/common.functions";

const Filter = ({ onSubmit, exportExcel, allUnits }) => {
    const [filterParams, setFilterParams] = useState({
        isActive: true,
        unitId: "",
    });

    const handleCreate = () => {
        window.location = resolveClientUrl("Department/Create");
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        onSubmit(filterParams);
    };

    const handleChange = (event) => {
        setFilterParams({
            ...filterParams,
            [event.target.name]: event.target.value,
        });
    };

    const sortedUnits = allUnits.sort((a, b) => {
        const nameA = (a.name || "").toLowerCase();
        const nameB = (b.name || "").toLowerCase();
        return nameA.localeCompare(nameB);
    });

    return (
        <form className="filter" onSubmit={handleSubmit}>
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form label--xs">
                        <label className="control-label">Status</label>
                    </div>
                    <div className="select-form">
                        <select
                            className="form-control"
                            name="isActive"
                            defaultValue={filterParams.isActive}
                            onChange={handleChange}>
                            <option value="">All</option>
                            <option value="true">Active</option>
                            <option value="false">Inactive</option>
                        </select>
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form label--xs">
                        <label className="control-label">BU</label>
                    </div>
                    <div className="select-form">
                        <select
                            className="form-control"
                            name="unitId"
                            defaultValue={filterParams.unitId}
                            onChange={handleChange}
                            style={{ minWidth: "100px" }}>
                            <option value="">All</option>
                            {sortedUnits.map((unit) => (
                                <option key={unit.id} value={unit.id}>
                                    {unit.code}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>
                <div
                    className="form-group form-group--m"
                    style={{ width: "unset" }}>
                    <button type="submit" className="btn btn-primary">
                        Search
                    </button>
                    {window.hasPermission === "True" && (
                        <a className="btn btn-request" onClick={handleCreate}>
                            <span className="fa fa-plus" />
                            &nbsp;New Department
                        </a>
                    )}
                    <button
                        type="button"
                        className="btn btn-excel"
                        onClick={exportExcel}
                        style={{ marginLeft: 5 }}>
                        Excel
                    </button>
                </div>
            </div>
        </form>
    );
};

export default Filter;
