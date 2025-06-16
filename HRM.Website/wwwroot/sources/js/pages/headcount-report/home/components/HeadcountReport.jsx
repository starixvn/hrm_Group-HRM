import React, { Fragment } from "react";
import Filter from "./Filter";
import { resolveClientUrl } from "../../../../shared/common.functions";

const HeadcountReport = () => {
    const exportExcel = (selectedYear) => {
        window.location.href = resolveClientUrl(
            `HeadcountReport/ExportExcel?year=${selectedYear}`
        );
    };

    return (
        <Fragment>
            <Filter exportExcel={exportExcel} />
        </Fragment>
    );
};

export default HeadcountReport;
