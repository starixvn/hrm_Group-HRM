import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { axiosError, blockUI, unblockUI, floatThead, resolveClientUrl, downloadFile } from '../../shared/common.functions';

$(document).ready(function () {
    let query = function () {
        blockUI();

        axios({
            url: resolveClientUrl('report/GetUnpaidLeaveSummary'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                year: $('#Year').val(),
                teamIds: $('#TeamIds').val(),
                category: $('#Category').val(),
                employee: $('#Employee').val(),
                employeeId: $('#EmployeeId').val(),
                leaveType: $('#LeaveType.select-form').multiSelectTreeView('getSelectedItems'),
                period: $('#Period.select-form').multiSelectTreeView('getSelectedItems')
            },
            responseType: 'text'
        }).then(response => {
            $('#UnpaidLeaveSummaryDetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

    axios({
        url: resolveClientUrl('Section/GetOrgChart'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    }).then(response => {
        $('#Team').multiSelectTreeView({
            data: response.data,
            isSelectedAll: true,
            hasSearchBox: true
        });
    }).catch(axiosError);

    axios({
        url: resolveClientUrl('Report/GetUnpaidLeaveType'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    }).then(response => {
        $('#LeaveType').multiSelectTreeView({
            data: response.data,
            isSelectedAll: true
        });

        query();
    }).catch(axiosError);

    $('#Period').multiSelectTreeView({
        data: [{ text: '1H', checked: false, enable: true, isLeaf: true, id: 1 }, { text: '2H', checked: false, enable: true, isLeaf: true, id: 2 }],
        isSelectedAll: true
    });

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').click(function () {
        blockUI();
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        axios({
            url: resolveClientUrl('ExportLeaveSummary/UnpaidLeaveSummary'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                year: $('#Year').val(),
                teamIds: $('#TeamIds').val(),
                category: $('#Category').val(),
                employee: $('#Employee').val(),
                employeeId: $('#EmployeeId').val(),
                leaveType: $('#LeaveType.select-form').multiSelectTreeView('getSelectedItems'),
                period: $('#Period.select-form').multiSelectTreeView('getSelectedItems')
            },
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename=(.+)/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});