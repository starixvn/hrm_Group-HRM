import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import '../../shared/multiselect-tree-view';
import {
    axiosError, blockUI, unblockUI, initDatepicker,
    resolveClientUrl, downloadFile
} from '../../shared/common.functions';

$(document).ready(function () {
    const query = function () {
        blockUI();

        axios({
            url: resolveClientUrl('ShiftStatisticReport/ShiftStatistic'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                fromDate: $('#FromDate').val(),
                toDate: $('#ToDate').val(),
                TeamIds: $('#TeamIds').val(),
                isLocal: $('#IsLocal').val(),
                IsShowWholeCompany: $('#IsShowWholeCompany').prop('checked')
            },
            responseType: 'text'
        }).then(response => {
            $('#shiftstatisticdetails').html(response.data);
        }).catch(axiosError).then(() => unblockUI());
    };

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

        $('#btnSearch').trigger('click');
    }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        const params = {
            fromDate: $('#FromDate').val(),
            toDate: $('#ToDate').val(),
            teamIds: $('#TeamIds').val(),
            isLocal: $('#IsLocal').val(),
            IsShowWholeCompany: $('#IsShowWholeCompany').prop('checked')
        };

        blockUI();
        axios({
            url: resolveClientUrl('ShiftStatisticReport/ExportExcel'),
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