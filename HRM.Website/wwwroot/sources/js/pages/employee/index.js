import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import tippy from 'tippy.js';
import '../../shared/multiselect-tree-view';
import '../../../../sources/libs/SimplePagination/jquery.simplePagination.js';
import '../../shared/employee.search.in.team';

import { axiosError, blockUI, floatThead, unblockUI, getParam, resolveClientUrl, sortTable, showFullAvatar } from '../../shared/common.functions';

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

    function query() {
        blockUI();
        axios({
            url: resolveClientUrl('employee/ContactList'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'text',
            data: $('form.filter').serialize(),
        }).then(response => {
            unblockUI();
            $('#contact-list').html(response.data);
            initTooltip();
            if ($('table#data #pagination').length > 0) {
                initPagination();
                $('#ItemsPerPageOption').on('change', function () {
                    $('form.filter #ItemsPerPage').val($(this).val());
                    $('form.filter #CurrentPage').val(1);
                    query();
                });
                $('.fullSizeAvatar').on('click', function () {
                    const codeId = $(this).data('code');
                    const nickname = $(this).data('nickname');
                    showFullAvatar(codeId, nickname);
                });
            }

            floatThead('table#data');
            initSortTable();
        }).catch(axiosError);
    }

    function initTooltip() {
        tippy('.avatar-tooltip', {
            maxWidth: 'none',
            animation: 'fade',
            arrow: true,
            placement: 'auto',
            appendTo: document.getElementById('tippy-avatar'),
            allowHTML: true,
            content: (reference) => {
                const path = resolveClientUrl('employee/avatar/' + $(reference).attr('data-code'));
                return '<img src="' + path + '" />';
            }
        });
    }

    function initSortTable() {
        $('th.col-sorter').on('click', function () {
            sortTable('#data', this);
            $("tbody tr").each(function (index) {
                var initIndex = index + 1;
                $(this).find("td:eq(0)").html(initIndex++);
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
                isSelectedAll: true,
                hasSearchBox: true
            });
        }).catch(axiosError);

        $('#category').on('change', function () {
            switch ($(this).val()) {
                case '0':
                    $('#IsLocal').val(false);
                    break;
                case '1':
                    $('#IsLocal').val(true);
                    break;
                default:
                    $('#IsLocal').val(null);
                    break;
            }
        });

        var q = getParam('query');
        if (q !== null) {
            var decode = decodeURIComponent(q);
            $('#Keyword').val(decode);
        }

        $('#Keyword').keydown(function (event) {
            // enter
            if (event.keyCode === 13) {
                query();
            }
        });

        query();

        $('form').on("submit", function (evt) {
            evt.preventDefault();
            $('form.filter #CurrentPage').val('1');
            $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
            query();
        });
    });
})();