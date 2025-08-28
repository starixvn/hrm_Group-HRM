import React, { Component, Fragment } from "react";
import { Button } from "react-bootstrap";
import { toast } from "react-toastify";
import axios from "axios";

import {
  axiosError,
  blockUI,
  unblockUI,
  resolveClientUrl,
} from "../../../shared/common.functions";
import { RequestType } from "../../../shared/constants";
import {
  editTeamSupervisor,
  editEmployeeSupervisor,
  reorderSupervisor,
} from "../supervisor/common.functions";
import ToastLayout from "../../../shared/layouts/ToastLayout";
import Table from "./table/table";

const PageType = {
  Team: 1,
  Employee: 2,
};

export default class Supervisor extends Component {
  constructor(props) {
    super(props);

    this.state = {
      supervisors: [],
      employeeOptions: [],
    };

    this.roleId = document.getElementById("RoleId").value;
  }

  componentDidMount() {
    this.getSupervisors();
  }

  getEmployees = keyword => {
    if (keyword.trim().length > 1) {
      axios({
        url: resolveClientUrl("GroupTeam/getemployees"),
        method: "GET",
        headers: { "X-Requested-With": "XMLHttpRequest" },
        params: {
          keyword,
          isEnable: true,
          isDelete: false,
        },
      })
        .then(response => {
          const employeeOptions = response.data.map(item => {
            return {
              key: item.empId,
              id: item.empId,
              nickname: item.nickName,
              fullName: item.fullName,
              teamCode: item.teamCode !== null ? item.teamCode : '',
              value: `${item.nickName} (${item.company})`,
              companyName: item.company !== null ? item.company : '',
            };
          });
          this.setState({ employeeOptions: employeeOptions });
        })
        .catch(axiosError);
    } else {
      this.setState({ employeeOptions: [] });
    }
  };

  getSupervisors = () => {
    blockUI();
    let promise;
    if (this.props.pageType === PageType.Team) {
      promise = axios({
        url: resolveClientUrl("GroupTeam/GetTeamSupervisorByRoleId"),
        method: "GET",
        headers: { "X-Requested-With": "XMLHttpRequest" },
        params: {
          teamId: this.props.objectId,
          roleId: this.roleId,
        },
      });
    } else {
      promise = axios({
        url: resolveClientUrl("Employee/GetEmployeeSupervisorByRoleId"),
        method: "GET",
        headers: { "X-Requested-With": "XMLHttpRequest" },
        params: {
          employeeId: this.props.objectId,
          roleId: this.roleId,
        },
      });
    }
    promise
      .then(response => {
        this.setState({ supervisors: response.data });
      })
      .catch(axiosError)
      .then(() =>unblockUI());
  };

  handleChangeOrder = (key, employeeId) => {
    const vm = this;
    reorderSupervisor(
      key,
      employeeId,
      [...this.state.supervisors],
      function (supervisors) {
        vm.setState({ supervisors });
      }
    );
  };

  handleAddEmployee = employee => {
    const index = this.state.supervisors.findIndex(item => item.ownerId === employee.id);
    this.setState({ employeeOptions: [] });
    if (index < 0) {
      const supervisors = [
        ...this.state.supervisors,
        {
          ownerId: employee.id,
          nickName: employee.value,
          fullName: employee.fullName,
          teamCode: employee.teamCode,
          companyName: employee.companyName,
          leaves: true,
          changeShifts: true,
          lateInEarlyOuts: true,
          checkInOuts: true,
          overtimes: true,
          keepInViews: true,
        },
      ];
      this.setState({ supervisors });
    }
  };

  handleRemoveEmployee = employeeId => {
    const supervisors = [...this.state.supervisors],
      index = supervisors.findIndex(item =>item.ownerId === employeeId);
    supervisors.splice(index, 1);
    this.setState({ supervisors });
  };

  handleChangePermissionOnRequest = (employeeId, requestType, isChecked) => {
    const supervisors = [...this.state.supervisors],
      index = supervisors.findIndex(item =>item.ownerId === employeeId);
    supervisors[index] = { ...supervisors[index], [requestType]: isChecked };
    this.setState({ supervisors });
  };

  handleSave = () => {
    const supervisors = [];
    const requestTypes = [
      { key: "leaves", value: RequestType.Leave },
      { key: "changeShifts", value: RequestType.ChangeShift },
      { key: "lateInEarlyOuts", value: RequestType.LateIn },
      { key: "lateInEarlyOuts", value: RequestType.EarlyOut },
      { key: "checkInOuts", value: RequestType.CheckIn },
      { key: "checkInOuts", value: RequestType.CheckOut },
      { key: "overtimes", value: RequestType.Overtime },
      { key: "keepInViews", value: RequestType.KeepInView },
    ];
    this.state.supervisors.forEach((item, index) => {
      const ownerId = item.ownerId;
      const displayOrder = index + 1;
      requestTypes.forEach(requestType => {
        if (item[requestType.key]) {
          supervisors.push({
            ownerId,
            requestType: requestType.value,
            displayOrder,
          });
        }
      });
    });
    if (this.props.pageType === PageType.Team) {
      editTeamSupervisor(this.props.objectId, this.roleId, supervisors, toast);
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
    const { employeeOptions, supervisors } = this.state;
    return (
      <Fragment>
        <ToastLayout />
        <Table
          data={supervisors}
          roleId={this.roleId}
          employeeOptions={employeeOptions}
          onSearchEmployees={this.getEmployees}
          onAddEmployee={this.handleAddEmployee}
          onChangePermissionOnRequest={this.handleChangePermissionOnRequest}
          onRemoveEmployee={this.handleRemoveEmployee}
          onChangeOrder={this.handleChangeOrder}
        />
        <Button onClick={this.handleSave} bsStyle="primary">
          Save
        </Button>
        <a
          className="btn btn-default"
          href={resolveClientUrl(
            `${
              this.props.pageType === PageType.Team
                ? "GroupTeam/TeamSupervisor"
                : "GroupEmployeeList/Edit"
            }/${this.props.objectId}`
          )}
        >
          Back
        </a>
      </Fragment>
    );
  }
}

