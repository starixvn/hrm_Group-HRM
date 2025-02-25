import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import moment from 'moment';

import { axiosError, floatThead, confirm, alert, blockUI, unblockUI, initDatepicker, resolveClientUrl } from '../../shared/common.functions';

const postnatalModule = (function ($) {
    const domElement = {
        AddButton: '#add-postnatal',
        Table: '#list-postnatal',
        $dialog: $('#dialog'),
        CreateForm: '#create-form',
        EditForm: '#edit-form'
    };

    function getPostnatal() {
        blockUI();
        axios({
            method: 'GET',
            url: resolveClientUrl('postnatal/getPostnatalGrid?year=' + $('#Year').val()),
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'text'
        }).then(response => {
            $(domElement.Table).html(response.data);
            floatThead('#data');
            initEditEvent();
            initDeleteEvent();
            initToolTip();
        }).then(() => unblockUI()).catch(axiosError);
    }

    function initToolTip() {
        $('.tooltip-tipsy').tipsy({
            html: true,
            gravity: 'e'
        });
    }

    function validateAddPostnatal() {
        let isValid = true,
            errorMsg = '',
            empId = domElement.$dialog.find('#EmployeeId').val(),
            fromDate = domElement.$dialog.find('#FromDate').val(),
            toDate = domElement.$dialog.find('#ToDate').val(),
            workingHours = domElement.$dialog.find('#WorkingHours').val();

        if (!empId || parseInt(empId, 10) <= 0) {
            errorMsg += '<li>Employee is required.</li>';
        }

        if (!moment(toDate).isSameOrAfter(moment(fromDate))) {
            errorMsg += '<li>To Date must be greater than or equal to From Date.</li>';
        }

        if (workingHours === '' || parseInt(workingHours, 10) < 1 || parseInt(workingHours, 10) > 24) {
            errorMsg += '<li>Working hours per day must be an integer and between 1 and 24.</li>';
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
                url: resolveClientUrl('postnatal/create'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'text'
            }).then(response => {
                domElement.$dialog.html(response.data);
                initAddPostnatalDialog();
                initAutoComplete();
                initDatepicker();
            }).catch(axiosError);
        });
    }

    function initAddPostnatalDialog() {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Add Employee Postnatal',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function (e) {
                        removeMsg(domElement.$dialog);
                        if (validateAddPostnatal()) {
                            axios({
                                method: 'POST',
                                url: resolveClientUrl('postnatal/Create'),
                                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                                data: $(domElement.CreateForm).serialize()
                            }).then(response => {
                                if (response.data.IsSuccessful) {
                                    domElement.$dialog.dialog('close');
                                    alert('Create employee postnatal successfully!', getPostnatal);
                                } else {
                                    showErrorMsg(domElement.$dialog, '<li>' + response.data.Message + '</li>');
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
            minWidth: 460,
            dialogClass: 'modal-shadow'
        });
    }

    function removeMsg($dialog) {
        $dialog.find('.validation-summary-valid')
            .hide().removeClass('success').removeClass('validation-summary-errors').find('ul:first').html('');
    }

    function showErrorMsg($dialog, msg) {
        $dialog.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(msg);
    }

    function initEventEditPostnatal(postnatalId) {
        domElement.$dialog.dialog({
            modal: true,
            title: 'Edit Employee Postnatal',
            buttons: [
                {
                    text: 'Submit',
                    class: 'btn btn-primary',
                    click: function () {
                        removeMsg(domElement.$dialog);
                        if (validateAddPostnatal()) {
                            axios({
                                method: 'POST',
                                url: resolveClientUrl('postnatal/Update/' + postnatalId),
                                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                                data: $(domElement.EditForm).serialize()
                            }).then(response => {
                                if (response.data.IsSuccessful) {
                                    domElement.$dialog.dialog('close');
                                    alert('Update employee postnatal successfully!', getPostnatal);
                                } else {
                                    showErrorMsg(domElement.$dialog, '<li>' + response.data.Message + '</li>');
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
            minWidth: 460,
            dialogClass: 'modal-shadow'
        });
    }

    function initAutoComplete() {
        let $selectorName = domElement.$dialog.find('#Employee'),
            $selectorId = domElement.$dialog.find('#EmployeeId');
        $selectorName.autocomplete({
            source: function (request, response) {
                axios({
                    url: resolveClientUrl('employee/GetMemberByGender'),
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    params: {
                        keyword: request.term,
                        gender: 0 //female
                    }
                }).then(res => {
                    response($.map(res.data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')',
                            value: item.Id
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
        $('.edit-postnatal').on('click', function (event) {
            event.preventDefault();

            const postnatalId = $(this).data('id');
            axios({
                url: resolveClientUrl('postnatal/edit/' + postnatalId),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'text'
            }).then(response => {
                domElement.$dialog.html(response.data);
                initEventEditPostnatal(postnatalId);
                initAutoComplete();
                initDatepicker();
            }).catch(axiosError);
        });
    }

    function initDeleteEvent() {
        var $selector = $('.delete-postnatal');
        $selector.on('click', function (event) {
            event.preventDefault();
            const postnatalId = $(this).data('id');
            confirm('Are you sure you want to remove this postnatal?', function () {
                axios({
                    method: 'POST',
                    url: resolveClientUrl('postnatal/delete/' + postnatalId),
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                }).then(response => {
                    if (response.data.IsSuccessful) {
                        alert('Remove employee postnatal successfully!', getPostnatal);
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
            getPostnatal();
        });
    }

    function initModule() {
        initSubmitFiler();
        getPostnatal();
        initAddEvent();
    }

    return {
        initModule
    };
})(jQuery);

$(document).ready(function () {
    postnatalModule.initModule();
});