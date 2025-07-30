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
        $checkInTypeId: $('#RequestTypeId_1'),
        $checkOutTypeId: $('#RequestTypeId_2'),
        $attendantDate: $('#AttendantDate'),
        $forDate: $('#ForDate'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $hour: $('#Hour'),
        $minute: $('#Minute'),
        $remark: $('#Remark'),
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
                    requestType: RequestType.CheckIn
                }
            }),
            axios({
                url: resolveClientUrl('request/GetApprovers'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.CheckIn
                }
            })
        ];

        Promise.all(promises)
            .then(([verifierResponse, approverResponse]) => {
                const verifiers = verifierResponse.data,
                    approvers = approverResponse.data;
                let options = '';
                if ($('#AutoApproveRequest').prop('checked')) {
                    options += '<option>--Select a Verifier--</option>';
                }
                for (let i = 0; i < verifiers.length; i++) {
                    options += "<option value='" + verifiers[i].ownerId + "'>" + verifiers[i].nickName + " (" + verifiers[i].fullName + ")</option>";
                }
                domElements.$verifierId.html(options);

                options = '';
                if ($('#AutoApproveRequest').prop('checked')) {
                    options += '<option>--Select an Approver--</option>';
                }
                for (var i = 0; i < approvers.length; i++) {
                    options += "<option value='" + approvers[i].ownerId + "'>" + approvers[i].nickName + " (" + approvers[i].fullName + ")</option>";
                }
                domElements.$approverId.html(options);
            }).catch(axiosError);
    };
    let getDisabledDates = function (callback) {
        blockUI();
        const requestTypeId = domElements.$checkInTypeId.prop('checked') ? RequestType.CheckIn : RequestType.CheckOut;
        axios({
            url: resolveClientUrl('checkinout/GetDisabledDate'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                requestTypeId: requestTypeId,
                employeeId: domElements.$employeeId.val(),
                fromDate: domElements.$attendantDate.attr('mindate'),
                toDate: domElements.$attendantDate.attr('maxdate')
            }
        }).then(response => {
            const totalItems = response.data.length;
            let disabledDate = '';
            for (let i = 0; i < totalItems; i++) {
                disabledDate = disabledDate.length > 0
                    ? disabledDate + ',' + moment(response.data[i]).format('YYYY/MM/DD')
                    : moment(response.data[i]).format('YYYY/MM/DD');
            }

            domElements.$attendantDate.attr('disabledDate', disabledDate);
            domElements.$forDate.attr('disabledDate', disabledDate);

            if (callback !== undefined && typeof callback === 'function') {
                callback();
            }
        }).catch(axiosError).then(() => unblockUI());
    };
    let bindingHourAndMinute = function () {
        if (domElements.$checkInTypeId.prop('checked')) {
            let intime = $('#InTime').val().trim();
            domElements.$hour.val(intime.substring(0, 2));
            domElements.$minute.val(intime.substring(3, 5));
        } else {
            let outtime = $('#OutTime').val().trim();
            domElements.$hour.val(outtime.substring(0, 2));
            domElements.$minute.val(outtime.substring(3, 5));
        }
    };
    let checkInOrOutInNextDay = function (isInOutAfterDay) {
        const attendantDate = new Date(domElements.$attendantDate.val());
        if (isInOutAfterDay === 'true') {
            const requestDate = new Date(attendantDate.getTime() + 86400000);
            domElements.$forDate.val(moment(requestDate).format('DD MMM, YYYY'));
        } else {
            domElements.$forDate.val(moment(attendantDate).format('DD MMM, YYYY'));
        }
    };
    let checkCrossDay = function () {
        const isInAfterDay = $('#IsInAfterDay').val(),
            isOutAfterDay = $('#IsOutAfterDay').val();

        if (domElements.$checkInTypeId.prop('checked')) {
            checkInOrOutInNextDay(isInAfterDay);
        }

        if (domElements.$checkOutTypeId.prop('checked')) {
            checkInOrOutInNextDay(isOutAfterDay);
        }
    };
    let validateForm = function () {
        let isValid = true,
            errorMessages = [];
        const employeeId = parseInt(domElements.$employeeId.val(), 10),
            attendantDate = new Date(domElements.$attendantDate.val()),
            requestDate = new Date(domElements.$forDate.val()),
            hour = parseInt(domElements.$hour.val(), 10),
            minute = parseInt(domElements.$minute.val(), 10),
            approverId = domElements.$approverId.val(),
            verifierId = domElements.$verifierId.val();
        const dayDiff = (requestDate - attendantDate) / 1000 / 60 / 60 / 24;
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

        if (dayDiff < 0) {
            errorMessages.push('Request Date must be equal or greater than On Date!');
        }

        if (hour < 0 || hour >= 24) {
            errorMessages.push('Hour field must be between 0 and 23.');
        }

        if (minute < 0 || minute >= 60) {
            errorMessages.push('Minute field must be between 0 and 60.');
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

    $('#Employee').autocomplete({
        source: function (request, response) {
            axios({
                url: resolveClientUrl('request/GetEmployeesByActingUser'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    role: RoleSystem.OnBehalfRequest,
                    requestType: RequestType.CheckIn,
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
            } else {
                $('#EmployeeId').val('0');
            }
        }
    });

    domElements.$checkInTypeId.on('change', function () {
        getDisabledDates();
        domElements.$attendantDate.trigger('change');
    });

    domElements.$checkOutTypeId.on('change', function () {
        getDisabledDates();
        domElements.$attendantDate.trigger('change');
    });

    domElements.$attendantDate.on('change', function () {
        axios({
            url: resolveClientUrl('shift/getShiftByDate'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                employeeId: domElements.$employeeId.val(),
                date: domElements.$attendantDate.val()
            }
        }).then(response => {
            const shift = response.data;
            if (shift !== null) {
                $('#shiftname').text(shift.name);
                $('#InTime').val(shift.beginCheckInTime);
                $('#OutTime').val(shift.beginCheckOutTime);
                $('#CrossDay').val(shift.crossDay);
                $('#IsInAfterDay').val(shift.isInAfterDay);
                $('#IsOutAfterDay').val(shift.isOutAfterDay);
                bindingHourAndMinute();
                checkCrossDay();
            }
        }).catch(axiosError);
    });

    domElements.$btnSubmit.on('click', function () {
        hideValidationSummaryErrors(false);

        const isValid = validateForm();
        if (isValid) {
            domElements.$btnSubmit.prop('disabled', true);
            blockUI();

            const typeId = domElements.$checkInTypeId.prop('checked') ? RequestType.CheckIn : RequestType.CheckOut;
            const model = {
                employeeId: domElements.$employeeId.val(),
                refNo: $('#RefNO').val().trim(),
                requestTypeId: typeId,
                attendantDate: domElements.$attendantDate.val(),
                forDate: domElements.$forDate.val(),
                reasonId: $('#reasonId').val(),
                hour: domElements.$hour.val(),
                minute: domElements.$minute.val(),
                remark: $.trim(domElements.$remark.val()),
                verifierId: domElements.$verifierId.val(),
                approverId: domElements.$approverId.val()
            };

            var files = document.getElementById('file').files;
            var fd = new FormData();
            for (var i = 0; i < files.length; i++) {
                fd.append('files', files[i]);
            }
            fd.append('requestModel', JSON.stringify(model));
            var xhr = new XMLHttpRequest();
            xhr.addEventListener('load', uploadComplete, false);
            xhr.open('POST', resolveClientUrl('checkinout/Create'));
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
            setTimeout(function () { location.href = resolveClientUrl('checkinout/employeeview'); }, 3000);
        } else {
            toastr.error(data.message, '', toastOptions());
        }
    }

    getDisabledDates(initDatepicker);

    $('form').areYouSure();
});