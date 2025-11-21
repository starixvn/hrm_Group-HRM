import React from 'react';
import Select from 'react-select';

const CurrencyMultiSelect = ({ 
    options = [], 
    value = [], 
    onChange, 
    isDisabled = false
}) => {
    const selectOptions = options.map(option => ({
        value: String(option.id),
        label: option.currencyCode
    }));

    const selectValue = Array.isArray(value)
        ? value.map(val => {
            const match = selectOptions.find(opt => opt.value === String(val));
            return match || { value: String(val), label: String(val) };
        })
        : [];

    const handleChange = selectedOption => {
        onChange(selectedOption ? [selectedOption.value] : []);
    };

    return (
        <div className="react-select-container">
            <Select
                options={selectOptions}
                value={selectValue}
                onChange={handleChange}
                isDisabled={isDisabled}
                closeMenuOnSelect={true}
                hideSelectedOptions={false}
                isClearable={true}
                isSearchable={true}
                noOptionsMessage={() => "No currencies found"}
                classNamePrefix="react-select"
            />
        </div>
    );
};

export default CurrencyMultiSelect;


