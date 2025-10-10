import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import { axiosError, alert, initDatepicker, resolveClientUrl, floatThead, blockUI, unblockUI } from '../../shared/common.functions';
import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';

const annualLeaveBalance = (function ($) {
    function getOrgChart() {
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

            loadTableData();
            
        }).catch(axiosError);
    }

    function submitData(postToUrl) {
        const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
        const year = $('#Year').val() || 0;
        $('#EmployeeStatusIds').val($('#EmployeeStatus').multiSelectTreeView('getSelectedItems'));
        const statusIds = $('#EmployeeStatusIds').val().split(',');
        return resolveClientUrl(postToUrl + year
            + '&teamIds=' + (teamIds !== '' ? teamIds : ' ')
            + '&Employee=' + $('#Employee').val()
            + '&EmployeeId=' + $('#EmployeeId').val()
            + '&EmployeeStatusIds=' + statusIds
            + '&Category=' + $('#Category').val());
    }

    function loadTableData() {
        var url = submitData('AnnualLeaveBalance/index?year=');
        blockUI();
        axios({
            url: url,
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            if (response.data) {
                var $response = $(response.data);
                var $newTable = $response.find('table#data');
                if ($newTable.length > 0) {
                    $('table#data').replaceWith($newTable);
                    floatThead('table#data');
                    $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });
                    initCheckAllEvent();
                    initCheckSingleRowEvent();
                }
            }
        }).catch(axiosError).then(() => unblockUI());
    }

    function initSubmitFilterEvent() {
        $('.filter form').submit(function (e) {
            e.preventDefault();
            loadTableData();
        });
    }

    function initExportExcelEvent() {
        $('#btnExcel').click(function () {
            if(window.IsJoinedDateRule == 'True') {
                var postToUrl = 'ExportLeaveSummary/AnnualLeaveSummaryForJoinedDate?year=';
                var url = submitData(postToUrl);
            } else {
                var postToUrl = 'ExportLeaveSummary/AnnualLeaveSummary?year=';
                var url = submitData(postToUrl);
            }
            location.href = url;
        });
    }

    function initCheckAllEvent() {
        $('#checkuncheckall').on('change', function () {
            const checked = $(this).prop('checked');
            $('.aba').prop('checked', checked);
            $('.checkuncheckall').prop('checked', checked);
            if (checked) {
                $('tbody tr').addClass('selected');
            } else {
                $('tbody tr').removeClass('selected');
            }
        });
    }

    function initCheckSingleRowEvent() {
        $('.aba').click(function () {
            var tr = $(this).closest('tr');
            if (tr.hasClass('selected')) {
                tr.removeClass();
            }
            else {
                tr.addClass('selected');
            }
        });
    }

    function initAdjustmentBtnEvent() {
        $('#addadj').click(function () {
            const empIds = [];
            $('.aba').each(function (index, item) {
                var checked = $(item).prop('checked');
                if (checked) {
                    empIds.push($(item).attr('data-id'));
                }
            });
            $('#EmpIds').val(empIds);

            if (empIds.length > 0) {
                handleInsertForm();
            } else {
                alert('Please choose at least one employee to continue!');
            }
        });
    }

    function handleInsertForm() {
        $('#insertform').dialog({
            modal: true,
            width: 460,
            open: function () {
                $('#TotalDays').val('');
            },
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        let $form = $(this);
                        if (validateInputInsertForm($form)) {
                            removeMsg($form);
                            var model = {
                                empIds: $("#EmpIds").val(),
                                description: $form.find("#Description").val(),
                                affectedFrom: $form.find("#AffectedFrom").val(),
                                totalDays: parseInt($form.find("#TotalDays").val(), 10)
                            };
                            axios({
                                method: 'POST',
                                url: resolveClientUrl('AnnualLeaveBalance/Adjust'),
                                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                                data: model
                            }).then(response => {
                                if (response.data.isSuccessful) {
                                    alert('Adjust successfully.');
                                    setTimeout(function () {
                                        loadTableData();
                                    }, 3000);
                                } else {
                                    showErrorMsg($form, response.data.message);
                                }
                            }).catch(axiosError);
                        }
                    }
                },
                {
                    text: 'Cancel',
                    class: 'btn btn-default',
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ],
            dialogClass: 'modal-shadow'
        });
    }

    function initCloseBalanceButtonEvent() {
        $('#resetblc').click(function () {
            $('#resetform').dialog({
                modal: true,
                width: 410,
                open: function () {
                    removeMsg($(this));
                },
                buttons: [{
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        var $form = $(this).find('form');
                        if (validateInputResetForm($form)) {
                            removeMsg($form);
                            axios({
                                url: resolveClientUrl('AnnualLeaveBalance/Reset'),
                                method: 'POST',
                                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                                data: $form.serialize()
                            }).then(response => {
                                if (response.data.isSuccessful) {
                                    alert('Close balance successful!');
                                    $('#resetform').dialog('close');
                                } else {
                                    alert(response.data.message);
                                }
                            }).catch(axiosError);
                        }
                    }
                },
                {
                    text: 'Cancel',
                    class: 'btn btn-default',
                    click: function () {
                        $(this).dialog('close');
                    }
                }],
                dialogClass: 'modal-shadow'
            });
        });
    }

    function validateInputInsertForm($form) {
        const totalDays = parseInt($form.find('input[name="TotalDays"]').val(), 10);
        if (totalDays === null || totalDays === '' || isNaN(totalDays)) {
            showErrorMsg($form, 'Total days is required.');
            return false;
        }

        return true;
    }

    function validateInputResetForm($form) {
        var check = true;
        var errorString = '';
        var totalDays = parseInt($form.find('input[name="MinBalance"]').val());

        if (totalDays == null || totalDays === "" || isNaN(totalDays)) {
            errorString = "Max Balance is required.";
        }

        if (errorString.length > 0) {
            showErrorMsg($form, errorString);
            return false;
        }

        return check;
    }

    function removeMsg($dialog) {
        $dialog.find('.validation-summary-valid')
            .hide().removeClass('success').removeClass('validation-summary-errors').find('ul:first').html("");
    }

    function showErrorMsg($dialog, msg) {
        removeMsg($dialog);
        $dialog.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(msg);
    }

    function init() {
        getOrgChart();
        initDatepicker();
        initSubmitFilterEvent();
        initExportExcelEvent();
        initCheckAllEvent();
        initCheckSingleRowEvent();
        initAdjustmentBtnEvent();
        initCloseBalanceButtonEvent();
        floatThead('table#data');
        $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });
    }

    return {
        init
    };
})(jQuery);

$(document).ready(function () {
    annualLeaveBalance.init();
    const statusIds = $('#EmployeeStatusIds').val().split(',');
   $('#EmployeeStatus').multiSelectTreeView({
       data: [
           { id: 1, text: 'Enable', enable: true, checked: statusIds.indexOf('1') !== -1, isLeaf: true },
           { id: 2, text: 'Disabled', enable: true, checked: statusIds.indexOf('2') !== -1, isLeaf: true },
           { id: 3, text: 'Deleted', enable: true, checked: statusIds.indexOf('3') !== -1, isLeaf: true }
       ]
   });
});