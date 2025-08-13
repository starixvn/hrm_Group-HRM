import React, { useState, useEffect } from 'react';
import {
    PERIOD_TYPES,
    PeriodType,
    PeriodTypeLabel,
} from '../constants/periodType';
import UploadSalaryModal from './UploadSalaryModal';
import ViewUploadHistoryModal from './ViewUploadHistoryModal';

const Filter = ({ onSearch }) => {
    const [month, setMonth] = useState(() => {
        const now = new Date();
        return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(
            2,
            '0'
        )}`;
    });
    const [periodType, setPeriodType] = useState(PeriodType.ALL);

    useEffect(() => {
        handleSearch();
    }, []);

    const handleSearch = () => {
        onSearch({
            month,
            periodType,
        });
    };

    const handleMonthClick = e => {
        e.target.showPicker();
    };

    return (
        <div className="form-horizontal  clearfix">
            <div className="form-group">
                <div className="label-form">
                    <label className="control-label">Month</label>
                </div>
                <div className="input-form">
                    <input
                        type="month"
                        className="form-control"
                        value={month}
                        onChange={e => setMonth(e.target.value)}
                        onClick={handleMonthClick}
                    />
                </div>
            </div>
            <div className="form-group">
                <div className="label-form">
                    <label className="control-label">Period Type</label>
                </div>
                <div className="input-form">
                    <select
                        className="form-control"
                        value={periodType}
                        onChange={e => setPeriodType(e.target.value)}>
                        <option key={PeriodType.ALL} value={null}>
                            {PeriodTypeLabel[PeriodType.ALL]}
                        </option>
                        {PERIOD_TYPES.map(type => (
                            <option key={type} value={type}>
                                {PeriodTypeLabel[type]}
                            </option>
                        ))}
                    </select>
                </div>
            </div>
            <div className="form-group">
                <button className="btn btn-primary" onClick={handleSearch}>
                    Search
                </button>
            </div>
            <div className="form-group action-section">
                {window.hasPermissionToUpload === 'True' && (
                    <UploadSalaryModal onSuccess={handleSearch} />
                )}
                <ViewUploadHistoryModal />
            </div>
        </div>
    );
};

export default Filter;
