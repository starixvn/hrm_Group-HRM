import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import '../../date';
import {
    axiosError, floatThead, initDatepicker, setDateRange,
    blockUI, unblockUI, downloadFile, resolveClientUrl, sortTable
} from '../../shared/common.functions';

function dataChangeEvent() {
    $('#Date').val($('#DateOption').val());
    $('#Date').on('change', function () {
        const value = $(this).val();
        setDateRange(value);
        $('#DateOption').val(value);
    });
    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val(0);
        $('#DateOption').val(0);
    });
}

function GetOrgChart() {
    axios({
        url: resolveClientUrl('Section/GetOrgChart'),
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

function searchKeyword() {
    $('#btnSearch').on('click', function () {
        const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
        $('#TeamIds').val(teamIds === '' ? ' ' : teamIds);

        $('form.filter').submit();
    });
}

function exportReport() {
    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));

        blockUI();
        axios({
            url: resolveClientUrl('WorkingHoursSummaryReport/ExportExcel'),
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

function initSortTable() {
    $('th.col-sorter').on('click', function () {
        sortTable('table#data', this);
        $("table#data tbody tr").each(function (index) {
            var initIndex = index + 1;
            $(this).find("td:eq(0)").html(initIndex++);
        });
    });
}
$(document).ready(function () {
    floatThead('table#data');
    initSortTable();
    initDatepicker();
    dataChangeEvent();
    GetOrgChart();
    searchKeyword();
    exportReport();
});