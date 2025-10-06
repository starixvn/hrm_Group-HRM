import axios from 'axios';
import { toast } from 'react-toastify';
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
import { PageType } from '../../../shared/constants';

// Common employee search logic for basic managers (LeaderTakeoverManager, ShiftManager)
export const getEmployees = (query, pageType) => {
    if (query.trim().length <= 1) {
        return Promise.resolve([]);
    }

    let apiCall;
    const isGroupTeam = pageType === PageType.GroupTeam;
    
    if (isGroupTeam) {
        apiCall = getGroupTeamEmployees(query);
    } else {
        // For basic managers - use employee/getemployees API
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

    return apiCall
        .then(response => {
            return response.data.map(item => {
                const id = isGroupTeam ? item.empId : item.id;
                return {
                    key: id,
                    id: id,
                    teamCode: item.teamCode !== null ? item.teamCode : '',
                    value: pageType === PageType.GroupTeam
                        ? `${item.nickName} (${item.company})`
                        : item.nickName,
                };
            });
        })
        .catch(axiosError);
};

// Employee search logic for discipline managers (with excludeIds)
export const getEmployeesWithExclude = (query, pageType, roleId, excludeIds) => {
    if (query.trim().length <= 1) {
        return Promise.resolve([]);
    }

    let apiCall;
    const isGroupTeam = pageType === PageType.GroupTeam;
    
    if (isGroupTeam) {
        apiCall = getGroupTeamEmployees(query);
    } else {
        // For discipline managers - use getSupervisors with excludeIds
        apiCall = getSupervisors(query, roleId, excludeIds);
    }

    return apiCall
        .then(response => {
            return response.data.map(item => {
                const id = isGroupTeam ? item.empId : item.id;
                return {
                    key: id,
                    id: id,
                    teamCode: item.teamCode !== null ? item.teamCode : '',
                    value: pageType === PageType.GroupTeam
                        ? `${item.nickName} (${item.company})`
                        : item.nickName,
                };
            });
        })
        .catch(axiosError);
};

// Common get managers logic
export const getManagers = (pageType, objectId, apiEndpoint) => {
    blockUI();
    let url, params;

    if (pageType === PageType.Team) {
        url = `Team/${apiEndpoint}`;
        params = { teamId: objectId };
    } else if (pageType === PageType.GroupTeam) {
        url = `GroupTeam/${apiEndpoint}`;
        params = { teamId: objectId };
    } else {
        url = `Employee/${apiEndpoint}`;
        params = { employeeId: objectId };
    }

    return axios({
        url: resolveClientUrl(url),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        params: params,
    })
        .then(response => {
            unblockUI();
            return response.data;
        })
        .catch(error => {
            unblockUI();
            axiosError(error);
            return [];
        });
};

// Common add employee logic
export const addEmployee = (managers, employee) => {
    return [
        ...managers,
        {
            ownerId: employee.id,
            nickname: employee.value,
            canRemove: true,
        },
    ];
};

// Add employee logic with duplicate check (for ShiftManager)
export const addEmployeeWithDuplicateCheck = (managers, employee) => {
    const index = managers.findIndex(item => item.ownerId === employee.id);
    if (index < 0) {
        return [
            ...managers,
            {
                ownerId: employee.id,
                nickname: employee.value,
                canRemove: true,
            },
        ];
    }
    return managers; // Return original if duplicate
};

// Common remove employee logic
export const removeEmployee = (managers, employeeId) => {
    const index = managers.findIndex(item => item.ownerId === employeeId);
    if (index >= 0) {
        const newManagers = [...managers];
        newManagers.splice(index, 1);
        return newManagers;
    }
    return managers;
};

// Common change order logic (for components that support reordering)
export const changeOrder = (managers, key, employeeId, setStateCallback) => {
    reorderSupervisor(
        key,
        employeeId,
        [...managers],
        function (newManagers) {
            setStateCallback({ managers: newManagers });
        }
    );
};

// Common save logic
export const saveManagers = (managers, pageType, objectId, roleId) => {
    const supervisors = managers.map((item, index) => ({
        ownerId: item.ownerId,
        displayOrder: index + 1,
    }));

    if (pageType === PageType.Team) {
        editTeamSupervisor(objectId, roleId, supervisors, toast);
    } else if (pageType === PageType.GroupTeam) {
        editTeamSupervisor(objectId, roleId, supervisors, toast, 'GroupTeam');
    } else {
        editEmployeeSupervisor(objectId, roleId, supervisors, toast);
    }
};

// Common back URL logic
export const getBackUrl = (pageType) => {
    switch (pageType) {
        case PageType.Team:
            return 'team/TeamSupervisor';
        case PageType.GroupTeam:
            return 'GroupTeam/TeamSupervisor';
        case PageType.Employee:
        default:
            return 'employee/edit';
    }
};

// Common component initialization
export const initializeComponent = () => {
    const roleId = document.getElementById('RoleId').value;
    const companyId = (document.getElementById('CompanyId') || {}).value || '';
    
    return {
        roleId,
        companyId,
        state: {
            managers: [],
            employeeOptions: [],
        }
    };
};