import React, { PureComponent } from 'react';
import { RoleSystem } from '../constants';
import EmployeeAutocomplete from './EmployeeAutocomplete';

export default class SupervisorTHead extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            value: '',
            employee: null
        };
    }

    handleSelect = (value, item) => {
        this.setState({ value, employee: item });
    };

    handleChange = value => {
        this.setState({ value });
        this.props.onSearchEmployees(value);
    };

    handleAddEmployee = () => {
        const employee = this.state.employee;
        if (employee !== null) {
            this.props.onAddEmployee(employee);
            this.setState({ value: '', employee: null });
        }
    };

    render() {
        const { employeeOptions, roleId } = this.props;
        let roleName = '';
        switch (roleId) {
            case RoleSystem.Verifier:
                roleName = 'Verifier';
                break;
            case RoleSystem.Approver:
                roleName = 'Approver';
                break;
            case RoleSystem.OnBehalfRequest:
                roleName = 'Employee';
                break;
        }

        return (
            <thead>
                <tr>
                    <th colSpan={9}>
                        <div className="header-control">
                            <div>{roleName}</div>
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
                    <th rowSpan={2}>{roleName}</th>
                    <th colSpan={6}>Request</th>
                    <th rowSpan={2} />
                </tr>
                <tr>
                    <th>Leave</th>
                    <th>Change Shift</th>
                    <th>Late In/Early Out</th>
                    <th>Check in/out</th>
                    <th>Overtime</th>
                    <th>Compensation Leave</th>
                </tr>
            </thead>
        );
    }
}
