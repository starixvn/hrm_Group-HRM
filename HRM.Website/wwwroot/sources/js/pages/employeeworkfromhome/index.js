import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import moment from 'moment';
import axios from 'axios';

import '../../shared/multiselect-tree-view';

import {
    axiosError, floatThead, confirm, alert,
    blockUI, unblockUI, initDatepicker, resolveClientUrl
} from '../../shared/common.functions';

const tempFile = [];

const workFromHomeModule = (function ($) {
    const domElement = {
        AddButton: '#add-workfromhome',
        Table: '#list-workfromhome',
        $dialog: $('#dialog'),
        CreateForm: '#create-form',
        EditForm: '#edit-form'
    };

    function getEmployeeWorkFromHome() {
        blockUI();
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        axios({
            url: resolveClientUrl('EmployeeWorkFromHome/GetEmployeeWorkFromHomeGrid'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                fromDate: $("#FromDateFilter").val(),
                toDate: $("#ToDateFilter").val(),
                teamIds: $('#TeamIds').val(),
                isLocal: $('#IsLocal').val()
            },
            responseType: 'text'
        }).then(response => {
            $(domElement.Table).html(response.data);
            floatThead('#data');
            initEditEvent();
            initDeleteEvent();
            initToolTip();
        }).catch(axiosError).then(() => unblockUI());
    }

    function initToolTip() {
        $('.tooltip-tipsy').tipsy({
            html: true,
            gravity: 'e'
        });
    }

    function validateAddEmployeeWorkFromHome() {
        let isValid = true,
            errorMsg = '',
            empId = domElement.$dialog.find('#EmployeeId').val(),
            fromDate = domElement.$dialog.find('#FromDate').val(),
            toDate = domElement.$dialog.find('#ToDate').val(),
            type = domElement.$dialog.find('input[name="Type"]:checked').val(),
            workingHours = domElement.$dialog.find('#WorkingHours').val();
        let files = document.getElementById('file_emp_wfh').files,
            allowedExtensions = /(\.jpg|\.png|\.jpeg|\.pdf|\.doc|\.docx|\.xls|\.xlsx|\.txt)$/i;

        if (!empId || parseInt(empId, 10) <= 0) {
            errorMsg += '<li>Employee is required.</li>';
        }

        if (type === 'Fixed-period') {
            if (!toDate) {
                errorMsg += '<li>To Date is required for Fixed-period.</li>';
            } else if (!moment(toDate).isSameOrAfter(moment(fromDate))) {
                errorMsg += '<li>To Date must be greater than or equal to From Date.</li>';
            }
        }

        if (workingHours === '' || parseInt(workingHours, 10) < 1 || parseInt(workingHours, 10) > 24) {
            errorMsg += '<li>Working hours per day must be an integer and between 1 and 24.</li>';
        }

        for (var i = 0; i < files.length; i++) {
            if (files[i].size / 1024 > 5120) {
                errorMsg += `<li>Attach file ('${files[i].name}') is to large to upload!. Maximum = 5120kb</li>`;
            }
            if (!allowedExtensions.exec(files[i].name)) {
                errorMsg += `<li>File type of '${files[i].name}' incorrect!</li>`;
            }
        }

        if (errorMsg.length > 0) {
            showErrorMsg(domElement.$dialog, errorMsg);
            isValid = false;
        }

        return isValid;
    }

    function initAddEvent() {
        $(domElement.AddButton).on('click', function (e) {
            e.preventDefault();
            axios({
                url: resolveClientUrl('EmployeeWorkFromHome/create'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'text'
            }).then(response => {
                domElement.$dialog.html(response.data);
                initAddEmployeeWorkFromHomeDialog();
                initAutoComplete();
                initDatepicker();
            }).catch(axiosError);
        });
    }

    function initTypeRadioButtons() {
        domElement.$dialog.find('input[name="Type"]').on('change', function () {
            const type = $(this).val();
            const $toDateInput = domElement.$dialog.find('#ToDate');
            const $toDateIcon = $toDateInput.closest('.calendar-form').find('.icon-calendar');
            
            if (type === 'Permanent') {
                $toDateInput.prop('disabled', true);
                $toDateInput.addClass('disabled');
                $toDateInput.val('');
                $toDateInput.datepicker('disable');
                $toDateIcon.addClass('disabled').off('click');
            } else {
                $toDateInput.prop('disabled', false);
                $toDateInput.removeClass('disabled');
                $toDateInput.datepicker('enable');
                $toDateIcon.removeClass('disabled');
                // Re-initialize icon click handler
                $toDateIcon.off('click').on('click', function () {
                    if ($toDateInput.datepicker('widget').is(':visible')) {
                        $toDateInput.datepicker('hide');
                    } else {
                        $toDateInput.datepicker('show');
                    }
                });
            }
        });
        
        // Initialize on load
        const selectedType = domElement.$dialog.find('input[name="Type"]:checked').val();
        if (selectedType === 'Permanent') {
            const $toDateInput = domElement.$dialog.find('#ToDate');
            const $toDateIcon = $toDateInput.closest('.calendar-form').find('.icon-calendar');
            $toDateInput.prop('disabled', true).addClass('disabled').val('');
            $toDateInput.datepicker('disable');
            $toDateIcon.addClass('disabled').off('click');
        }
    }

    function initAddEmployeeWorkFromHomeDialog() {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Add Employee Work From Home',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function (e) {
                        removeMsg(domElement.$dialog);
                        if (validateAddEmployeeWorkFromHome()) {
                            const type = domElement.$dialog.find('input[name="Type"]:checked').val();
                            const model = {
                                employeeId: $("#EmployeeId").val(),
                                fromDate: $("#FromDate").val(),
                                toDate: type === 'Permanent' ? null : $("#ToDate").val(),
                                remark: $("#Remark").val()
                            }

                            var files = document.getElementById('file_emp_wfh').files;
                            var fd = new FormData();
                            for (var i = 0; i < files.length; i++) {
                                fd.append('files', files[i]);
                            }
                            fd.append('requestModel', JSON.stringify(model));
                            var xhr = new XMLHttpRequest();
                            xhr.onreadystatechange = function () {
                                if (xhr.readyState === 4 && xhr.status === 200) {
                                    var response = JSON.parse(xhr.responseText);
                                    if (response.IsSuccessful) {
                                        domElement.$dialog.dialog('close');
                                        alert('Create employee work from home successfully!', getEmployeeWorkFromHome);
                                    } else {
                                        showErrorMsg(domElement.$dialog, '<li>' + response.Message + '</li>');
                                    }
                                }
                            }
                            xhr.open('POST', resolveClientUrl('EmployeeWorkFromHome/Create'));
                            xhr.send(fd);
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
            minWidth: 460,
            dialogClass: 'modal-shadow'
        });
        initTypeRadioButtons();
    }

    function removeMsg($dialog) {
        $dialog.find('.validation-summary-valid')
            .hide().removeClass('success').removeClass('validation-summary-errors').find('ul:first').html("");
    }

    function showErrorMsg($dialog, msg) {
        $dialog.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(msg);
    }

    function initEventEditWorkFromHome(employeeworkfromhomeId) {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Edit Employee Work From Home',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        removeMsg(domElement.$dialog);
                        if (validateAddEmployeeWorkFromHome()) {
                            const type = domElement.$dialog.find('input[name="Type"]:checked').val();
                            const model = {
                                employeeId: $("#EmployeeId").val(),
                                fromDate: $("#FromDate").val(),
                                toDate: type === 'Permanent' ? null : $("#ToDate").val(),
                                remark: $("#Remark").val(),
                                employeeWorkFromHomeId: $("#EmployeeWorkFromHomeId").val()
                            }

                            var files = document.getElementById('file_emp_wfh').files;
                            var fd = new FormData();
                            for (var i = 0; i < files.length; i++) {
                                fd.append('files', files[i]);
                            }
                            fd.append('requestModel', JSON.stringify(model));
                            fd.append('filesRemove', JSON.stringify(tempFile));
                            var xhr = new XMLHttpRequest();
                            xhr.onreadystatechange = function () {
                                if (xhr.readyState === 4 && xhr.status === 200) {
                                    var response = JSON.parse(xhr.responseText);
                                    if (response.IsSuccessful) {
                                        domElement.$dialog.dialog('close');
                                        alert('Update employee work from home successfully!', getEmployeeWorkFromHome);
                                    } else {
                                        showErrorMsg(domElement.$dialog, '<li>' + response.Message + '</li>');
                                    }
                                }
                            }
                            xhr.open('POST', resolveClientUrl('EmployeeWorkFromHome/Edit'));
                            xhr.send(fd);
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
            minWidth: 460,
            dialogClass: 'modal-shadow'
        });
        initTypeRadioButtons();
    }

    function initAutoComplete() {
        let $selectorName = domElement.$dialog.find('#Employee'),
            $selectorId = domElement.$dialog.find('#EmployeeId');
        $selectorName.autocomplete({
            source: function (request, response) {
                axios({
                    url: resolveClientUrl('employee/getemployees'),
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    params: {
                        query: request.term,
                        enable: true,
                        isDelete: false
                    }
                }).then(res => {
                    response($.map(res.data, function (item) {
                        return {
                            label: item.nickName + ' (' + item.fullName + ')',
                            value: item.id
                        };
                    }));
                }).catch(axiosError);
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $selectorName.val(ui.item.label);
                $selectorId.val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $selectorName.val(ui.item.label);
                $selectorId.val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $selectorName.val(ui.item.label);
                    $selectorId.val(ui.item.value);
                } else {
                    if ($selectorName.val() === '') {
                        $selectorId.val('');
                    }
                }
            }
        });
    }

    function initEditEvent() {
        $('.edit-employeeworkfromhome').on('click', function (event) {
            event.preventDefault();

            const employeeworkfromhomeId = $(this).data('id');
            axios({
                url: resolveClientUrl('EmployeeWorkFromHome/Edit/' + employeeworkfromhomeId),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'text'
            }).then(response => {
                domElement.$dialog.html(response.data);
                initEventEditWorkFromHome(employeeworkfromhomeId);
                initAutoComplete();
                initDatepicker();
                initTypeRadioButtons();
            }).catch(axiosError);
        });
    }

    function initDeleteEvent() {
        var $selector = $('.delete-employeeworkfromhome');
        $selector.on('click', function (event) {
            event.preventDefault();
            const employeeworkfromhomeId = $(this).data('id');
            confirm('Are you sure you want to remove this employee work from home?', function () {
                axios({
                    method: 'POST',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    url: resolveClientUrl('EmployeeWorkFromHome/Delete/' + employeeworkfromhomeId),
                }).then(response => {
                    if (response.data.IsSuccessful) {
                        alert('Remove employee work from home successfully!', getEmployeeWorkFromHome);
                    } else {
                        alert(response.data.Message);
                    }
                }).catch(axiosError);
            });
        });
    }

    function initSubmitFiler() {
        $('form').on('submit', function (e) {
            e.preventDefault();
            getEmployeeWorkFromHome();
        });
    }

    function initExportExcel() {
        $('#btnExcel').on('click', function (e) {
            e.preventDefault();
            var postToUrl = 'ExportEmployeeWorkFromHome/EmployeeWorkFromHome?';

            $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
            var url = resolveClientUrl(postToUrl +
                'teamIds=' +
                $('#TeamIds').val() +
                '&fromDate=' +
                $("#FromDateFilter").val() +
                '&toDate=' +
                $("#ToDateFilter").val() +
                '&isLocal=' +
                $('#IsLocal').val()
            );
            location.href = url;
        });
    }

    function initModule() {
        initSubmitFiler();
        initAddEvent();
        initDatepicker();
        initExportExcel();

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
            getEmployeeWorkFromHome();
        }).catch(axiosError);
    }

    return {
        initModule
    };
})(jQuery);

$(document).on('click', '.delete-file', function () {
    const fileName = $(this).attr("data-id");
    var classDelete = document.getElementById(fileName);
    classDelete.remove();
    tempFile.push(fileName);
});

$(document).ready(function () {
    workFromHomeModule.initModule();
});