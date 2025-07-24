import axios from 'axios';
import { stringify } from 'qs';

import {
    reorder,
    resolveClientUrl,
    axiosError,
} from '../../../shared/common.functions';

function getSupervisors(query, roleIds, excludeIds) {
    return axios(resolveClientUrl('groupEmployeeList/GetSupervisors'), {
        params: {
            roleIds,
            query,
            excludeIds,
        },
    });
}

function getOrgChart(companyId = null) {
    return axios({
        method: 'GET',
        url: resolveClientUrl('Team/GetOrgChart'),
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        params: companyId ? { companyId } : {},
    });
}

function editEmployeeSupervisor(employeeId, roleId, supervisors, toastObj) {
    axios({
        url: resolveClientUrl('employee/EditEmployeeSupervisor'),
        method: 'POST',
        data: stringify({
            employeeId,
            roleId,
            supervisors,
        }),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded',
        },
    })
        .then((response) => {
            const data = response.data;
            if (data.isSuccessful) {
                toastObj.success('Updated Successfully!');
            } else {
                toastObj.error(data.message);
            }
        })
        .catch(axiosError);
}

function editTeamSupervisor(teamId, roleId, supervisors, toastObj) {
    axios({
        url: resolveClientUrl('GroupTeam/EditTeamSupervisor'),
        method: 'POST',
        data: stringify({
            teamId,
            roleId,
            supervisors,
        }),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded',
        },
    })
        .then((response) => {
            const data = response.data;
            if (data.isSuccessful) {
                toastObj.success('Updated Successfully!');
            } else {
                toastObj.error(data.message);
            }
        })
        .catch(axiosError);
}

function reorderSupervisor(key, employeeId, supervisors, callback) {
    const totalItems = supervisors.length,
        sourceIndex = supervisors.findIndex(
            (item) => item.ownerId === employeeId
        );
    let destinationIndex = 0;

    switch (key) {
        case 'up':
            destinationIndex = sourceIndex - 1;
            break;
        case 'down':
            destinationIndex = sourceIndex + 1;
            break;
        case 'bottom':
            destinationIndex = totalItems;
            break;
    }

    if (
        destinationIndex < 0 ||
        destinationIndex > totalItems ||
        sourceIndex === destinationIndex
    ) {
        return;
    }

    if (callback !== null && typeof callback === 'function') {
        supervisors = reorder(supervisors, sourceIndex, destinationIndex);
        callback(supervisors);
    }
}

function moveUp(vm) {
    const { ownerId } = vm.props.data;
    vm.props.onChangeOrder('up', ownerId);
}

function moveDown(vm) {
    const { ownerId } = vm.props.data;
    vm.props.onChangeOrder('down', ownerId);
}

function moveTop(vm) {
    const { ownerId } = vm.props.data;
    vm.props.onChangeOrder('top', ownerId);
}

function moveBottom(vm) {
    const { ownerId } = vm.props.data;
    vm.props.onChangeOrder('bottom', ownerId);
}

export {
    getSupervisors,
    getOrgChart,
    editEmployeeSupervisor,
    editTeamSupervisor,
    reorderSupervisor,
    moveUp,
    moveDown,
    moveTop,
    moveBottom,
};
