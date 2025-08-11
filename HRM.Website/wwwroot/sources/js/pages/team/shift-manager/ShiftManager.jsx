import React, { Component, Fragment } from 'react';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import axios from 'axios';

import {
    axiosError,
    blockUI,
    unblockUI,
    resolveClientUrl,
} from '../../../shared/common.functions';
import {
    editTeamSupervisor,
    editEmployeeSupervisor,
    reorderSupervisor,
    getGroupTeamEmployees,
} from '../supervisor/common.functions';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from '../discipline-manager/table/table';

const PageType = {
    Team: 1,
    Employee: 2,
    GroupTeam: 3,
};

export default class ShiftManager extends Component {
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
        this.getShiftManagers();
    }
    getEmployees = (query) => {
        if (query.trim().length > 1) {
            let apiCall;
            const isGroupTeam = this.props.pageType === PageType.GroupTeam;
            if (isGroupTeam) {
                apiCall = getGroupTeamEmployees(query);
            } else {
                apiCall = axios({
                    url: resolveClientUrl('employee/getemployees'),
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    params: {
                        query,
                        enable: true,
                        isDelete: false,
                    },
                });
            }

            apiCall
                .then((response) => {
                    const employeeOptions = response.data.map((item) => {
                        var id = isGroupTeam ? item.empId : item.id;
                        return {
                            key: id,
                            id: id,
                            teamCode:
                                item.teamCode !== null ? item.teamCode : '',
                            value:
                                this.props.pageType === PageType.GroupTeam
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

    getShiftManagers = () => {
        blockUI();

        let url, params;

        if (this.props.pageType === PageType.Team) {
            url = 'Team/GetShiftManagerByTeamId';
            params = { teamId: this.props.objectId };
        } else if (this.props.pageType === PageType.GroupTeam) {
            url = 'GroupTeam/GetShiftManagerByTeamId';
            params = { teamId: this.props.objectId };
        } else {
            url = 'Employee/GetShiftManagerByEmployeeId';
            params = { employeeId: this.props.objectId };
        }

        axios({
            url: resolveClientUrl(url),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: params,
        })
            .then((response) => {
                this.setState({ managers: response.data });
            })
            .catch(axiosError)
            .then(() => unblockUI());
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

    handleAddEmployee = (employee) => {
        const index = this.state.managers.findIndex(
            (item) => item.ownerId === employee.id
        );
        this.setState({ employeeOptions: [] });
        if (index < 0) {
            const managers = [
                ...this.state.managers,
                {
                    ownerId: employee.id,
                    nickname: employee.value,
                    canRemove: true,
                },
            ];
            this.setState({ managers });
        }
    };

    handleRemoveEmployee = (employeeId) => {
        const managers = [...this.state.managers],
            index = managers.findIndex((item) => item.ownerId === employeeId);
        managers.splice(index, 1);
        this.setState({ managers });
    };

    handleSave = () => {
        const supervisors = this.state.managers.map((item, index) => {
            return {
                ownerId: item.ownerId,
                displayOrder: index + 1,
            };
        });
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
