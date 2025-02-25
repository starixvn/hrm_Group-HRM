import React, { PureComponent } from 'react';
import Autocomplete from 'react-autocomplete';

export default class EmployeeAutocomplete extends PureComponent {
    render() {
        return (
            <Autocomplete
                items={this.props.options}
                getItemValue={item => item.value}
                wrapperProps={{ className: 'react-autocomplete-wrapper' }}
                wrapperStyle={{ display: 'block' }}
                renderInput={props => <input type="text" {...props} />}
                renderMenu={items => <div className="items" children={items} />}
                renderItem={(item, isHighlighted) =>
                    <div className={`item ${isHighlighted ? 'highlighted' : ''}`} key={item.id}>
                        {item.value}
                    </div>
                }
                inputProps={{
                    className: 'form-control',
                    placeholder: 'nickname or full name'
                }}
                value={this.props.value}
                onChange={this.props.onChange}
                onSelect={this.props.onSelect}
            />
        );
    }
}