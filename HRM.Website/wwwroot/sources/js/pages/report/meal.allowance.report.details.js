import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import { parse } from 'qs';
import 'floatthead/dist/jquery.floatThead';

import '../../shared/employee.search.in.team';
import '../../shared/multiselect-tree-view';
import {
    axiosError, initDatepicker, floatThead,
    blockUI, unblockUI, downloadFile, resolveClientUrl, sortTable
} from '../../shared/common.functions';

$(document).on('click', '#btnSearchDetails', function () {
    $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
    query();
});

let query = function () {
    blockUI();
    axios({
        url: resolveClientUrl('MealAllowanceReport/MealAllowanceDetailsGrid'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        params: parse($('form.filter-detail').serialize()),
        responseType: 'text'
    }).then(response => {
        $('#MealAllowanceDetaills').html(response.data);
        floatThead('table#data');

        $('th.col-sorter').on('click', function () {
            sortTable('table#data', this);
            $("table#data tbody tr").each(function (index) {
                var initIndex = index + 1;
                $(this).find("td:eq(0)").html(initIndex++);
            });
        });
    }).catch(axiosError).then(() => unblockUI());
};

$(document).on('click', '#btnExcelDetails', function () {
    $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
    blockUI();
    axios({
        url: resolveClientUrl('MealAllowanceReport/ExportExcel'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        params: parse($('form.filter-detail').serialize()),
        responseType: 'blob'
    }).then(response => {
        const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
        downloadFile(response.data, fileName);
    }).catch(axiosError).then(() => unblockUI());
});

$(document).ready(function () {
    initDatepicker();

    axios({
        url: resolveClientUrl('Team/GetOrgChart'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    }).then(response => {
        $('#Team').multiSelectTreeView({
            data: response.data,
            isSelectedAll: true,
            hasSearchBox: true
        });
        query();
    }).catch(axiosError);
});