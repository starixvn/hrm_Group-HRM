import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import { axiosError, alert, initDatepicker, resolveClientUrl, floatThead } from '../../shared/common.functions';
import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';

const annualLeaveBalance = (function ($) {
    function getSection() {
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
        }).catch(axiosError);
    }

    function submitData(postToUrl) {
        const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
        return resolveClientUrl(postToUrl + $('#Year').val()
            + '&teamIds=' + (teamIds !== '' ? teamIds : ' ')
            + '&Employee=' + $('#Employee').val()
            + '&EmployeeId=' + $('#EmployeeId').val()
            + '&Category=' + $('#Category').val());
    }

    function initSubmitFilterEvent() {
        $('.filter form').submit(function (e) {
            e.preventDefault();
            var postToUrl = 'AnnualLeaveBalance/index?year=';
            var url = submitData(postToUrl);
            location.href = url;
        });
    }

    function initExportExcelEvent() {
        $('#btnExcel').click(function () {
            var postToUrl = 'ExportLeaveSummary/AnnualLeaveSummary?year=';
            var url = submitData(postToUrl);
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
                                    setInterval(function () {
                                        document.location = resolveClientUrl('annualleavebalance/index');
                                    }, 2000);
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
        getSection();
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
});