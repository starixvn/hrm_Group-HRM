import 'jquery-ui-bundle/jquery-ui';
import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';
import 'floatthead/dist/jquery.floatThead';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { downloadFile, axiosError, blockUI, initDatepicker, unblockUI, resolveClientUrl, sortTable, floatThead } from '../../shared/common.functions';

const workingCalendarModule = (function ($) {
    function query() {
        blockUI();

        axios({
            url: resolveClientUrl('calendar/WorkingCalendarDetails'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                fromDate: $('#FromDate').val(),
                toDate: $('#ToDate').val(),
                teamIds: $('#Team').multiSelectTreeView('getSelectedItems'),
                jobTitleIds: $('#JobTitle').multiSelectTreeView('getSelectedItems'),
                employeeId: $('#EmployeeId').val(),
                isLocal: $('#IsLocal').val()
            },
            responseType: 'text'
        }).then(response => {
            $('#workingCalendarDetails').html(response.data);

            floatThead('table#data');

            $('.quick-search').on('keyup', function () {
                var value = $(this).val().toLowerCase();
                $("table#data tbody tr").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
                });
            });
            initSortTable();

        }).catch(axiosError).then(() => unblockUI());
    }

    function initModule() {
        const promises = [
            axios({
                url: resolveClientUrl('Team/GetOrgChart'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }),
            axios({
                url: resolveClientUrl('calendar/GetJobTitle'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
        ];

        Promise.all(promises)
            .then(([teamResponse, jobResponse]) => {
                $('#Team').multiSelectTreeView({
                    data: teamResponse.data,
                    isSelectedAll: true,
                    hasSearchBox: true
                });
                $('#JobTitle').multiSelectTreeView({
                    data: jobResponse.data,
                    isSelectedAll: true,
                    hasSearchBox: true
                });

                query();
            }).catch(axiosError);

        initDatepicker();

        $('#btnSearch').on('click', function () {
            query();
        });

        $('#btnExcel').on('click', function () {
            blockUI();
            axios({
                url: resolveClientUrl('ExportCalendar/EmployeeWorkingCalendar'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    fromDate: $('#FromDate').val(),
                    toDate: $('#ToDate').val(),
                    teamIds: $('#Team').multiSelectTreeView('getSelectedItems'),
                    jobTitleIds: $('#JobTitle').multiSelectTreeView('getSelectedItems'),
                    employeeId: $('#EmployeeId').val(),
                    isLocal: $('#IsLocal').val()
                },
                responseType: 'blob'
            }).then(response => {
                const fileName = response.headers['content-disposition'].split(';')[1].split('filename=')[1];
                downloadFile(response.data, fileName);
            }).catch(axiosError).then(() => unblockUI());
        });
    }

    function initSortTable() {
        $('th.col-sorter').on('click', function () {
            sortTable('#data', this);
        });
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    workingCalendarModule.initModule();
});