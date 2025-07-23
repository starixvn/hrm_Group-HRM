import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery.are-you-sure/jquery.are-you-sure';
import toastr from 'toastr';
import axios from 'axios';
import moment from 'moment';

import {
    axiosError, initDatepicker, showValidationSummaryErrors, hideValidationSummaryErrors, toastOptions,
    blockUI, unblockUI, resolveClientUrl
} from '../../../shared/common.functions';
import { RequestType, RoleSystem } from '../../../shared/constants';

$(document).ready(function () {
    let domElements = {
        $employeeId: $('#EmployeeId'),
        $lateInTypeId: $('#RequestType_3'),
        $earlyOutTypeId: $('#RequestType_4'),
        $onDate: $('#OnDate'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $minute: $('#Minute'),
        $remark: $('#Remark'),
        $actualLateIn: $('#actualLateIn'),
        $lateInHours: $('#lateInHours'),
        $actualEarlyOut: $('#actualEarlyOut'),
        $earlyOutHours: $('#earlyOutHours'),
        $btnSubmit: $('#btnSubmit')
    };
    let getDataAfterChangeEmployee = function () {
        const employeeId = domElements.$employeeId.val();
        const promises = [
            axios({
                url: resolveClientUrl('request/GetVerifiers'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.LateIn
                }
            }),
            axios({
                url: resolveClientUrl('request/GetApprovers'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.LateIn
                }
            })
        ];

        Promise.all(promises)
            .then(([verifierResponse, approverResponse]) => {
                const verifiers = verifierResponse.data,
                    approvers = approverResponse.data;
                let options = '';
                for (let i = 0; i < verifiers.length; i++) {
                    options += "<option value='" + verifiers[i].ownerId + "'>" + verifiers[i].nickName + " (" + verifiers[i].fullName + ")</option>";
                }
                domElements.$verifierId.html(options);

                options = '';
                for (var i = 0; i < approvers.length; i++) {
                    options += "<option value='" + approvers[i].ownerId + "'>" + approvers[i].nickName + " (" + approvers[i].fullName + ")</option>";
                }
                domElements.$approverId.html(options);
            }).catch(axiosError);
    };
    let getDisabledDates = function (callback) {
        const requestTypeId = domElements.$lateInTypeId.prop('checked') ? RequestType.LateIn : RequestType.EarlyOut;
        axios({
            url: resolveClientUrl('lateinearlyout/GetDisabledDate'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                requestTypeId: requestTypeId,
                employeeId: domElements.$employeeId.val(),
                fromDate: domElements.$onDate.attr('mindate'),
                toDate: domElements.$onDate.attr('maxdate')
            }
        }).then(response => {
            const totalItems = response.data.length;
            let disabledDate = '';
            for (let i = 0; i < totalItems; i++) {
                disabledDate = disabledDate.length > 0
                    ? disabledDate + ',' + moment(response.data[i]).format('YYYY/MM/DD')
                    : moment(response.data[i]).format('YYYY/MM/DD');
            }

            domElements.$onDate.attr('disabledDate', disabledDate);
            if (callback !== undefined && typeof callback === 'function') {
                callback();
            }
        }).catch(axiosError);
    };
    let showHideActualLateInEarlyOut = function () {
        if (domElements.$lateInTypeId.prop('checked')) {
            domElements.$actualLateIn.show();
            domElements.$actualEarlyOut.hide();
        } else {
            domElements.$actualLateIn.hide();
            domElements.$actualEarlyOut.show();
        }
    };
    let getActualLateInEarlyOut = function () {
        axios({
            url: resolveClientUrl('lateinearlyout/GetActualLateInEarlyOut'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                employeeId: domElements.$employeeId.val(),
                workDate: domElements.$onDate.val()
            }
        }).then(response => {
            const data = response.data;
            if (data.lateIn > 0) {
                domElements.$lateInHours.html(data.lateIn + "'");
            } else {
                domElements.$lateInHours.html('N/A');
            }

            if (data.earlyOut > 0) {
                domElements.$earlyOutHours.html(data.earlyOut + "'");
            } else {
                domElements.$earlyOutHours.html('N/A');
            }
        }).catch(axiosError);
    };
    let validateForm = function () {
        let isValid = true,
            errorMessages = [];
        const employeeId = parseInt(domElements.$employeeId.val(), 10),
            minute = parseInt(domElements.$minute.val(), 10),
            approverId = domElements.$approverId.val(),
            verifierId = domElements.$verifierId.val();
        let files = document.getElementById('file').files,
            allowedExtensions = /(\.jpg|\.png|\.jpeg|\.pdf|\.doc|\.docx|\.xls|\.xlsx|\.txt)$/i;

        if (employeeId === 0) {
            errorMessages.push('Employee field is required.');
        }

        if (!$('#AutoApproveRequest').prop('checked')) {
            if (verifierId === null || verifierId === '') {
                errorMessages.push('Verified by field is required.');
            }
            if (approverId === null || approverId === '') {
                errorMessages.push('Approved by field is required.');
            }
        }

        if (minute <= 0 || minute > 60) {
            errorMessages.push('Minute field must be between 1 and 60.');
        }

        if ($.trim(domElements.$remark.val()).length === 0) {
            errorMessages.push('Remark field is required.');
        }

        for (var i = 0; i < files.length; i++) {
            if (files[i].size / 1024 > 5120) {
                errorMessages.push('Attach file (' + files[i].name + ') is to large to upload!. Maximum = 5120kb');
            }
            if (!allowedExtensions.exec(files[i].name)) {
                errorMessages.push('File type of ' + files[i].name + ' incorrect!');
            }
        }

        if (errorMessages.length > 0) {
            showValidationSummaryErrors(errorMessages);
            isValid = false;
        }

        return isValid;
    };

    getDisabledDates(initDatepicker);
    getActualLateInEarlyOut();
    showHideActualLateInEarlyOut();

    $('#Employee').autocomplete({
        source: function (request, response) {
            axios({
                url: resolveClientUrl('request/GetEmployeesByActingUser'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    role: RoleSystem.OnBehalfRequest,
                    requestType: RequestType.LateIn,
                    query: request.term
                }
            }).then(rs => {
                response($.map(rs.data, function (item) {
                    return {
                        label: item.nickName + ' (' + item.fullName + ')',
                        value: item.ownerId,
                        autoApproveRequest: item.autoApproveRequest
                    };
                }));
            }).catch(axiosError);
        },
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
            $('#AutoApproveRequest').prop('checked', ui.item.autoApproveRequest);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
            $('#AutoApproveRequest').prop('checked', ui.item.autoApproveRequest);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#Employee').val(ui.item.label);
                $('#EmployeeId').val(ui.item.value);
                $('#AutoApproveRequest').prop('checked', ui.item.autoApproveRequest);
                getDataAfterChangeEmployee();
                getDisabledDates();
                getActualLateInEarlyOut();
            } else {
                $('#EmployeeId').val('0');
            }
        }
    });

    domElements.$lateInTypeId.on('change', function () {
        getDisabledDates();
        getActualLateInEarlyOut();
        showHideActualLateInEarlyOut();
    });

    domElements.$earlyOutTypeId.on('change', function () {
        getDisabledDates();
        getActualLateInEarlyOut();
        showHideActualLateInEarlyOut();
    });

    domElements.$onDate.on('change', function () {
        getActualLateInEarlyOut();
    });

    domElements.$btnSubmit.on('click', function () {
        hideValidationSummaryErrors(false);
        const typeId = domElements.$lateInTypeId.prop('checked') ? RequestType.LateIn : RequestType.EarlyOut;
        const model = {
            employeeId: domElements.$employeeId.val(),
            requestType: typeId,
            onDate: domElements.$onDate.val(),
            ReasonId: $('#reasonId').val(),
            minute: domElements.$minute.val(),
            remark: $.trim(domElements.$remark.val()),
            verifierId: domElements.$verifierId.val(),
            approverId: domElements.$approverId.val()
        };

        const isValid = validateForm();
        if (isValid) {
            domElements.$btnSubmit.prop('disabled', true);
            blockUI();

            var files = document.getElementById('file').files;
            var fd = new FormData();
            for (var i = 0; i < files.length; i++) {
                fd.append('files', files[i]);
            }
            fd.append('requestModel', JSON.stringify(model));
            var xhr = new XMLHttpRequest();
            xhr.addEventListener('load', uploadComplete, false);
            xhr.open('POST', resolveClientUrl('lateinearlyout/Create'));
            xhr.send(fd);
        }
    });

    function uploadComplete(evt) {
        domElements.$btnSubmit.prop('disabled', false);
        unblockUI();

        const data = JSON.parse(evt.target.response);
        if (data.isSuccessful) {
            $('form').removeClass('dirty');
            toastr.success('Create successfully!', '', toastOptions());
            setTimeout(function () { location.href = resolveClientUrl('lateinearlyout/employeeview'); }, 3000);
        } else {
            toastr.error(data.message, '', toastOptions());
        }
    }

    $('form').areYouSure();
});