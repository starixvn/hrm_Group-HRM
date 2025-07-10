import React from 'react';
import { resolveClientUrl } from '../../../../shared/common.functions';

const Filter = () => {
    const handleCreate = () => {
        window.location.href = resolveClientUrl('salarystructure/create');
    };

    return (
        <div className="filter">
            <div className="form-horizontal clearfix">
                <div className="form-group form-group--m" style={{ width: 'unset' }}>
                    <button 
                        type="button"
                        className="btn btn-request" 
                        onClick={handleCreate}
                    >
                        <span className="fa fa-plus" />
                        &nbsp;New Salary Structure
                    </button>
                </div>
            </div>
        </div>
    );
};

export default Filter;