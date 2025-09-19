import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import '../../jquery-weekpicker';
import moment from 'moment';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import { axiosError, blockUI, unblockUI, floatThead, resolveClientUrl, downloadFile, sortTable } from '../../shared/common.functions';

$(document).ready(function () {
    let query = function () {
        blockUI();
        axios({
            url: resolveClientUrl('WeeklyWorkingHours/GetWeeklyWorkingHours'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#WeeklyWorkingHoursDetails').html(response.data);
            floatThead('table#data');
            $('th.col-sorter').on('click', function () {
                sortTable('table#data', this);
                $('table#data tbody tr').each(function (index) {
                    let rowIndex = index + 1;
                    $(this).find('td:eq(0)').html(rowIndex++);
                });
            });
        }).catch(axiosError).then(() => unblockUI());
    };

    $(document).on('mouseup', function (e) {
        let $wrapper = $('.weekpicker-wrapper');

        if (!$wrapper.is(e.target) && $wrapper.has(e.target).length === 0 && $wrapper.is(':visible')) {
            $('#week-picker').hide();
        }
    });

    $('#week-picker').weekpicker({
        firstDay: 1,
        currentText: $('#FromDate').val(),
        onSelect: function (dateText, startDateText, startDate, endDate) {
            $('#FromDate').val(moment(startDate).format('DD MMM, YYYY'));
            $('#ToDate').val(moment(endDate).format('DD MMM, YYYY'));
            $('#week-picker').hide();
        }
    });

    $('#week-picker').hide();

    $('#FromDate, #ToDate, .icon-calendar').on('click', function () {
        $('#week-picker').toggle();
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

    $('#EmployeeStatus').multiSelectTreeView({
        data: [
            { id: 1, text: 'Enable', enable: true, checked: true, isLeaf: true },
            { id: 2, text: 'Disabled', enable: true, checked: false, isLeaf: true },
            { id: 3, text: 'Deleted', enable: true, checked: false, isLeaf: true }
        ]
    });

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        $('#EmployeeStatusIds').val($('#EmployeeStatus').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        $('#EmployeeStatusIds').val($('#EmployeeStatus').multiSelectTreeView('getSelectedItems'));

        blockUI();
        axios({
            url: resolveClientUrl('WeeklyWorkingHours/ExportExcel'),
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