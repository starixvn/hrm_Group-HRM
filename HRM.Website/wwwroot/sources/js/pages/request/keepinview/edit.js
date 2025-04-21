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
import { KeepInViewType } from '../../../shared/constants';

$(document).ready(function () {
    const requestId = $('#RequestId').val(),
        employeeId = $('#EmployeeId').val();
    let domElements = {
        $keepInViewType: $('input[type="radio"][name="KeepInViewType"]'),
        $keepInViewByDay: $('#KeepInViewByDay'),
        $groupHalfDay: $('#groupHalfDay'),
        $halfDay: $('#HalfDay'),
        $date: $('#Date'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $timeGroup: $('#timeGroup'),
        $fromTime: $('#FromTime'),
        $toTime: $('#ToTime'),
        $crossDay: $('#CrossDay'),
        $remark: $('#Remark'),
        $shiftId: $('#ShiftId'),
        $groupNewShift: $('#groupNewShift'),
        $btnSubmit: $('#btnSubmit')
    };
    let getDisabledDates = function (callback) {
        blockUI();
        const requestTypeId = domElements.$keepInViewByDay.prop('checked') ? KeepInViewType.KeepInViewByDay : KeepInViewType.KeepInViewByHour;
        axios({
            url: resolveClientUrl('keepinview/getDisabledDate'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                requestId: requestId,
                requestTypeId: requestTypeId,
                employeeId: employeeId,
                fromDate: domElements.$date.attr('mindate'),
                toDate: domElements.$date.attr('maxdate')
            }
        }).then(response => {
            const totalItems = response.data.length;
            let disabledDate = '';
            for (let i = 0; i < totalItems; i++) {
                disabledDate = disabledDate.length > 0
                    ? disabledDate + ',' + moment(response.data[i]).format('YYYY/MM/DD')
                    : moment(response.data[i]).format('YYYY/MM/DD');
            }

            domElements.$date.attr('disabledDate', disabledDate);

            if (callback !== undefined && typeof callback === 'function') {
                callback();
            }
        }).catch(axiosError).then(() => unblockUI());
    };
    let validateForm = function () {
        let isValid = true,
            errorMessages = [];
        const approverId = domElements.$approverId.val(),
            verifierId = domElements.$verifierId.val();
        let files = document.getElementById('file').files,
            allowedExtensions = /(\.jpg|\.png|\.jpeg|\.pdf|\.doc|\.docx|\.xls|\.xlsx|\.txt)$/i;

        if (!$('#IsTopManager').prop('checked')) {
            if (verifierId === null || verifierId === '') {
                errorMessages.push('Verified by field is required.');
            }
            if (approverId === null || approverId === '') {
                errorMessages.push('Approved by field is required.');
            }
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

    var tempFile = [];
    $('.delete-file').on('click', function () {
        const fileName = $(this).attr("data-id");
        var classDelete = document.getElementById(fileName);
        classDelete.remove();
        tempFile.push(fileName);
    });

    domElements.$keepInViewType.on('change', function () {
        getDisabledDates();

        if (domElements.$keepInViewByDay.prop('checked')) {
            domElements.$groupHalfDay.show();
            domElements.$groupNewShift.show();
            if (domElements.$halfDay.prop('checked')) {
                domElements.$timeGroup.show();
            } else {
                domElements.$timeGroup.hide();
            }
        } else {
            domElements.$groupHalfDay.hide();
            domElements.$groupNewShift.hide();
            domElements.$timeGroup.show();
        }
    });

    domElements.$halfDay.on('change', function () {
        if (domElements.$halfDay.prop('checked')) {
            domElements.$timeGroup.show();
        } else {
            domElements.$timeGroup.hide();
        }
    });

    domElements.$date.on('change', function () {
        axios({
            url: resolveClientUrl('shift/getShiftByDate'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                employeeId: employeeId,
                date: domElements.$date.val()
            }
        }).then(response => {
            const shift = response.data;
            if (shift !== null) {
                domElements.$fromTime.val(shift.BeginCheckInTime);
                domElements.$toTime.val(shift.BeginCheckOutTime);
                domElements.$crossDay.prop('checked', shift.CrossDay);
            }
        }).catch(axiosError);
    });

    domElements.$btnSubmit.on('click', function () {
        hideValidationSummaryErrors(false);
        const typeId = domElements.$keepInViewByDay.prop('checked') ? KeepInViewType.KeepInViewByDay : KeepInViewType.KeepInViewByHour;
        const model = {
            requestId: requestId,
            employeeId: employeeId,
            keepInViewType: typeId,
            halfDay: domElements.$halfDay.prop('checked'),
            date: domElements.$date.val(),
            fromTime: domElements.$fromTime.val(),
            toTime: domElements.$toTime.val(),
            crossDay: domElements.$crossDay.prop('checked'),
            verifierId: domElements.$verifierId.val(),
            approverId: domElements.$approverId.val(),
            shiftId: typeId === 1 ? domElements.$shiftId.val() : null,
            remark: $.trim(domElements.$remark.val())
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
            fd.append('filesRemove', JSON.stringify(tempFile));
            var xhr = new XMLHttpRequest();
            xhr.addEventListener('load', uploadComplete, false);
            xhr.open('POST', resolveClientUrl('keepinview/edit'));
            xhr.send(fd);
        }
    });

    function uploadComplete(evt) {
        domElements.$btnSubmit.prop('disabled', false);
        unblockUI();

        const data = JSON.parse(evt.target.response);
        if (data.isSuccessful) {
            $('form').removeClass('dirty');
            toastr.success('Update successfully!', '', toastOptions());
            setTimeout(function () { location.href = resolveClientUrl('keepinview/employeeview'); }, 3000);
        } else {
            toastr.error(data.message, '', toastOptions());
        }
    }

    let showHideWorkingShift = function () {
        if (domElements.$keepInViewByDay.prop('checked')) {
            domElements.$groupNewShift.show();
        } else
            domElements.$groupNewShift.hide();
    }

    $('form').areYouSure();

    getDisabledDates(initDatepicker);

    showHideWorkingShift();
});