import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import tippy from 'tippy.js';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import '../../date';
import '../request/date.filter';
import {
    blockUI, unblockUI, floatThead, initDatepicker,
    resolveClientUrl, downloadFile, axiosError, sortTable
} from '../../shared/common.functions';
import { CurrentRequestStatus } from '../../shared/constants';

function initTooltip() {
    tippy('.tooltip-request', {
        maxWidth: 'none',
        animation: 'fade',
        arrow: true,
        placement: 'auto',
        allowHTML: true,
        appendTo: document.getElementById('tippy-request'),
        content: reference => {
            const object = $(reference),
                result = $('<div/>').html(object.data('requestdetail')).text(),
                leavetype = object.data('leavetype');
            let status = '';
            switch (parseInt(object.attr('data-currentStatus'))) {
                case CurrentRequestStatus.Pending:
                    status = "Pending";
                    break;
                case CurrentRequestStatus.Verified:
                    status = "Verified";
                    break;
                case CurrentRequestStatus.Approved:
                    status = "Approved";
                    break;
                case CurrentRequestStatus.Processed:
                    status = "Processed";
                    break;
                default:
                    status = "Approved";
            }

            return "<div>" + "<table class='table-tooltip checkinout'>" +
                (leavetype ? "<tr class='hasborder'><td class='col1'>Leave Type</td><td class='col2'>" + leavetype + "</td></tr>" : '') +
                "<tr class='hasborder'>" +
                "<td class='col1'>Details</td>" +
                "<td class='col2'>" + result +
                "</td></tr><tr class='hasborder'><td class='col1'>Created Date</td><td class='col2'>" +
                object.attr('data-createddate') +
                "</td></tr><tr  class='hasborder'><td class='col1'>Remark</td><td class='col2'>" +
                object.attr('data-remark') + "</td></tr>" +
                "</td></tr><tr><td class='col1'>Status</td><td class='col2'>" + status + "</td></table ></div>";
        }
    });

    tippy('.tooltip-tippy', {
        animation: 'fade',
        arrow: true,
        placement: 'auto'
    });
}

function query() {
    blockUI();
    axios({
        url: resolveClientUrl('report/attendance/details'),
        method: 'POST',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        params: parse($('form.filter').serialize()),
        responseType: 'text'
    }).then(response => {
        $('#attendance-report').html(response.data);
        initSortTable();
        floatThead('table#data');
        initTooltip();
    }).catch(axiosError).then(() => unblockUI());
}

function initSortTable() {
    $('th.col-sorter').on('click', function () {
        sortTable('#data', this);
        $("#data tbody tr").each(function (index) {
            var initIndex = index + 1;
            $(this).find("td:eq(0)").html(initIndex++);
        });
    });
}

$(document).ready(function () {
    $('#ExcludeDeletedUsers').on('change', function (event) {
        $(this).val(event.target.checked);
    });

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

        query();
    }).catch(axiosError);

    initDatepicker();

    $('form.filter #category').on('change', function () {
        $('form.filter #IsLocal').val($(this).val());
    });

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $("#btnExcel").click(function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        blockUI();
        axios({
            url: resolveClientUrl('report/attendance/exportExcel'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].split(';')[1].split('filename=')[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});