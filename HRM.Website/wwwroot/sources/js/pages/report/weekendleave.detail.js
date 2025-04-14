import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../../libs/monthPicker/monPickr-0.0.3';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import '../../date';
import {
    axiosError, blockUI, unblockUI, initDatepicker, floatThead,
    resolveClientUrl, downloadFile
} from '../../shared/common.functions';

function initEventForDateRangeDropdown() {
    $('form.filter #DateRange').on('change', function () {
        const year = $('form.filter #DateRange').val();

        if (parseInt(year) !== 0) {
            var fromDate = new Date($('form.filter #FromDate').val());
            var toDate = new Date($('form.filter #ToDate').val());

            fromDate.setFullYear(parseInt(year), 0, 1);
            toDate.setFullYear(parseInt(year), 11, 31);

            $('form.filter #FromDate').val(formatDate(fromDate));
            $('form.filter #ToDate').val(formatDate(toDate));
        }
    });
}

function formatDate(date) {
    var monthNames = [
        "Jan", "Feb", "Mar",
        "Apr", "May", "Jun", "Jul",
        "Aug", "Sep", "Oct",
        "Nov", "Dec"
    ];

    var day = date.getDate();
    var monthIndex = date.getMonth();
    var year = date.getFullYear();

    return (day < 10 ? "0" + day : day) + ' ' + monthNames[monthIndex] + ', ' + year;
}

function query() {
    blockUI();
    axios({
        url: resolveClientUrl('report/PartialListWeekendLeaveDetail'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        params: parse($('form.filter').serialize()),
        responseType: 'text'
    }).then(response => {
        $('#weekend-leave-detail').html(response.data);
        floatThead('table#data');
    }).catch(axiosError).then(() => unblockUI());
}

$(document).ready(function () {
    initEventForDateRangeDropdown();

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

    initDatepicker();

    $('form.filter #btnSubmit').on('click', function (event) {
        $('#TeamId').val($('#Team').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('form.filter #btnExcel').on('click', function (event) {
        event.preventDefault();

        blockUI();
        axios({
            url: resolveClientUrl('ExportWeekendLeave/ExportWeekendLeaveDetailReport'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename=([^;]+)/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});