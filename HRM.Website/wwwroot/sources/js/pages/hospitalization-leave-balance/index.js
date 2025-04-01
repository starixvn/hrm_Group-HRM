import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';

import { axiosError, alert, initDatepicker, resolveClientUrl, floatThead } from '../../shared/common.functions';

const hospitalizationLeaveBalance = (function ($) {
    function getSection() {
        axios({
            method: 'GET',
            url: resolveClientUrl('Section/GetOrgChart'),
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
            + '&employee=' + $('#Employee').val()
            + '&employeeId=' + $('#EmployeeId').val()
            + '&isLocal=' + $('#IsLocal').val());
    }

    function initSubmitFilterEvent() {
        $('.filter form').submit(function (event) {
            event.preventDefault();
            const postToUrl = 'HospitalizationLeaveBalance/index?year=';
            location.href = submitData(postToUrl);
        });
    }

    function initExportExcelEvent() {
        $('#btnExcel').click(function () {
            const postToUrl = 'HospitalizationLeaveBalance/ExportSummaryReport?year=';
            const url = submitData(postToUrl);
            location.href = url;
        });
    }

    function initCheckAllEvent() {
        $('#checkboxAll').on('change', function () {
            const checked = $(this).prop('checked');
            $('table#data tbody input.checkbox-custom').prop('checked', checked);
            $('.checkboxAll').prop('checked', checked);
            if (checked) {
                $('tbody tr').addClass('selected');
            } else {
                $('tbody tr').removeClass('selected');
            }
        });
    }

    function initCheckSingleRowEvent() {
        $('table#data tbody input.checkbox-custom').click(function () {
            const tr = $(this).closest('tr');
            if (tr.hasClass('selected')) {
                tr.removeClass();
            } else {
                tr.addClass('selected');
            }
        });
    }

    function initAdjustmentBtnEvent() {
        $('#btnAdjustment').click(function () {
            const empIds = [];
            $('table#data tbody input.checkbox-custom').each(function (index, item) {
                const checked = $(item).prop('checked');
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
                        const $form = $(this);
                        if (validateInputInsertForm($form)) {
                            removeMsg($form);
                            axios({
                                method: 'POST',
                                url: resolveClientUrl('HospitalizationLeaveBalance/Adjust'),
                                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                                data: {
                                    empIds: $("#EmpIds").val(),
                                    description: $form.find("#Description").val(),
                                    affectedFrom: $form.find("#AffectedFrom").val(),
                                    totalDays: parseInt($form.find("#TotalDays").val(), 10)
                                }
                            }).then(response => {
                                if (response.data.isSuccessful) {
                                    alert('Adjust successfully.');
                                    setInterval(function () {
                                        document.location = resolveClientUrl('HospitalizationLeaveBalance/index');
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
        $('#btnReset').click(function () {
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
                        const $form = $(this).find('form');
                        if (validateInputResetForm($form)) {
                            removeMsg($form);
                            axios({
                                method: 'POST',
                                url: resolveClientUrl('HospitalizationLeaveBalance/Reset'),
                                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                                data: {
                                    resetDays: parseInt($form.find("#ResetDays").val(), 10),
                                    rescription: $form.find("#Description").val()
                                }
                            }).then(response => {
                                if (response.data.isSuccessful) {
                                    alert('Close balance successful.');
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
        let isValid = true;
        const totalDays = parseInt($form.find('input[name="TotalDays"]').val(), 10);
        if (totalDays === null || isNaN(totalDays)) {
            showErrorMsg($form, 'Total days is required.');
            isValid = false;
        }

        return isValid;
    }

    function validateInputResetForm($form) {
        let isValid = true;
        const totalDays = parseInt($form.find('input[name="ResetDays"]').val());
        if (totalDays == null || isNaN(totalDays)) {
            showErrorMsg($form, 'Max Balance is required.');
            isValid = false;
        }

        return isValid;
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
    hospitalizationLeaveBalance.init();
});