import React, { Component, Fragment } from 'react';
import { Button } from 'react-bootstrap';
import { resolveClientUrl } from '../../../shared/common.functions';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from '../discipline-manager/table/table';
import {
    getEmployees,
    getManagers,
    addEmployeeWithDuplicateCheck,
    removeEmployee,
    changeOrder,
    saveManagers,
    getBackUrl,
    initializeComponent
} from '../shared/teamManagerHelpers';
import { PageType } from '../../../shared/constants';

export default class ShiftManager extends Component {
    constructor(props) {
        super(props);
        const { roleId, companyId, state } = initializeComponent();
        this.state = state;
        this.roleId = roleId;
        this.companyId = companyId;
    }

    componentDidMount() {
        this.getShiftManagers();
    }
    getEmployees = query => {
        getEmployees(query, this.props.pageType)
            .then(employeeOptions => {
                this.setState({ employeeOptions });
            })
            .catch(() => {
                this.setState({ employeeOptions: [] });
            });
    };

    getShiftManagers = () => {
        const apiEndpoint = this.props.pageType === PageType.Employee ? 'GetShiftManagerByEmployeeId' : 'GetShiftManagerByTeamId';
        getManagers(this.props.pageType, this.props.objectId, apiEndpoint)
            .then(managers => {
                this.setState({ managers });
            });
    };

    handleChangeOrder = (key, employeeId) => {
        changeOrder(this.state.managers, key, employeeId, this.setState.bind(this));
    };

    handleAddEmployee = employee => {
        this.setState({ employeeOptions: [] });
        const managers = addEmployeeWithDuplicateCheck(this.state.managers, employee);
        this.setState({ managers });
    };

    handleRemoveEmployee = employeeId => {
        const managers = removeEmployee(this.state.managers, employeeId);
        this.setState({ managers });
    };

    handleSave = () => {
        saveManagers(this.state.managers, this.props.pageType, this.props.objectId, this.roleId);
    };

    render() {
        const { employeeOptions, managers } = this.state;
        return (
            <Fragment>
                <ToastLayout />
                <Table
                    data={managers}
                    employeeOptions={employeeOptions}
                    onSearchEmployees={this.getEmployees}
                    onChangeOrder={this.handleChangeOrder}
                    onAddEmployee={this.handleAddEmployee}
                    onRemoveEmployee={this.handleRemoveEmployee}
                    companyId={this.companyId}
                />
                <Button onClick={this.handleSave} bsStyle="primary">
                    Save
                </Button>
                <a
                    className="btn btn-default"
                    href={resolveClientUrl(
                        `${getBackUrl(this.props.pageType)}/${this.props.objectId}`
                    )}>
                    Back
                </a>
            </Fragment>
        );
    }

}

