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

$(document).ready(function () {
    initDatepicker();

    let query = function () {
        blockUI();
        axios({
            url: resolveClientUrl('MealAllowanceReport/GetMealAllowanceSummary'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#MealAllowanceSummary').html(response.data);
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
            url: resolveClientUrl('MealAllowanceReport/ExportExcel'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });

    $('#btnExcelDetails').on('click', function () {
        $('.filter-details #TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        blockUI();
        axios({
            url: resolveClientUrl('MealAllowanceReport/ExportExcel'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('.filter-details form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});

$(document).on('click', '.viewdetails', function () {
    var empId = $(this).data('empid'),
        employee = $(this).data('employee'),
        teamIds = $('form.filter #TeamIds').val(),
        isLocal = $('form.filter #IsLocal').val(),
        fromDate = $('form.filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
        toDate = $('form.filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val();

    window.open(resolveClientUrl('MealAllowanceReport/MealAllowanceDetails?fromDate=' + fromDate + '&toDate=' + toDate
        + '&employeeId=' + empId + '&employee=' + employee + '&teamIds=' + teamIds
        + '&isLocal=' + isLocal), '_blank');
});