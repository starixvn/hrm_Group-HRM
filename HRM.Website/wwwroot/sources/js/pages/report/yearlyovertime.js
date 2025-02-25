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

function searchKeyWord() {
    $('#btnSearch').on('click', function () {
        const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
        $('#TeamIds').val(teamIds === '' ? ' ' : teamIds);
        $('form.filter').submit();
    });
}

function getOrgChart() {
    axios({
        url: resolveClientUrl('Section/GetOrgChart'),
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
    floatThead('table#data');
    initSortTable();
    searchKeyWord();
    getOrgChart();
    exportExcel();
});