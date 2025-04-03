import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import {
    axiosError, floatThead, initDatepicker, resolveClientUrl,
    blockUI, unblockUI, downloadFile, sortTable
} from '../../shared/common.functions';

function initSortTable() {
    $('th.col-sorter').on('click', function () {
        sortTable('table#data', this);
    });
}

function getOrgChart() {
    axios({
        url: resolveClientUrl('Team/GetOrgChart'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    }).then(response => {
        $('#Team').multiSelectTreeView({
            data: response.data,
            selectedItems: $('#TeamIds').val(),
            hasSearchBox: true
        });
    }).catch(axiosError);
}

function searchKeyWord() {
    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        $('form.filter').submit();
    });
}

function exportExcel() {
    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));

        blockUI();
        axios({
            url: resolveClientUrl('OvertimeReport/ExportDailyOvertime'),
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
    initDatepicker();
    floatThead('table#data');
    initSortTable();
    getOrgChart();
    searchKeyWord();
    exportExcel();
});