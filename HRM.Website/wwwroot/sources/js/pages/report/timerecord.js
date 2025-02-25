import 'jquery-ui-bundle/jquery-ui';
import '../../../libs/SimplePagination/jquery.simplePagination';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import 'jquery-contextmenu/dist/jquery.contextMenu';
import {
    blockUI, floatThead, unblockUI, confirm, alert,
    initDatepicker, resolveClientUrl, axiosError, downloadFile
} from '../../shared/common.functions';
import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import axios from 'axios';
import { parse } from 'qs';

(function () {
    function initPagination() {
        var currentPage = $('table#data #pagination #CurrentPage').val(),
            itemsOnPage = $('table#data #pagination #ItemsPerPage').val(),
            totalItem = $('table#data #pagination #TotalItem').val();

        currentPage = currentPage ? parseInt(currentPage, 10) : 1;
        itemsOnPage = itemsOnPage ? parseInt(itemsOnPage, 10) : 1;
        totalItem = totalItem ? parseInt(totalItem, 10) : 1;

        $('table#data #pagination #numberlist').pagination({
            items: totalItem,
            itemsOnPage: itemsOnPage,
            currentPage: currentPage,
            displayedPages: 3,
            prevText: '<i class="fa fa-angle-left"></i>',
            nextText: '<i class="fa fa-angle-right"></i>',
            ellipsePageSet: false,
            cssStyle: 'compact-theme',
            onPageClick: function (pageNumber) {
                $('form.filter #CurrentPage').val(pageNumber);
                query();
            }
        });
    }

    function processInOut(inoutType, eventElement) {
        const recordTimeId = parseInt(eventElement.attr('data-id'), 10);
        blockUI();

        axios({
            url: resolveClientUrl('time/ProcessInOut'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: { inoutType, recordTimeId }
        }).then(response => {
            unblockUI();
            if (response.data.IsSuccessful) {
                query();
            } else {
                alert(response.data.Message);
                return;
            }
        });
    }

    function processCommand(command, options) {
        const eventElement = options.$trigger;
        switch (command) {
            case 'in':
                confirm('Are you sure you change to in?', function () {
                    processInOut(0, eventElement);
                });
                break;
            case 'out':
                confirm('Are you sure you change to out?', function () {
                    processInOut(1, eventElement);
                });
                break;
        }
    }

    function bindMenu(element, menu) {
        let item = {};
        if (menu === 'inMenu') {
            item = { 'in': { name: 'In' } };
        } else {
            item = { 'out': { name: 'Out' } };
        }

        $.contextMenu({
            selector: element,
            callback: processCommand,
            trigger: 'left',
            menu: menu,
            items: item
        });
    }

    function query() {
        blockUI();

        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        axios({
            url: resolveClientUrl('Report/TimeRecordDetail'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'text',
            data: $('form.filter').serialize()
        }).then(response => {
            $('#TimeRecordContent').html(response.data);
            $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });

            if ($('table#data #pagination').length > 0) {
                bindMenu('.in-action', 'inMenu');
                bindMenu('.out-action', 'outMenu');

                if ($('table#data #pagination').length > 0) {
                    initPagination();

                    $('#ItemsPerPageOption').on('change', function () {
                        $('form.filter #ItemsPerPage').val($(this).val());
                        $('form.filter #CurrentPage').val(1);
                        query();
                    });
                }
                floatThead('table#data');
            }
        }).then(() => unblockUI()).catch(axiosError);
    }

    $(document).ready(function () {
        axios({
            url: resolveClientUrl('Section/GetOrgChart'),
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

        $('#Employee').change(function () {
            if ($.trim($(this).val()).length === 0) {
                $(this).val('');
            }

            return false;
        });

        $('#btnSubmit').on('click', function () {
            $('form.filter #CurrentPage').val(1);
            query();
        });

        $('form.filter #btnExcel').on('click', function (event) {
            event.preventDefault();

            blockUI();
            axios({
                url: resolveClientUrl('ExportTimeRecord/Export'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: parse($('form.filter').serialize()),
                responseType: 'blob'
            }).then(response => {
                const fileName = response.headers['content-disposition'].match(/filename=(.+)/)[1];
                downloadFile(response.data, fileName);
            }).catch(axiosError).then(() => unblockUI());
        });

        initDatepicker();
    });
})();