import React from 'react';
import SelectField from '../../../../shared/components/SelectField';

const Permission = ({ isOpen, onToggle }) => {
    return (
        <div className={`panel-group ${!isOpen ? 'collapsed' : ''}`}>
            <div className='panel-heading' onClick={onToggle}>
                <h4 className='panel-title'>
                    <i className={`fa fa-chevron-${isOpen ? 'down' : 'right'}`}></i>
                    Contact
                </h4>
            </div>
            <div className={`panel-body ${!isOpen ? 'collapsed' : ''}`}>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Roles
                        </label>
                    </div>
                    <div className='input-form'>
                        <span className='read-only-field'>
                            Member
                        </span>
                    </div>
                </div>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>
                            Category
                        </label>
                    </div>
                    <div className='input-form'>
                        <SelectField
                            name={'IsLocal'}
                            options={[
                                { value: true, label: 'Local' },
                                { value: false, label: 'Expat' }
                            ]}
                            isSearchable={true}
                            isClearable={true}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Permission;
