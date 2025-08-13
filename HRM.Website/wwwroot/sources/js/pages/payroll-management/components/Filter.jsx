import React, { useState, useEffect } from 'react';
import { CATEGORY, Category, CategoryLabel } from '../constants/constants';
import GenerateSalaryModal from './GenerateSalaryModal';
import MonthPicker from '../../../shared/components/MonthPicker';

const Filter = ({ onSearch }) => {
    const [month, setMonth] = useState(() => {
        const now = new Date();
        return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(
            2,
            '0'
        )}`;
    });
    const [category, setCategory] = useState(Category.LOCAL);

    useEffect(() => {
        handleSearch();
    }, []);

    const handleSearch = () => {
        onSearch({
            month: `${month}-01`,
            isLocal: category === Category.LOCAL,
        });
    };

    const handleGenerateSuccess = generatedMonth => {
        setMonth(generatedMonth);
        onSearch({
            month: `${generatedMonth}-01`,
            isLocal: category === Category.LOCAL,
        });
    };

    return (
        <div className="form-horizontal flex-column">
            <div className="filter-section">
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Month</label>
                    </div>
                    <div className="input-form">
                        <MonthPicker
                            value={month}
                            onChange={setMonth}
                            minDate="2020/01/01"
                            maxDate={new Date()}
                        />
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Category</label>
                    </div>
                    <div className="input-form">
                        <select
                            className="form-control"
                            value={category}
                            onChange={e =>
                                setCategory(Number(e.target.value))
                            }>
                            {CATEGORY.map(type => (
                                <option key={type} value={type}>
                                    {CategoryLabel[type]}
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
            </div>

            <div className="form-group action-section">
                {window.hasPermissionToUpload === 'True' && (
                    <GenerateSalaryModal onSuccess={handleGenerateSuccess} />
                )}
            </div>
        </div>
    );
};

export default Filter;