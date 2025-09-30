import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import { axiosError, floatThead, resolveClientUrl, blockUI, unblockUI, downloadFile, sortTable } from '../../shared/common.functions';

function initSortTable() {
    $('th.col-sorter').on('click', function () {
        sortTable('table#data', this);
        $("table#data tbody tr").each(function (index) {
            var initIndex = index + 1;
            $(this).find("td:eq(0)").html(initIndex++);
        });
    });
}

function searchKeyword() {
    $('#btnSearch').on('click', function () {
        const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
        $('#TeamIds').val(teamIds === '' ? ' ' : teamIds);

        $('#EmployeeStatusIds').val($('#EmployeeStatus').multiSelectTreeView('getSelectedItems'));

        $('form.filter').submit();
    });
}

function getOrgChart() {
    axios({
        url: resolveClientUrl('Team/GetOrgChart'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    }).then(response => {
        const teamIds = $('#TeamIds').val();

        $('#Team').multiSelectTreeView({
            data: response.data,
            selectedItems: teamIds,
            hasSearchBox: true
        });
    }).catch(axiosError);
}

function exportExcel() {
    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        $('#EmployeeStatusIds').val($('#EmployeeStatus').multiSelectTreeView('getSelectedItems'));
        blockUI();
        axios({
            url: resolveClientUrl('OvertimeReport/ExportYearlyOvertime'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
}

$(document).ready(function () {
    const statusIds = $('#EmployeeStatusIds').val().split(',');
    $('#EmployeeStatus').multiSelectTreeView({
        data: [
            { id: 1, text: 'Enable', enable: true, checked: statusIds.indexOf('1') !== -1, isLeaf: true },
            { id: 2, text: 'Disabled', enable: true, checked: statusIds.indexOf('2') !== -1, isLeaf: true },
            { id: 3, text: 'Deleted', enable: true, checked: statusIds.indexOf('3') !== -1, isLeaf: true }
        ]
    });

    floatThead('table#data');
    initSortTable();
    searchKeyword();
    getOrgChart();
    exportExcel();
});