import React, { useState, useEffect } from 'react';
import Autocomplete from 'react-autocomplete';

const PICAutocomplete = ({ data, onChange, onSelect, value }) => {
    const [keyword, setKeyword] = useState(value || '');
    const [options, setOptions] = useState([]);
    const [employees, setEmployees] = useState([]);

    useEffect(() => {
        if (value) {
            setKeyword(value);
        }
    }, [value]);

    useEffect(() => {
        if (data) {
            setEmployees(data);
        }
    }, [data]);

    const handleChange = event => {
        const value = event.target.value;
        setKeyword(value);
        onChange(value);
        getEmployees(value);
    };

    const handleSelect = (selectedKeyword, selectedItem) => {
        setKeyword(selectedKeyword);
        onSelect(selectedItem);
    };
    
    const getEmployees = keyword => {
        if (!employees) {
            return;
        }
        if (keyword.trim().length > 1) {
            const filtered = employees
            .filter(emp =>
                emp.nickName &&
                emp.nickName
                .toLowerCase()
                .includes(keyword.toLowerCase())
            )
            .map(emp => ({
                key: emp.id,
                id: emp.id,
                nickname: emp.nickName,
                value: emp.displayNameWithCompany,
                companyName: emp.company || "",
            }));
            setOptions(filtered);
        } else {
            setOptions([]);
        }
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

export default PICAutocomplete;