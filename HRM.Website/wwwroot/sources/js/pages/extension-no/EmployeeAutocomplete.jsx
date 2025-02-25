import React, { useState } from 'react';
import Autocomplete from 'react-autocomplete';

const EmployeeAutocomplete = ({ options, onChange, onSelect }) => {
    const [keyword, setKeyword] = useState('');

    const handleChange = event => {
        const value = event.target.value;
        setKeyword(value);
        onChange(value);
    };

    const handleSelect = (selectedKeyword, selectedItem) => {
        setKeyword(selectedKeyword);
        onSelect(selectedItem);
    };

    return (
        <Autocomplete
            items={options}
            getItemValue={item => item.value}
            wrapperProps={{ className: 'react-autocomplete-wrapper' }}
            wrapperStyle={{ display: 'block' }}
            renderInput={props => <input type="text" name="Employee" {...props} />}
            renderMenu={items => <div className="items" children={items} />}
            renderItem={(item, isHighlighted) =>
                <div className={`item ${isHighlighted ? 'highlighted' : ''}`} key={item.id}>
                    {item.value}
                </div>
            }
            inputProps={{
                className: 'form-control'
            }}
            value={keyword}
            onChange={handleChange}
            onSelect={handleSelect}
        />
    );
};

export default EmployeeAutocomplete;