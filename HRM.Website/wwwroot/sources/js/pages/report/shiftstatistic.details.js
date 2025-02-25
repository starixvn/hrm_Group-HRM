import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import '../../shared/employee.search.in.team';
import '../../shared/multiselect-tree-view';
import { axiosError, blockUI, unblockUI, floatThead, initDatepicker, resolveClientUrl, downloadFile } from '../../shared/common.functions';

$(document).ready(function () {
    const query = function () {
        blockUI();

        const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
        axios({
            url: resolveClientUrl('ShiftStatisticReport/ShiftStatisticDetails'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                workdate: $('#WorkDate').val(),
                shiftId: $('#ShiftId').val(),
                employeeId: $('#EmployeeId').val(),
                teamIds: teamIds !== '' ? teamIds : ' ',
                isLocal: $('#IsLocal').val(),
                IsShowWholeCompany: $('#IsShowWholeCompany').prop('checked')
            },
            responseType: 'text'
        }).then(response => {
            $('#shiftstatisticdetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

    initDatepicker();

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

        $('#btnSearch').trigger('click');
    }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        const params = {
            workdate: $('#WorkDate').val(),
            shiftId: $('#ShiftId').val(),
            employeeId: $('#EmployeeId').val(),
            teamIds: $('#TeamIds').val(),
            isLocal: $('#IsLocal').val(),
            IsShowWholeCompany: $('#IsShowWholeCompany').prop('checked')
        };
        blockUI();
        axios({
            url: resolveClientUrl('ShiftStatisticReport/ExportExcelStatisticDetails'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: params,
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});