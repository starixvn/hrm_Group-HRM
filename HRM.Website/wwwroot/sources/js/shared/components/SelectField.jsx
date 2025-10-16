import React from 'react';
import { Field } from 'formik';
import Select from 'react-select';

const SelectField = ({
  name,
  options = [],
  placeholder = '',
  isSearchable = true,
  isClearable = true,
  valueKey = 'value',
  labelKey = 'label',
  onChange
}) => {
  const selectOptions = options.map(option => ({
    value: option[valueKey],
    label: option[labelKey]
  }));

  return (
    <Field name={name}>
      {({ field, form }) => (
        <Select
          options={selectOptions}
          value={selectOptions.find(option =>
            String(option.value) === String(field.value)
          ) || null}
          onChange={selectedOption => {
            const value = selectedOption ? selectedOption.value : null;
            form.setFieldValue(name, value);
            if (onChange) {
              onChange(value);
            }
          }}
          placeholder={placeholder}
          isSearchable={isSearchable}
          isClearable={isClearable}
        />
      )}
    </Field>
  );
};

export default SelectField;
