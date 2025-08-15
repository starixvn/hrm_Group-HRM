import React, { useState, useEffect } from 'react';
import DatePicker from 'react-datepicker';

const MonthPicker = ({
    value,
    onChange,
    className = 'form-control',
    placeholder = 'Select month...',
    minDate,
    maxDate,
    disabled = false,
    dateFormat = 'MMMM yyyy',
    style,
    ...otherProps
}) => {
    const [selectedDate, setSelectedDate] = useState(() => {
        if (value) {
            const [year, month] = value.split('-');
            return new Date(parseInt(year), parseInt(month) - 1, 1);
        }
        return null;
    });

    useEffect(() => {
        if (value) {
            const [year, month] = value.split('-');
            setSelectedDate(new Date(parseInt(year), parseInt(month) - 1, 1));
        } else {
            setSelectedDate(null);
        }
    }, [value]);

    const handleDateChange = date => {
        setSelectedDate(date);

        if (date && onChange) {
            const newMonth = `${date.getFullYear()}-${String(
                date.getMonth() + 1
            ).padStart(2, '0')}`;
            onChange(newMonth);
        } else if (!date && onChange) {
            onChange('');
        }
    };

    return (
        <div
            className="month-picker-container"
            style={{ position: 'relative', display: 'block', width: '100%' }}>
            <DatePicker
                selected={selectedDate}
                onChange={handleDateChange}
                dateFormat={dateFormat}
                showMonthYearPicker
                className={className}
                placeholderText={placeholder}
                minDate={minDate}
                maxDate={maxDate}
                disabled={disabled}
                style={{ ...style, paddingRight: '35px' }}
                autoComplete="off"
                {...otherProps}
            />
            <i
                className="fa fa-calendar"
                style={{
                    position: 'absolute',
                    right: '10px',
                    top: '50%',
                    transform: 'translateY(-50%)',
                    color: '#666',
                    pointerEvents: 'none',
                    fontSize: '14px',
                }}
            />
        </div>
    );
};

export default MonthPicker;
