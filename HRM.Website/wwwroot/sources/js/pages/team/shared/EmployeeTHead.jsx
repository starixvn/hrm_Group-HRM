import React, { PureComponent } from 'react';
import EmployeeAutocomplete from '../../../shared/components/EmployeeAutocomplete';

export default class EmployeeTHead extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            value: '',
            employee: null
        };
    }

    handleChange = value => {
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
        const { employeeOptions, title = 'Employee' } = this.props;

        return (
            <thead>
                <tr>
                    <th colSpan={2}>
                        <div className="header-control">
                            <div>{title}</div>
                            <EmployeeAutocomplete value={this.state.value}
                                options={employeeOptions}
                                onChange={this.handleChange}
                                onSelect={this.handleSelect}
                            />
                            <button type="button" className="btn btn-link btn-add-list" onClick={this.handleAddEmployee}>+ Add to list</button>
                        </div>
                    </th>
                </tr>
                <tr>
                    <th>{title}</th>
                    <th />
                </tr>
            </thead>
        );
    }
}