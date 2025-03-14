import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery.are-you-sure/jquery.are-you-sure';
import 'tipsy/src/javascripts/jquery.tipsy';
import axios from 'axios';
import moment from 'moment';

import { axiosError, confirm, initDatepicker, resolveClientUrl, blockUI, unblockUI } from '../../../shared/common.functions';
import { LeaveType, DateFormat, RequestType, DateTimeFormat } from '../../../shared/constants';

const RequestCommon = {
    showErrorMsg: function (msg, htmlObject) {
        $('.validation-summary-valid').show();
        $('.validation-summary-valid').addClass('validation-summary-errors');
        var msgWrp = $('.validation-summary-valid').find('ul:first');
        msgWrp.append("<li>" + RequestCommon.htmlDecode(msg) + "</li>");
        htmlObject = htmlObject;
    },
    showErrorMsgs: function (nessages, htmlObject) {
        $('.validation-summary-valid').show();
        $('.validation-summary-valid').addClass('validation-summary-errors');
        var msgWrp = $('.validation-summary-valid').find('ul:first');
        nessages.forEach(function (msg) {
            msgWrp.append("<li>" + RequestCommon.htmlDecode(msg) + "</li>");
        });

        htmlObject = htmlObject;
    },
    removeErrorMsg: function (isReset) {
        if (isReset) {
            $(".validation-summary-errors").addClass("validation-summary-valid").removeClass("validation-summary-errors");
        }
        var msgWrp = $('.validation-summary-valid').removeClass('validation-summary-errors');
        msgWrp.find('ul:first').html("");
        $('.validation-summary-valid').hide();
    },
    showSuccessMsg: function (msg, url) {
        $('.validation-summary-valid').show();
        var msgWrp = $('.validation-summary-valid').addClass('success').find('ul:first');
        msgWrp.append("<li>" + RequestCommon.htmlDecode(msg) + "</li>");
        confirm('Successfull!\nPress OK to continue', function () {
            location.href = resolveClientUrl(url);
        });
    },
    htmlDecode: function (value) {
        return $('<div/>').html(value).text();
    }
};

function uploadComplete(evt) {
    const data = JSON.parse(evt.target.response);
    if (data.isSuccessful) {
        $('form').removeClass('dirty');
        confirm('Successfull!\nPress OK to continue', function () {
            location.href = resolveClientUrl('Leave/EmployeeView');
        });
    } else {
        RequestCommon.showErrorMsg(data.message, LeaveRequest.employee);
    }
}

function GetFormData(leaveRequest, requestDetails, tempFile) {
    var files = document.getElementById('file').files;
    var fd = new FormData();
    for (var i = 0; i < files.length; i++) {
        fd.append('files', files[i]);
    }
    fd.append('request', JSON.stringify(leaveRequest));
    fd.append('requestDetails', JSON.stringify(requestDetails));
    fd.append('filesRemove', JSON.stringify(tempFile));
    return fd;
}

var tempFile = [];
$('.delete-file').on('click', function () {
    const fileName = $(this).attr("data-id");
    var classDelete = document.getElementById(fileName);
    classDelete.remove();
    tempFile.push(fileName);
});

const LeaveRequest = (function ($) {
    return {
        verifierId: null,
        approverId: null,
        leaveTypeId: null,
        leavetypeInfo: null,
        fromDate: null,
        toDate: null,
        employeeId: null,
        employee: null,
        fromTimeHour: null,
        fromTimeMinute: null,
        toTimeHour: null,
        toTimeMinute: null,
        totalDays: null,
        halfDayOff: null,
        requestType: null,
        requestId: null,
        annualBalance: null,
        keepInViewBalance: null,
        currentLeaveBalance: null,
        isFromTimeAfterDay: null,
        isToTimeAfterDay: null,

        init: function () {
            LeaveRequest.verifierId = $('#VerifierId');
            LeaveRequest.approverId = $('#ApproverId');
            LeaveRequest.leaveTypeId = $('#LeaveTypeId');
            LeaveRequest.leavetypeInfo = $('#leavetype-info');
            LeaveRequest.fromDate = $('#FromDate');
            LeaveRequest.toDate = $('#ToDate');
            LeaveRequest.employeeId = $('#EmployeeId');
            LeaveRequest.employee = $('#Employee');
            LeaveRequest.fromTimeHour = $('#FromTimeHour');
            LeaveRequest.fromTimeMinute = $('#FromTimeMinute');
            LeaveRequest.toTimeHour = $('#ToTimeHour');
            LeaveRequest.toTimeMinute = $('#ToTimeMinute');
            LeaveRequest.totalDays = $('#TotalDays');
            LeaveRequest.halfDayOff = $('#HalfDayOff');
            LeaveRequest.requestType = $('#RequestType');
            LeaveRequest.requestId = $('#RequestId');
            LeaveRequest.annualBalance = $('#lblAnnualBalance');
            LeaveRequest.keepInViewBalance = $('#lblKeepInViewBalance');
            LeaveRequest.keepInViewByHourBalance = $('#lblKeepInViewByHourBalance');
            LeaveRequest.currentLeaveBalance = $('#lblCurrentLeaveBalance');
            LeaveRequest.weekendLeaveTaken = $('#lblWeekendLeave');
            LeaveRequest.isFromTimeAfterDay = $('#IsFromTimeAfterDay');
            LeaveRequest.isToTimeAfterDay = $('#IsToTimeAfterDay');
            LeaveRequest.autocomplete();
            LeaveRequest.getLeaveTypeInfo();
            $('#errors-model-state').hide();

            // for edit
            initDatepicker();
            LeaveRequest.enableControlForEdit();

            LeaveRequest.enableCheckboxHospitalizationLeave();

            LeaveRequest.halfDayOff.change(function () {
                LeaveRequest.bindHalfDay();
            });
            LeaveRequest.fromDate.change(function () {
                LeaveRequest.dateChange();
            });
            LeaveRequest.toDate.change(function () {
                LeaveRequest.dateChange();
            });
            LeaveRequest.leaveTypeId.change(function () {
                LeaveRequest.leaveTypeChange();
            });

            LeaveRequest.isFromTimeAfterDay.change(function () {
                if (LeaveRequest.halfDayOff.prop('checked')) {
                    if (LeaveRequest.isFromTimeAfterDay.prop('checked')) {
                        LeaveRequest.isToTimeAfterDay.prop('checked', true);
                        let toDate = new Date(LeaveRequest.fromDate.val());
                        toDate.setDate(toDate.getDate() + 1);
                        LeaveRequest.toDate.val(moment(toDate).format(DateFormat));
                    } else {
                        LeaveRequest.isToTimeAfterDay.prop('checked', false);
                        LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
                    }
                } else {
                    LeaveRequest.isFromTimeAfterDay.prop('checked', false);
                    LeaveRequest.isToTimeAfterDay.prop('checked', false);
                    LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
                }
            });

            $('#FromTimeHour, #ToTimeHour').on('change', function () {
                const value = parseInt($(this).val(), 10) || 0;
                if (value > 23) {
                    $(this).val('00');
                } else {
                    LeaveRequest.timeChangeHandler();
                }
            });

            $('#FromTimeMinute, #ToTimeMinute').on('change', function () {
                const value = parseInt($(this).val(), 10) || 0;
                if (value > 59) {
                    $(this).val('00');
                } else {
                    LeaveRequest.timeChangeHandler();
                }
            });

            $('#btnSubmit').click(function (evt) {
                evt.preventDefault();
                RequestCommon.removeErrorMsg(false);
                LeaveRequest.saveLeaveRequest();
            });

            if (parseInt(LeaveRequest.leaveTypeId.val(), 10) === LeaveType.GeneralLeave) {
                LeaveRequest.showLeaveDetail(
                    LeaveRequest.employeeId.val(),
                    LeaveRequest.fromDate.val(),
                    LeaveRequest.toDate.val(),
                    LeaveRequest.requestId.val());
            }

            LeaveRequest.disableControlForDetail();

            if (LeaveRequest.isFromTimeAfterDay.prop('checked')) {
                $('.time-isafterday').show();
                LeaveRequest.isFromTimeAfterDay.trigger('change');
            }

            if (LeaveRequest.isToTimeAfterDay.prop('checked')) {
                let toDate = new Date(LeaveRequest.fromDate.val());
                toDate.setDate(toDate.getDate() + 1);
                LeaveRequest.toDate.val(moment(toDate).format(DateFormat));
            }

            LeaveRequest.leavetypeInfo.tipsy({
                className: 'tipsy-leave-type-info',
                gravity: $.fn.tipsy.autoWE
            });
        },
        timeChangeHandler: function () {
            const fromTime = LeaveRequest.leftPadding(LeaveRequest.fromTimeHour.val(), 2)
                + ':' + LeaveRequest.leftPadding(LeaveRequest.fromTimeMinute.val(), 2),
                toTime = LeaveRequest.leftPadding(LeaveRequest.toTimeHour.val(), 2)
                    + ':' + LeaveRequest.leftPadding(LeaveRequest.toTimeMinute.val(), 2);
            if (!LeaveRequest.isFromTimeAfterDay.prop('checked')) {
                if (fromTime > toTime) {
                    LeaveRequest.isToTimeAfterDay.prop('checked', true);
                    let toDate = new Date(LeaveRequest.fromDate.val());
                    toDate.setDate(toDate.getDate() + 1);
                    LeaveRequest.toDate.val(moment(toDate).format(DateFormat));
                } else {
                    LeaveRequest.isToTimeAfterDay.prop('checked', false);
                    LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
                }
            }
        },
        employeeChange: function () {
            LeaveRequest.getVerifierAndApprover();
            LeaveRequest.getLeaveBalance();
            LeaveRequest.getDisableDate();
            LeaveRequest.getLeaveDetail();
        },
        showLeaveDetail: function (employeeId, fromDate, toDate, requestId) {
            var leaveDetailContainer = $('#leaveDetails');
            if (leaveDetailContainer.length > 0) {
                axios({
                    url: resolveClientUrl('Leave/LeaveRequestDetail'),
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    params: { 'employeeId': employeeId, 'fromDate': fromDate, 'toDate': toDate, 'requestId': requestId },
                    responseType: 'text'
                }).then(response => {
                    leaveDetailContainer.empty();
                    leaveDetailContainer.html(response.data);
                }).catch(axiosError);
            }
        },
        saveLeaveRequest: function () {
            const isValid = LeaveRequest.validateForm();
            if (isValid) {
                RequestCommon.removeErrorMsg(false);
                const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);

                let leaveRequest = {
                    RequestId: LeaveRequest.requestId.val(),
                    EmployeeId: LeaveRequest.employeeId.val(),
                    EmployeeName: LeaveRequest.employee.val(),
                    RequestType: RequestType.Leave,
                    FromDate: LeaveRequest.fromDate.val(),
                    ToDate: LeaveRequest.toDate.val(),
                    IsHospitalApplication: $('#IsHospitalApplication').prop('checked'),
                    LeaveTypeId: leaveTypeId,
                    LeaveTypeName: LeaveRequest.leaveTypeId.find(':selected').text(),
                    TotalDays: LeaveRequest.totalDays.val(),
                    RefNo: $('#RefNo').val(),
                    Remark: $('#Remark').val(),
                    VerifierId: LeaveRequest.verifierId.val(),
                    ApproverId: LeaveRequest.approverId.val()
                };

                const requestDetails = [];
                let leaveDate = '';
                let isHalfDay = false;
                let fromTime = new Date();
                let toTime = new Date();
                if (leaveTypeId === LeaveType.GeneralLeave) {
                    $('.tabledata tbody tr').each(function () {
                        leaveDate = $.trim($('td.tdleavedate', this).first().attr('data-date'));
                        isHalfDay = $('td input.chkhalfday', this).first().prop('checked');
                        fromTime = new Date(leaveDate + (isHalfDay ? (' ' + $('td select.ddlfromtime', this).first().val()) : ''));
                        toTime = new Date(leaveDate + (isHalfDay ? (' ' + $('td select.ddltotime', this).first().val()) : ''));

                        if ($('td input.chkIsFromAfterday', this).first().prop('checked')) {
                            fromTime.setDate(fromTime.getDate() + 1);
                        }

                        if ($('td input.chkIsToAfterday', this).first().prop('checked')) {
                            toTime.setDate(toTime.getDate() + 1);
                        }

                        requestDetails.push({
                            OnDate: leaveDate,
                            WorkType: $('td select.ddlWorkType', this).first().val(),
                            LeaveTypeId: $('td select.ddl-leave-type', this).first().val(),
                            IsHalfDay: isHalfDay,
                            FromTime: moment(fromTime).format(DateTimeFormat),
                            ToTime: moment(toTime).format(DateTimeFormat)
                        });
                    });
                } else if (LeaveRequest.halfDayOff.prop('checked')) {
                    let fromDate = new Date(LeaveRequest.fromDate.val()),
                        toDate = new Date(LeaveRequest.fromDate.val()),
                        fromDateTime, toDateTime;

                    if (LeaveRequest.isFromTimeAfterDay.prop('checked')) {
                        fromDate.setDate(fromDate.getDate() + 1);
                    }

                    if (LeaveRequest.isToTimeAfterDay.prop('checked')) {
                        toDate.setDate(toDate.getDate() + 1);
                    }

                    fromDateTime = moment(fromDate).format(DateFormat) + ' ' + LeaveRequest.leftPadding(LeaveRequest.fromTimeHour.val(), 2)
                        + ':' + LeaveRequest.leftPadding(LeaveRequest.fromTimeMinute.val(), 2);
                    toDateTime = moment(toDate).format(DateFormat) + ' ' + LeaveRequest.leftPadding(LeaveRequest.toTimeHour.val(), 2)
                        + ':' + LeaveRequest.leftPadding(LeaveRequest.toTimeMinute.val(), 2);

                    requestDetails.push({
                        OnDate: LeaveRequest.fromDate.val(),
                        WorkType: 2,
                        LeaveTypeId: LeaveRequest.leaveTypeId.val(),
                        IsHalfDay: true,
                        FromTime: fromDateTime,
                        ToTime: toDateTime
                    });
                }

                var xhr = new XMLHttpRequest();
                xhr.addEventListener("load", uploadComplete, false);
                if (LeaveRequest.requestId.val() === '-1') {
                    xhr.open("POST", resolveClientUrl('leave/Create'));
                } else {
                    xhr.open("POST", resolveClientUrl('leave/Update'));
                }
                xhr.send(GetFormData(leaveRequest, requestDetails, tempFile));
            }
        },
        dateChange: function () {
            LeaveRequest.getLeaveDetail();
            LeaveRequest.getLeaveBalance();
        },
        getLeaveTypeInfo: function () {
            const description = LeaveRequest.leaveTypeId.children('option:selected').attr('title');
            LeaveRequest.leavetypeInfo.attr('title', description ? description : '');
            if (description && description.length > 0) {
                LeaveRequest.leavetypeInfo.show();
            } else {
                LeaveRequest.leavetypeInfo.hide();
            }
        },
        leaveTypeChange: function () {
            LeaveRequest.getLeaveTypeInfo();
            LeaveRequest.getDisableDate();
            LeaveRequest.getLeaveDetail();
            LeaveRequest.enableCheckboxHospitalizationLeave();
        },
        bindHalfDay: function () {
            if (LeaveRequest.halfDayOff.prop('checked')) {
                LeaveRequest.enableControlForHalfday();
                LeaveRequest.totalDays.val(0.5);
                LeaveRequest.getTimeByShift();
            } else {
                LeaveRequest.disableControlForHalfDay();
                LeaveRequest.totalDays.val(1);
            }
        },
        enableControlForHalfday: function () {
            const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);
            if (leaveTypeId === LeaveType.GeneralLeave) {
                $('.time-businesstrip').hide();
                $('#grbHalfDay').hide();
            } else if (leaveTypeId === LeaveType.KeepInViewByHour || leaveTypeId === LeaveType.BusinessDuringWorking) {
                $('.time-businesstrip').show();
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
            } else {
                $('.time-businesstrip').show();
            }

            LeaveRequest.toDate.val(LeaveRequest.fromDate.val());
            LeaveRequest.toDate.datepicker('disable').prop('disabled', false);
        },
        enableCheckboxHospitalizationLeave: function () {
            const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);
            if (leaveTypeId === LeaveType.HospitalizationLeave) {
                $('#grbHospitalizationLeave').show();
            } else {
                $('#grbHospitalizationLeave').hide();
            }
        },
        disableControlForHalfDay: function () {
            $('.time-businesstrip').hide();
            $('.time-isafterday').hide();

            LeaveRequest.toDate.datepicker('enable');
        },
        getTimeByShift: function () {
            axios({
                url: resolveClientUrl('shift/getshiftbydate'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: { employeeId: LeaveRequest.employeeId.val(), date: LeaveRequest.fromDate.val() }
            }).then(response => {
                const data = response.data;
                if (data !== null) {
                    const arrTimeIn = data.beginCheckInTime.split(':'),
                        arrTimeOut = data.beginCheckOutTime.split(':');

                    let minuteFrom = parseInt(arrTimeIn[0], 10) * 60 + parseInt(arrTimeIn[1], 10),
                        minuteTo = parseInt(arrTimeOut[0], 10) * 60 + parseInt(arrTimeOut[1], 10),
                        isFromAfterday = data.isInAfterDay,
                        isToAfterday = data.isOutAfterDay;
                    $('.time-isafterday').show();
                    if (data.isInAfterDay) {
                        minuteFrom += 1440; //60*24
                    }

                    if (data.isOutAfterDay) {
                        minuteTo += 1440;
                    }

                    let averageTime = (minuteFrom + minuteTo) / 2;
                    if (averageTime >= 1440) {
                        averageTime = averageTime - 1440;
                        isToAfterday = true;
                    } else {
                        isToAfterday = false;
                    }

                    let hour = Math.floor(averageTime / 60),
                        minute = Math.floor((averageTime % 60) / 30) * 30;
                    minute = minute === 0 ? '00' : minute;
                    if (hour < 10) {
                        hour = '0' + hour;
                    }
                    console.log(arrTimeIn);

                    LeaveRequest.fromTimeHour.val(arrTimeIn[0]);
                    LeaveRequest.fromTimeMinute.val(arrTimeIn[1]);
                    LeaveRequest.toTimeHour.val(hour);
                    LeaveRequest.toTimeMinute.val(minute);

                    LeaveRequest.isFromTimeAfterDay.prop('checked', isFromAfterday);
                    LeaveRequest.isToTimeAfterDay.prop('checked', isToAfterday);

                    if (isToAfterday) {
                        let toDate = new Date(LeaveRequest.fromDate.val());
                        toDate.setDate(toDate.getDate() + 1);
                        LeaveRequest.toDate.val(moment(toDate).format(DateFormat));
                    }
                }

                LeaveRequest.totalDays.val(0.5);
            }).catch(axiosError);
        },
        autocomplete: function () {
            const requestType = LeaveRequest.requestType.val();
            LeaveRequest.employee.autocomplete({
                source: function (request, response) {
                    axios({
                        url: resolveClientUrl('request/GetEmployeesByActingUser'),
                        method: 'GET',
                        headers: { 'X-Requested-With': 'XMLHttpRequest' },
                        params: {
                            role: 5,
                            requestType: requestType,
                            query: request.term
                        }
                    }).then(res => {
                        response($.map(res.data, function (item) {
                            return {
                                label: item.nickName + ' (' + item.fullName + ')',
                                value: item.ownerId,
                                isTopManager: item.isTopManager
                            };
                        }));
                    }).catch(axiosError);
                },
                minLength: 1,
                select: function (event, ui) {
                    event.preventDefault();
                    LeaveRequest.employee.val(ui.item.label);
                    LeaveRequest.employeeId.val(ui.item.value);
                    $('#IsTopManager').prop('checked', ui.item.isTopManager);
                    LeaveRequest.employeeChange();
                    RequestCommon.removeErrorMsg(true);
                },
                focus: function (event, ui) {
                    event.preventDefault();
                    LeaveRequest.employee.val(ui.item.label);
                    LeaveRequest.employeeId.val(ui.item.value);
                    $('#IsTopManager').prop('checked', ui.item.isTopManager);
                },
                change: function (event, ui) {
                    event.preventDefault();
                    if (ui && ui.item) {
                        LeaveRequest.employee.val(ui.item.label);
                        LeaveRequest.employeeId.val(ui.item.value);
                        $('#IsTopManager').prop('checked', ui.item.isTopManager);
                    } else {
                        LeaveRequest.employeeId.val('0');
                    }
                }
            });
        },
        hideHalfday: function () {
            const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);
            if (leaveTypeId === LeaveType.UnpaidLeaveInsurance) {
                LeaveRequest.halfDayOff.prop('checked', false);
                LeaveRequest.disableControlForHalfDay();
                $('#grbHalfDay').hide();
            }
        },
        enableControlForEdit: function () {
            $('#RefNo').attr('disabled', 'disabled');
            if (parseInt(LeaveRequest.leaveTypeId.val(), 10) === LeaveType.GeneralLeave) {
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
                $('.time-isafterday').hide();
            }

            if (LeaveRequest.halfDayOff.prop('checked')) {
                LeaveRequest.enableControlForHalfday();
            } else {
                LeaveRequest.hideHalfday();
            }
        },
        bindLeaveRequestDetailForEdit: function () {
            if (parseInt(LeaveRequest.leaveTypeId.val(), 10) === LeaveType.GeneralLeave) {
                LeaveRequest.showLeaveDetail(LeaveRequest.employeeId.val(),
                    LeaveRequest.fromDate.val(),
                    LeaveRequest.toDate.val(),
                    LeaveRequest.requestId.val());
            }
        },
        disableControlForDetail: function () {
            if ($('#IsView').val() === 'True') {
                $('.form-horizontal input').attr('disabled', 'disabled');
                $('select').attr('disabled', 'disabled');
                $('textarea').attr('disabled', 'disabled');

                if (!LeaveRequest.halfDayOff.prop('checked')) {
                    LeaveRequest.hideHalfday();
                }
            }
        },
        validateForm: function () {
            let isValid = true,
                errorString = [],
                empName = LeaveRequest.employee.val(),
                fromDate = LeaveRequest.fromDate.val(), toDate = LeaveRequest.toDate.val(),
                approver = LeaveRequest.approverId.val(), verifier = LeaveRequest.verifierId.val(),
                remark = $('#Remark').val();
            const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);

            let start = $("[name='FromDate']").datepicker('getDate'),
                end = $("[name='ToDate']").datepicker('getDate'),
                days = (end - start) / 1000 / 60 / 60 / 24;

            let files = document.getElementById('file').files,
                allowedExtensions = /(\.jpg|\.png|\.jpeg|\.pdf|\.doc|\.docx|\.xls|\.xlsx|\.txt)$/i;

            if (empName !== undefined && (empName === null || empName === '')) {
                errorString.push('The Employee field is required.');
            }
            if (fromDate === null || fromDate === '') {
                errorString.push('From Date field is required.');
            }
            if (toDate === null || toDate === '') {
                errorString.push('To Date field is required.');
            }

            if (leaveTypeId === LeaveType.GeneralLeave) {
                const rows = $('.tabledata tbody tr'),
                    totalRows = rows.length;
                for (let i = 0; i < totalRows; i++) {
                    const row = $(rows[i]),
                        leaveDate = $.trim($('td.tdleavedate', row).first().attr('data-date')),
                        isHalfDay = $('td input.chkhalfday', row).first().prop('checked');
                    let fromTime = moment(leaveDate + (isHalfDay ? (' ' + $('td select.ddlfromtime', row).first().val()) : '')),
                        toTime = moment(leaveDate + (isHalfDay ? (' ' + $('td select.ddltotime', row).first().val()) : ''));

                    if ($('td input.chkIsFromAfterday', row).first().prop('checked')) {
                        fromTime.add(1, 'days');
                    }

                    if ($('td input.chkIsToAfterday', row).first().prop('checked')) {
                        toTime.add(1, 'days');
                    }

                    if (fromTime.isAfter(toTime)) {
                        errorString.push('To Time is invalid.');
                    }
                }
            } else {
                if (LeaveRequest.halfDayOff.prop('checked')) {
                    fromDate = moment(LeaveRequest.fromDate.val());
                    toDate = moment(LeaveRequest.fromDate.val());
                    if (LeaveRequest.isFromTimeAfterDay.prop('checked')) {
                        fromDate = fromDate.add(1, 'day');
                    }
                    if (LeaveRequest.isToTimeAfterDay.prop('checked')) {
                        toDate = toDate.add(1, 'day');
                    }
                    fromDate = fromDate.set('hour', LeaveRequest.leftPadding(LeaveRequest.fromTimeHour.val(), 2));
                    fromDate = fromDate.set('minute', LeaveRequest.leftPadding(LeaveRequest.fromTimeMinute.val(), 2));
                    toDate = toDate.set('hour', LeaveRequest.leftPadding(LeaveRequest.toTimeHour.val(), 2));
                    toDate = toDate.set('minute', LeaveRequest.leftPadding(LeaveRequest.toTimeMinute.val(), 2));
                    if (fromDate.isAfter(toDate)) {
                        errorString.push('Time is invalid.');
                    }
                }

                if (leaveTypeId === LeaveType.HospitalizationLeave && $('#IsHospitalApplication').prop('checked') === false) {
                    errorString.push('Applicable for stay in Hospital only field is required.');
                }
            }

            if (!$('#IsTopManager').prop('checked')) {
                if (verifier === null || verifier === '') {
                    errorString.push('Verified by field is required.');
                }
                if (approver === null || approver === '') {
                    errorString.push('Approved by field is required.');
                }
            }

            if (days < 0) {
                errorString.push('To Date must be greater than From Date.');
            }

            if ($.trim(remark).length === 0) {
                errorString.push('The Remark field is required.');
            }

            for (var i = 0; i < files.length; i++) {
                if (files[i].size / 1024 > 5120) {
                    errorString.push('Attach file (' + files[i].name + ') is to large to upload!. Maximum = 5120kb');
                }
                if (!allowedExtensions.exec(files[i].name)) {
                    errorString.push('File type of ' + files[i].name + ' incorrect!');
                }
            }

            if (errorString.length > 0) {
                RequestCommon.showErrorMsgs(errorString);
                isValid = false;
            }

            return isValid;
        },
        getVerifierAndApprover: function () {
            const promises = [
                axios({
                    url: resolveClientUrl('request/getVerifiers'),
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    params: {
                        employeeId: LeaveRequest.employeeId.val(),
                        requestType: LeaveRequest.requestType.val()
                    }
                }),
                axios({
                    url: resolveClientUrl('request/getApprovers'),
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    params: {
                        employeeId: LeaveRequest.employeeId.val(),
                        requestType: LeaveRequest.requestType.val()
                    }
                })
            ];

            Promise.all(promises)
                .then(([verifierResponse, approverResponse]) => {
                    const verifiers = verifierResponse.data,
                        approvers = approverResponse.data;
                    let options = '';
                    for (let i = 0; i < verifiers.length; i++) {
                        options += "<option value='" + verifiers[i].OwnerId + "'>" + verifiers[i].NickName + " (" + verifiers[i].FullName + ")</option>";
                    }
                    LeaveRequest.verifierId.html(options);

                    options = '';
                    for (let i = 0; i < approvers.length; i++) {
                        options += "<option value='" + approvers[i].OwnerId + "'>" + approvers[i].NickName + " (" + approvers[i].FullName + ")</option>";
                    }
                    LeaveRequest.approverId.html(options);
                }).catch(axiosError);
        },
        getLeaveBalance: function () {
            axios({
                url: resolveClientUrl('leave/GetLeaveBalance'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    employeeId: LeaveRequest.employeeId.val(),
                    requestId: LeaveRequest.requestId.val(),
                    toDate: LeaveRequest.toDate.val()
                }
            }).then(response => {
                const data = response.data;
                if (data !== null) {
                    LeaveRequest.annualBalance.html(data.annualBalance);
                    LeaveRequest.keepInViewBalance.html(data.keepInViewBalance);
                    LeaveRequest.keepInViewByHourBalance.html(data.keepInViewByHourBalance);
                    LeaveRequest.currentLeaveBalance.html(data.currentLeaveBalanceDay);
                    LeaveRequest.weekendLeaveTaken.html(data.weekedLeaveDayTaken);
                    $('#lblHospitalizationLeaveBalance').html(data.hospitalizationLeaveBalance);
                    $('#lblSickLeaveBalance').html(data.sickLeaveBalance);
                }
            }).catch(axiosError);
        },
        getTotalLeaveDay: function () {
            axios({
                url: resolveClientUrl('leave/getTotalLeaveDay'),
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    leaveTypeId: parseInt(LeaveRequest.leaveTypeId.val(), 10),
                    RequestId: LeaveRequest.requestId.val(),
                    employeeId: LeaveRequest.employeeId.val(),
                    fromDate: LeaveRequest.fromDate.val(),
                    toDate: LeaveRequest.toDate.val()
                }
            }).then(response => {
                if (response.data !== null) {
                    LeaveRequest.totalDays.val(response.data);
                }
            }).catch(axiosError);
        },
        getUnavailableDate: function () {
            blockUI();
            axios({
                url: resolveClientUrl('leave/getDisableDate'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    leaveTypeId: parseInt(LeaveRequest.leaveTypeId.val(), 10),
                    requestId: LeaveRequest.requestId.val(),
                    employeeId: LeaveRequest.employeeId.val(),
                    date: new Date().toDateString()
                }
            }).then(response => {
                if (response.data.length > 0) {
                    LeaveRequest.fromDate.attr('disabledDate', response.data.join(','));
                    LeaveRequest.toDate.attr('disabledDate', response.data.join(','));
                }
                else {
                    LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                }
            }).catch(axiosError).then(() => unblockUI());
        },
        getDisableDate: function () {
            var minDate = new Date(LeaveRequest.fromDate.attr('minDate'));
            var tempDate = new Date(new Date(minDate).setMonth(minDate.getMonth() + 12));
            const leaveTypeId = parseInt(LeaveRequest.leaveTypeId.val(), 10);

            if (leaveTypeId === LeaveType.GeneralLeave) {
                $('#grbHalfDay').hide();
                $('#grbTotalDay').hide();
                $('.time-businesstrip').hide();
                $('.time-isafterday').hide();
                LeaveRequest.toDate.datepicker('enable');
                LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
            } else if (leaveTypeId === LeaveType.KeepInViewByHour
                || leaveTypeId === LeaveType.BusinessDuringWorking) {
                LeaveRequest.halfDayOff.prop('checked', true);
                $('#grbTotalDay').hide();
            } else {
                $('#grbTotalDay').show();
                if (leaveTypeId !== LeaveType.UnpaidLeaveInsurance) {
                    $('#grbHalfDay').show();
                } else {
                    LeaveRequest.halfDayOff.prop('checked', false);
                    LeaveRequest.bindHalfDay();
                    $('#grbHalfDay').hide();
                }

                if (leaveTypeId === LeaveType.MaternityLeave) {
                    tempDate = new Date(new Date(minDate).setMonth(minDate.getMonth() + 12));
                    LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.getUnavailableDate();
                } else if (leaveTypeId === LeaveType.PublicHoliday) {
                    LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                } else if (leaveTypeId === LeaveType.OffsetLeave) {
                    tempDate = new Date(new Date(minDate).setMonth(minDate.getMonth() + 13));
                    LeaveRequest.fromDate.attr('disabledDate', '2010-01-01');
                    LeaveRequest.toDate.attr('disabledDate', '2010-01-01');
                } else {
                    LeaveRequest.getUnavailableDate();
                }
            }

            var maxDate = new Date(1900 + tempDate.getYear(), tempDate.getMonth(), 0);
            LeaveRequest.fromDate.datepicker('option', 'maxDate', maxDate);
            LeaveRequest.toDate.datepicker('option', 'maxDate', maxDate);
        },
        getLeaveDetail: function () {
            if (parseInt(LeaveRequest.leaveTypeId.val(), 10) === LeaveType.GeneralLeave) {
                $('#leaveDetailContainer').show();
                LeaveRequest.showLeaveDetail(
                    LeaveRequest.employeeId.val(),
                    LeaveRequest.fromDate.val(),
                    LeaveRequest.toDate.val(),
                    LeaveRequest.leaveTypeId.val());
            } else {
                $('#leaveDetailContainer').hide();
                $('#leaveDetails').empty();
                if (LeaveRequest.halfDayOff.prop('checked')) {
                    LeaveRequest.bindHalfDay();
                } else {
                    LeaveRequest.getTotalLeaveDay();
                }
            }
        },
        leftPadding: function (i, l, s) {
            var o = i.toString();
            if (!s) { s = '0'; }
            while (o.length < l) {
                o = s + o;
            }
            return o;
        }
    };
}(jQuery));

$(document).ready(function () {
    LeaveRequest.init();
    $('form').areYouSure();
});