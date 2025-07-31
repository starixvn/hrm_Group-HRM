import React, { Component, Fragment } from 'react';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import axios from 'axios';

import { axiosError, blockUI, unblockUI, resolveClientUrl } from '../../../shared/common.functions';
import { editTeamSupervisor, editEmployeeSupervisor, reorderSupervisor } from '../supervisor/common.functions';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from '../discipline-manager/table/table';

const PageType = {
    Team: 1,
    Employee: 2
};

export default class ShiftManager extends Component {
    constructor(props) {
        super(props);

        this.state = {
            managers: [],
            employeeOptions: []
        };

        this.roleId = document.getElementById('RoleId').value;
    }

    componentDidMount() {
        this.getShiftManagers();
    }

    getEmployees = keyword => {
        if (keyword.trim().length > 1) {
            axios({
                url: resolveClientUrl("GroupTeam/getemployees"),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    keyword,
                    isEnable: true,
                    isDelete: false
                }
            }).then(response => {
                const employeeOptions = response.data.map(item => {
                    return {
                        key: `${item.empId}-${item.company}`,
                        id: item.empId,
                        nickname: item.nickName,
                        fullName: item.fullName,
                        teamCode: item.teamCode !== null ? item.teamCode : "",
                        value: `${item.nickName} (${item.company})`,
                        companyName: item.company !== null ? item.company : "",
                        companyId: item.companyId !== null ? item.companyId : "",
                    };
                });

                this.setState({ employeeOptions: employeeOptions });
            }).catch(axiosError);
        } else {
            this.setState({ employeeOptions: [] });
        }
    };

    getShiftManagers = () => {
        blockUI();
        let promise;
        if (this.props.pageType === PageType.Team) {
            promise = axios({
                url: resolveClientUrl('Team/GetShiftManagerByTeamId'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    teamId: this.props.objectId
                }
            });
        } else {
            promise = axios({
                url: resolveClientUrl('Employee/GetShiftManagerByEmployeeId'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    employeeId: this.props.objectId
                }
            });
        }

        promise.then(response => {
            this.setState({ managers: response.data });
        }).catch(axiosError).then(() => unblockUI());
    };

    handleChangeOrder = (key, employeeId) => {
        const vm = this;
        reorderSupervisor(key, employeeId, [...this.state.managers], function (managers) {
            vm.setState({ managers });
        });
    };

    handleAddEmployee = employee => {
        const index = this.state.managers.findIndex(item => item.ownerId === employee.id);
        this.setState({ employeeOptions: [] });
        if (index < 0) {
            const managers = [...this.state.managers, 
                { 
                    ownerId: employee.id, 
                    nickname: employee.value, 
                    fullName: employee.fullName,
                    companyName: employee.companyName
                }];
            this.setState({ managers });
        }
    };

    handleRemoveEmployee = employeeId => {
        const managers = [...this.state.managers],
            index = managers.findIndex(item => item.ownerId === employeeId);
        managers.splice(index, 1);
        this.setState({ managers });
    };

    handleSave = () => {
        const supervisors = this.state.managers.map((item, index) => {
            return {
                ownerId: item.ownerId,
                displayOrder: index + 1
            };
        });
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
                    onChangeOrder={this.handleChangeOrder}
                    onAddEmployee={this.handleAddEmployee}
                    onRemoveEmployee={this.handleRemoveEmployee}
                />
                <Button onClick={this.handleSave} bsStyle="primary">Save</Button>
                <a className="btn btn-default"
                    href={resolveClientUrl(`${this.props.pageType === PageType.Team ? 
                             "GroupTeam/TeamSupervisor"
                            : "GroupEmployeeList/Edit"}/${this.props.objectId}`)}
                >Back</a>
            </Fragment>
        );
    }
}