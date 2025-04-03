import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import { parse } from 'qs';
import 'floatthead/dist/jquery.floatThead';

import '../../shared/multiselect-tree-view';
import {
    axiosError, initDatepicker, floatThead,
    blockUI, unblockUI, downloadFile, resolveClientUrl, sortTable
} from '../../shared/common.functions';

$(document).ready(function () {
    initDatepicker();

    let query = function () {
        blockUI();
        axios({
            url: resolveClientUrl('WorkingCalendarSummaryReport/GetWorkingCalendarSummary'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#WorkingCalendarDetails').html(response.data);
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

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        query();
    });

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
    }).catch(axiosError);

    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));

        blockUI();
        axios({
            url: resolveClientUrl('WorkingCalendarSummaryReport/ExportExcel'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});