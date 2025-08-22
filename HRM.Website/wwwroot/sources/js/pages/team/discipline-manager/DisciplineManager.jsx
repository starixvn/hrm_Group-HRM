import React, { Component, Fragment } from 'react';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import axios from 'axios';

import {
    axiosError,
    blockUI,
    unblockUI,
    resolveClientUrl
} from '../../../shared/common.functions';
import {
    getSupervisors,
    getGroupTeamEmployees,
    editTeamSupervisor,
    editEmployeeSupervisor,
    reorderSupervisor
} from '../supervisor/common.functions';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from './table/table';

const PageType = {
    Team: 1,
    Employee: 2,
    GroupTeam: 3
};

export default class DisciplineManager extends Component {
    constructor(props) {
        super(props);

        this.state = {
            managers: [],
            employeeOptions: [],
        };

        this.roleId = document.getElementById('RoleId').value;
        const companyId = (document.getElementById('CompanyId') || {}).value || '';
        this.companyId = companyId;
    }

    componentDidMount() {
        this.getDisciplineManagers();
    }

    getEmployees = query => {
        if (query.trim().length > 1) {
            const excludeIds = this.state.managers
                .map(item =>item.ownerId)
                .join(',');

            let apiCall;
            const isGroupTeam = this.props.pageType === PageType.GroupTeam;
            if (isGroupTeam) {
                apiCall = getGroupTeamEmployees(query);
            } else {
                apiCall = getSupervisors(query, this.roleId, excludeIds);
            }
            
            apiCall
                .then(response => {
                    const employeeOptions = response.data.map(item => {
                        const id = isGroupTeam ? item.empId : item.id;
                        return {
                            key: id,
                            id: id,
                            teamCode: item.teamCode !== null ? item.teamCode : '',
                            value: this.props.pageType === PageType.GroupTeam
                                    ? `${item.nickName} (${item.company})`
                                    : item.nickName,
                        };
                    });

                    this.setState({ employeeOptions: employeeOptions });
                })
                .catch(axiosError);
        } else {
            this.setState({ employeeOptions: [] });
        }
    };

    getDisciplineManagers = () => {
        blockUI();
        let url, params;

        if (this.props.pageType === PageType.Team) {
            url = 'Team/GetDisciplineManagerByTeamId';
            params = { teamId: this.props.objectId };
        } else if (this.props.pageType === PageType.GroupTeam) {
            url = 'GroupTeam/GetDisciplineManagerByTeamId';
            params = { teamId: this.props.objectId };
        } else {
            url = 'Employee/GetDisciplineManagerByEmployeeId';
            params = { employeeId: this.props.objectId };
        }

        axios({
            url: resolveClientUrl(url),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: params,
        })
            .then(response => {
                this.setState({ managers: response.data });
            })
            .catch(axiosError)
            .then(() => unblockUI());
    };

    handleAddEmployee = employee => {
        const managers = [
            ...this.state.managers,
            {
                ownerId: employee.id,
                nickname: employee.value,
                canRemove: true,
            },
        ];
        this.setState({ managers, employeeOptions: [] });
    };

    handleRemoveEmployee = employeeId => {
        const managers = [...this.state.managers],
            index = managers.findIndex(item =>item.ownerId === employeeId);
        managers.splice(index, 1);
        this.setState({ managers });
    };

    handleChangeOrder = (key, employeeId) => {
        const vm = this;
        reorderSupervisor(
            key,
            employeeId,
            [...this.state.managers],
            function (managers) {
                vm.setState({ managers });
            }
        );
    };

    handleSave = () => {
        const totalItems = this.state.managers.length;
        const supervisors = [];
        for (let i = 0; i < totalItems; i++) {
            supervisors.push({
                ownerId: this.state.managers[i].ownerId,
                displayOrder: i + 1,
            });
        }
        if (this.props.pageType === PageType.Team) {
            editTeamSupervisor(
                this.props.objectId,
                this.roleId,
                supervisors,
                toast
            );
        } else if (this.props.pageType === PageType.GroupTeam) {
            editTeamSupervisor(
                this.props.objectId,
                this.roleId,
                supervisors,
                toast,
                'GroupTeam'
            );
        } else {
            editEmployeeSupervisor(
                this.props.objectId,
                this.roleId,
                supervisors,
                toast
            );
        }
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
                    onChangeOrder={this.handleChangeOrder}
                    companyId={this.companyId}
                />
                <Button onClick={this.handleSave} bsStyle="primary">
                    Save
                </Button>
                <a
                    className="btn btn-default"
                    href={resolveClientUrl(
                        `${this.getBackUrl()}/${this.props.objectId}`
                    )}>
                    Back
                </a>
            </Fragment>
        );
    }

    getBackUrl = () => {
        switch (this.props.pageType) {
            case PageType.Team:
                return 'team/TeamSupervisor';
            case PageType.GroupTeam:
                return 'GroupTeam/TeamSupervisor';
            case PageType.Employee:
            default:
                return 'employee/edit';
        }
    };
}

