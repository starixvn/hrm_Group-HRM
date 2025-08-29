import React, { PureComponent } from 'react';

import EmployeeAutocomplete from '../../../../shared/components/EmployeeAutocomplete';

export default class THead extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            value: '',
            employee: null
        };
    }

    handleChange = event => {
        const value = event.target.value;

        this.setState({ value });
        this.props.onSearchEmployees(value);
    };

    handleSelect = (value, item) => {
        this.setState({ value, employee: item });
    };

    handleAddEmployee = () => {
        const employee = this.state.employee;
        if (employee !== null) {
            this.props.onAddEmployee(employee);
            this.setState({ value: '', employee: null });
        }
    };

    render() {
        const { employeeOptions } = this.props;

        return (
            <thead>
                <tr>
                    <th colSpan={2}>
                        <div className="header-control">
                            <div>Employee</div>
                            <EmployeeAutocomplete value={this.state.value}
                                options={employeeOptions}
                                onChange={this.handleChange}
                                onSelect={this.handleSelect}
                            />
                            <a href="javascript:;" onClick={this.handleAddEmployee}>+ Add to list</a>
                        </div>
                    </th>
                </tr>
                <tr>
                    <th>Employee</th>
                    <th />
                </tr>
            </thead>
        );
    }
}