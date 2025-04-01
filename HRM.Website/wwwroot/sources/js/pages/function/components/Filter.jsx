import React from 'react';

const Filter = ({ systemRoles, onChange }) => {
    const handleSelectChange = event => {
        onChange(event.target.value);
        event.target.form.dispatchEvent(new Event('submit', { cancelable: true }));
    };

    return (
        <form className="filter">
            <div className="form-horizontal clearfix">
                <div className="form-group form-group--m">
                    <div className="label-form label--xxs">
                        <label className="control-label">Role</label>
                    </div>
                    <div className="select-form">
                        <select name="roleId" as="select" className="form-control" onChange={handleSelectChange}>
                            {
                                systemRoles.map(role => (
                                    <option key={role.roleId} value={role.roleId}>
                                        {role.name}
                                    </option>
                                ))
                            }
                        </select>
                    </div>
                </div>
            </div>
        </form>
    );
};

export default Filter;