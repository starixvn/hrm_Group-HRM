import React, { useState } from "react";
import { resolveClientUrl } from "../../../../shared/common.functions";

const Filter = () => {
    const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());
    const currentYear = new Date().getFullYear();
    const years = Array.from(
        { length: currentYear - 2011 + 1 },
        (_, i) => currentYear - i
    );

    const handleYearChange = e => {
        setSelectedYear(parseInt(e.target.value, 10));
    };

    const handleExport = () => {
        window.location.href = resolveClientUrl(
            `HeadcountReport/ExportExcel?year=${selectedYear}`
        );
    };

    return (
        <form className="filter">
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form label--xs">
                        <label className="control-label">Year</label>
                    </div>
                    <div className="select-form">
                        <select
                            id="yearSelect"
                            className="form-control"
                            value={selectedYear}
                            onChange={handleYearChange}>
                            {years.map(year =>(
                                <option key={year} value={year}>
                                    {year}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>
                <div className="form-group" style={{ width: "unset" }}>
                    <button
                        type="button"
                        className="btn btn-excel"
                        onClick={handleExport}>
                        Export
                    </button>
                </div>
            </div>
        </form>
    );
};

export default Filter;

