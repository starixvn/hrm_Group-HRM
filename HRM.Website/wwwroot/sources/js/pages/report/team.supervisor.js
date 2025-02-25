import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import { axiosError, blockUI, unblockUI, floatThead, resolveClientUrl, downloadFile, sortTable } from '../../shared/common.functions';

(function () {
    function query() {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        blockUI();
        axios({
            url: resolveClientUrl('report/TeamSupervisorDetails'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#TeamSupervisorDetails').html(response.data);
            floatThead('table#data');
            initSortTable();
        }).catch(axiosError).then(() => unblockUI());
    }

    function initSortTable() {
        $('th.col-sorter').on('click', function () {
            sortTable('table#data', this);
            $('table#data tbody tr').each(function (index) {
                let rowIndex = index + 1;
                $(this).find('td:eq(0)').html(rowIndex++);
            });
        });
    }

    $(document).ready(function () {
        axios({
            url: resolveClientUrl('Section/GetOrgChart'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            $('#Team').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
            });

            query();
        }).catch(axiosError);

        $("#btnSubmit").on('click', function () {
            query();
        });

        $('#btnExcel').on('click', function () {
            $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));

            blockUI();
            axios({
                url: resolveClientUrl('ExportSupervisor/TeamSupervisor'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: parse($('form.filter').serialize()),
                responseType: 'blob'
            }).then(response => {
                const fileName = response.headers['content-disposition'].match(/filename=(.+)/)[1];
                downloadFile(response.data, fileName);
            }).catch(axiosError).then(() => unblockUI());
        });
    });
})();