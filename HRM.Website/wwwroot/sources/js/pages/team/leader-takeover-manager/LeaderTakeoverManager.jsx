import React, { Component, Fragment } from 'react';
import { Button } from 'react-bootstrap';
import { resolveClientUrl } from '../../../shared/common.functions';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from './table/table';
import {
    getEmployees,
    getManagers,
    addEmployee,
    removeEmployee,
    saveManagers,
    getBackUrl,
    initializeComponent
} from '../shared/teamManagerHelpers';

export default class LeaderTakeoverManager extends Component {
    constructor(props) {
        super(props);
        const { roleId, companyId, state } = initializeComponent();
        this.state = state;
        this.roleId = roleId;
        this.companyId = companyId;
    }

    componentDidMount() {
        this.getLeaderTakeoverManagers();
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

    getLeaderTakeoverManagers = () => {
        getManagers(this.props.pageType, this.props.objectId, 'GetLeaderTakeoverManagerByTeamId')
            .then(managers => {
                this.setState({ managers });
            });
    };

    handleAddEmployee = employee => {
        const managers = addEmployee(this.state.managers, employee);
        this.setState({ managers, employeeOptions: [] });
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
                    onAddEmployee={this.handleAddEmployee}
                    onRemoveEmployee={this.handleRemoveEmployee}
                    companyId={this.companyId}
                    pageType={this.props.pageType}
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

