import React, { Component, Fragment } from 'react';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import axios from 'axios';

import { axiosError, blockUI, unblockUI, resolveClientUrl } from '../../../shared/common.functions';
import { getSupervisors, editTeamSupervisor, editEmployeeSupervisor, reorderSupervisor } from '../supervisor/common.functions';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from './table/table';

const PageType = {
    Team: 1,
    Employee: 2
};

export default class DisciplineManager extends Component {
    constructor(props) {
        super(props);

        this.state = {
            managers: [],
            employeeOptions: []
        };

        this.roleId = document.getElementById('RoleId').value;
    }

    componentDidMount() {
        this.getDisciplineManagers();
    }

    getEmployees = query => {
        if (query.trim().length > 1) {
            const excludeIds = this.state.managers.map(item => item.ownerId).join(',');

            getSupervisors(query, this.roleId, excludeIds).then(response => {
                const employeeOptions = response.data.map(item => {
                    return {
                        id: item.id,
                        nickname: item.nickName,
                        fullName: item.fullName,
                        value: item.displayName
                    };
                });

                this.setState({ employeeOptions: employeeOptions });
            }).catch(axiosError);
        } else {
            this.setState({ employeeOptions: [] });
        }
    };

    getDisciplineManagers = () => {
        blockUI();
        if (this.props.pageType === PageType.Team) {
            axios({
                url: resolveClientUrl('Team/GetDisciplineManagerByTeamId'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    teamId: this.props.objectId
                }
            }).then(response => {
                this.setState({ managers: response.data });
            }).catch(axiosError).then(() => unblockUI());
        } else {
            axios({
                url: resolveClientUrl('Employee/GetDisciplineManagerByEmployeeId'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    employeeId: this.props.objectId
                }
            }).then(response => {
                this.setState({ managers: response.data });
            }).catch(axiosError).then(() => unblockUI());
        }
    };

    handleAddEmployee = employee => {
        const managers = [...this.state.managers, { ownerId: employee.id, nickname: employee.nickname, fullName: employee.fullName }];
        this.setState({ managers, employeeOptions: [] });
    };

    handleRemoveEmployee = employeeId => {
        const managers = [...this.state.managers],
            index = managers.findIndex(item => item.ownerId === employeeId);
        managers.splice(index, 1);
        this.setState({ managers });
    };

    handleChangeOrder = (key, employeeId) => {
        const vm = this;
        reorderSupervisor(key, employeeId, [...this.state.managers], function (managers) {
            vm.setState({ managers });
        });
    };

    handleSave = () => {
        const totalItems = this.state.managers.length;
        const supervisors = [];
        for (let i = 0; i < totalItems; i++) {
            supervisors.push({
                ownerId: this.state.managers[i].ownerId,
                displayOrder: i + 1
            });
        }
        if (this.props.pageType === PageType.Team) {
            editTeamSupervisor(this.props.objectId, this.roleId, supervisors, toast);
        } else {
            editEmployeeSupervisor(this.props.objectId, this.roleId, supervisors, toast);
        }
    };

    render() {
        const { employeeOptions, managers } = this.state;

        return (
            <Fragment>
                <ToastLayout />
                <Table data={managers}
                    employeeOptions={employeeOptions}
                    onSearchEmployees={this.getEmployees}
                    onAddEmployee={this.handleAddEmployee}
                    onRemoveEmployee={this.handleRemoveEmployee}
                    onChangeOrder={this.handleChangeOrder}
                />
                <Button onClick={this.handleSave} bsStyle="primary">Save</Button>
                <a className="btn btn-default"
                    href={resolveClientUrl(`${this.props.pageType === PageType.Team ? 'team/TeamSupervisor' : 'employee/edit'}/${this.props.objectId}`)}
                >Back</a>
            </Fragment>
        );
    }
}