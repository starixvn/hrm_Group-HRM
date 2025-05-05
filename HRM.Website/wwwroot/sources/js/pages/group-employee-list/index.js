import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import 'chosen-js/chosen.jquery';
import axios from 'axios';
import tippy from 'tippy.js';
import '../../../libs/SimplePagination/jquery.simplePagination.js';
import '../../shared/multiselect-tree-view.js';
import '../../shared/employee.search.in.team.js';

import { axiosError, blockUI, floatThead, unblockUI, getParam, resolveClientUrl, sortTable, showFullAvatar } from '../../shared/common.functions.js';

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

    function checkDisabledUser() {
        if ($("#disabled").prop("checked")) {
            $('table#data .resignedDate').show();
        } else {
            $('table#data .resignedDate').hide();
        }
    }

    function query() {
        $('#TeamIds').val($('#GroupTeam').multiSelectTreeView('getSelectedItems'));
        $('#CompanyIds').val($('#Company').multiSelectTreeView('getSelectedItems'));
        blockUI();
        axios({
            url: resolveClientUrl('GroupEmployeeList/ListEmployee'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'text',
            data: $('form.filter').serialize()
        }).then(response => {
            unblockUI();
            $('#group-employee-list').html(response.data);
            checkDisabledUser();

            tippy('.avatar-tooltip', {
                maxWidth: 'none',
                animation: 'fade',
                arrow: true,
                placement: 'auto',
                appendTo: document.getElementById('tippy-avatar'),
                allowHTML: true,
                content: '',
                onShow(instance) {
                    const code = $(instance.reference).attr('data-code');
                    fetch(`/employee/avatar/${code}`)
                        .then(response => response.text())
                        .then(avatarUrl => {
                            instance.setContent(`<img src="${avatarUrl}" style="max-width: 100px; max-height: 100px;" />`);
                        });
                }
            });

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

    function exportExcel(object) {
        var url = null,
            controllerPath;

        controllerPath = "ExportEmployee/ExportGroupEmployeeList?";

        $('#TeamIds').val($('#GroupTeam').multiSelectTreeView('getSelectedItems'));
        $('#CompanyIds').val($('#Company').multiSelectTreeView('getSelectedItems'));
        url = resolveClientUrl(controllerPath + $('form.filter').serialize());
        location.href = url;
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
            url: resolveClientUrl('GroupEmployeeList/GetOrgChart'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            $('#GroupTeam').multiSelectTreeView({
                data: response.data.data,
                selectedItems: response.data.teamIds,
                hasSearchBox: true
            });

            axios({
                url: resolveClientUrl('GroupEmployeeList/GetCompanyDropdownList'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).then(companyResponse => {
                $('#Company').multiSelectTreeView({
                    data: companyResponse.data,
                    selectedItems: $('#CompanyIds').val(),
                    hasSearchBox: true
                });
                query();

            }).catch(axiosError);

        }).catch(axiosError);


        $('#Keyword').keyup(function (event) {
            if (event.keyCode === 13) {
                query();
            }
        });

        var q = getParam('query');
        if (q !== null) {
            var decode = decodeURIComponent(q);
            $('#Keyword').val(decode);
        }

        $('#all').on('click', function () {
            $("#IsAll").val(true);
            $("#IsEnable").val(null);
            $("#IsDelete").val(null);
            $('#enabled').prop('checked', false);
            $('#deleted').prop('checked', false);
            $('#disabled').prop('checked', false);
        });

        $('#enabled').on('click', function () {
            $("#IsAll").val(null);
            $("#IsEnable").val(true);
            $("#IsDelete").val(null);
            $('#all').prop('checked', false);
            $('#deleted').prop('checked', false);
            $('#disabled').prop('checked', false);
        });

        $('#disabled').on('click', function () {
            $("#IsAll").val(null);
            $("#IsEnable").val(null);
            $("#IsDelete").val(null);
            $('#all').prop('checked', false);
            $('#deleted').prop('checked', false);
            $('#enabled').prop('checked', false);
        });

        $('#deleted').on('click', function () {
            $("#IsAll").val(null);
            $("#IsDelete").val(true);
            $("#IsEnable").val(null);
            $('#all').prop('checked', false);
            $('#enabled').prop('checked', false);
            $('#disabled').prop('checked', false);
        });

        $('#query').on("click", function (evt) {
            evt.preventDefault();
            $('form.filter #CurrentPage').val('1');
            query();
        });

        $("#exportExcel").on('click', function () {
            exportExcel($(this));
        });
    });
})();