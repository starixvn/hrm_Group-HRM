import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import { axiosError, floatThead, blockUI, unblockUI, downloadFile, resolveClientUrl, sortTable } from '../../shared/common.functions';

$(document).ready(function () {
    floatThead('table#data');

    $('th.col-sorter').on('click', function () {
        sortTable('table#data', this);
        $("table#data tbody tr").each(function (index) {
            var initIndex = index + 1;
            $(this).find("td:eq(0)").html(initIndex++);
        });
    });

    $('#btnExcel').on('click', function () {
        blockUI();
        axios({
            url: resolveClientUrl('ManpowerReport/ExportManpowerCategory'),
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