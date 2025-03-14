import 'jquery.are-you-sure/jquery.are-you-sure';
import axios from 'axios';
import { LeaveType } from '../../../shared/constants';
import { axiosError, resolveClientUrl } from '../../../shared/common.functions';

var LeaveRequestDetail = (function ($) {
    const workTypeEnum = {
        off: '1',
        leave: '2',
        onduty: '3'
    };
    function getTotalLeave() {
        let totalAnnualLeave = 0,
            totalUnpaidLeave = 0,
            objectTr = $('.tabledata tbody tr');
        for (let i = 0; i < objectTr.length; i++) {
            let currentObject = objectTr[i];
            if ($(currentObject).find('.ddlWorkType').first().val() === workTypeEnum.leave) {
                const leaveType = parseInt($(currentObject).find('.ddl-leave-type').first().val(), 10);
                if (leaveType === LeaveType.AnnualLeave) {
                    totalAnnualLeave += parseFloat($.trim($(currentObject).find('.tdnumberleaves').first().html()));
                }

                if (leaveType === LeaveType.UnpaidLeave) {
                    totalUnpaidLeave += parseFloat($.trim($(currentObject).find('.tdnumberleaves').first().html()));
                }
            }
        }

        $('#totalAnnualLeave').html(totalAnnualLeave);
        $('#totalUnpaidLeave').html(totalUnpaidLeave);
    }

    function workTypeChange(element) {
        const workType = element.val();
        if (element.closest('tr').find('.chkhalfday').prop('checked')) {
            element.closest('tr').find('.ddlfromtime').show();
            element.closest('tr').find('.ddltotime').show();
            element.closest('tr').find('.timehalfday').show();
        } else {
            element.closest('tr').find('.ddlfromtime').hide();
            element.closest('tr').find('.ddltotime').hide();
            element.closest('tr').find('.timehalfday').hide();
        }

        if (workType === workTypeEnum.leave) {
            element.closest('tr').find('.ddl-leave-type').show();
            element.closest('tr').find('.tdnumberleaves').show();
            element.closest('tr').find('.chkhalfday').removeAttr('disabled');
        } else if (workType === workTypeEnum.off || workType === workTypeEnum.onduty) {
            element.closest('tr').find('.ddl-leave-type').hide();
            element.closest('tr').find('.tdnumberleaves').hide();
            element.closest('tr').find('.ddlfromtime').hide();
            element.closest('tr').find('.ddltotime').hide();
            element.closest('tr').find('.timehalfday').hide();
            element.closest('tr').find('.chkhalfday').prop('checked', false).attr('disabled', 'disabled');
            element.closest('tr').find('.addhalfday').hide();
            element.closest('tr').find('.tdnumberleaves').html('1');
        } else {
            element.closest('tr').find('.ddl-leave-type').hide();
            element.closest('tr').find('.tdnumberleaves').hide();
            element.closest('tr').find('.chkhalfday').removeAttr('disabled');
        }

        getTotalLeave();
    }

    function showHideControlByWorkType() {
        let objectTr = $('.tabledata tbody tr');
        const length = objectTr.length;
        if (length > 0) {
            let object = objectTr[0];
            for (let i = 0; i < length; i++) {
                object = objectTr[i];
                if ($(object).find('.ddlWorkType').first().val() !== workTypeEnum.leave) {
                    $(object).find('.ddl-leave-type').hide();
                    $(object).find('.tdnumberleaves').hide();
                    if ($(object).find('.ddlWorkType').first().val() === workTypeEnum.off) {
                        $(object).find('.chkhalfday').prop('disabled', true);
                    }
                }
            }
        }
    }

    function disableControlForDetail() {
        if ($('#IsView').val() === 'True') {
            $('.form-horizontal input').prop('disabled', true);
            $('select').prop('disabled', true);
            $('textarea').prop('disabled', true);
            $('.tdhalfday').hide();
            getTotalLeave();
        }
    }

    return {
        init: function () {
            $('th.tdhalfday').width(100);

            LeaveRequestDetail.showHideHalfDay();
            showHideControlByWorkType();

            $('#leaveDetails').off('change', '.ddlWorkType').on('change', '.ddlWorkType', function () {
                workTypeChange($(this));
            });

            $('#leaveDetails').off('change', '.ddl-leave-type').on('change', '.ddl-leave-type', function () {
                const leaveTypeId = parseInt($(this).val(), 10);
                LeaveRequestDetail.showHideCheckboxHalfDay($(this).closest('tr'), leaveTypeId);
                getTotalLeave();
            });

            $('#leaveDetails').off('change', '.chkhalfday').on('change', '.chkhalfday', function () {
                LeaveRequestDetail.halfdayChange($(this).closest('tr'));
            });

            $('.addhalfday').on('click', function () {
                var object = $(this);
                var trObject = object.parent().parent();
                var trCopy = trObject.clone();
                var halfdayObject = trCopy.find('.addhalfday').first().html('- Remove a half').removeClass('addhalfday').addClass('removehaflday');
                var workTypeObject = trCopy.find('.ddlWorkType').first();

                halfdayObject.click(function () {
                    $(this).parent().parent().remove();
                    trObject.find('.addhalfday').first().html('+ Add a half');
                    trObject.find('.chkhalfday').removeAttr('disabled');
                    getTotalLeave();
                });

                workTypeObject.change();

                trObject.after(trCopy);
                trObject.find('.addhalfday').first().html('');
                getTotalLeave();

                trObject.find('.chkhalfday').prop('checked', true).attr('disabled', 'disabled');
                trCopy.find('.chkhalfday').attr('checked', true).attr('disabled', 'disabled');

                // bind value for time and check box of copy row
                trCopy.find('.ddlfromtime').val(trObject.find('.ddltotime').val());
                trCopy.find('.ddltotime').val(trCopy.find('.ddltotime').attr('data-outtime'));
                if (trObject.find('.chkIsToAfterday').prop('checked')) {
                    trCopy.find('.chkIsFromAfterday').attr('checked', true);
                } else {
                    trCopy.find('.chkIsFromAfterday').prop('checked', false);
                }

                if (trCopy.find('.ddltotime').attr('data-isOutAfterday') === 'true') {
                    trCopy.find('.chkIsToAfterday').prop('checked', trCopy.find('.ddltotime').attr('data-isOutAfterday'));
                } else {
                    trCopy.find('.chkIsToAfterday').prop('checked', false);
                }
            });

            $('.ddl-leave-type').each(function () {
                if ($(this).closest('tr').find('.chkhalfday').prop('checked')) {
                    $(this).find('option[value=' + LeaveType.UnpaidLeaveInsurance + ']').prop('disabled', true);
                }

                $(this).trigger('change');
            });

            disableControlForDetail();
        },
        halfdayChange: function (trObject) {
            if (trObject.find('.chkhalfday').prop('checked')) {
                trObject.find('.ddl-leave-type option[value=' + LeaveType.UnpaidLeaveInsurance + ']').prop('disabled', true);
                $('.tdhalfday').show();
                trObject.find('.addhalfday').first().html('+ Add a half').show();
                var fromTimeObject = trObject.find('.ddlfromtime');
                var toTimeObject = trObject.find('.ddltotime');

                fromTimeObject.show();
                toTimeObject.show();
                trObject.find('.timehalfday').show();
                trObject.find('.tdnumberleaves').html('0.5');

                LeaveRequestDetail.fillTimeForHalfday(trObject);
            } else {
                trObject.find('.ddl-leave-type option[value=' + LeaveType.UnpaidLeaveInsurance + ']').prop('disabled', false);
                trObject.find('.addhalfday').first().html();
                trObject.find('.ddlfromtime').hide();
                trObject.find('.ddltotime').hide();
                trObject.find('.timehalfday').hide();
                trObject.find('.tdnumberleaves').html('1');

                var isShow = false;
                var otherTrObject = $('.tabledata tbody tr');
                for (var i = 0; i < otherTrObject.length; i++) {
                    var object = otherTrObject[i];
                    if ($(object).find('.chkhalfday').first().prop('checked')) {
                        isShow = true;
                        break;
                    }
                }

                if (isShow) {
                    $('.tdhalfday').show();
                } else {
                    $('.tdhalfday').hide();
                }
            }

            getTotalLeave();
        },
        fillTimeForHalfday: function (trObject) {
            axios({
                url: resolveClientUrl('shift/getshiftbydate'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: { employeeId: $('#EmployeeId').val(), date: trObject.find('.tdleavedate').attr('data-date') }
            }).then(response => {
                const data = response.data;
                if (data !== null) {
                    const arrTimeIn = data.BeginCheckInTime.split(':');
                    const arrTimeOut = data.BeginCheckOutTime.split(':');

                    let minuteFrom = (parseInt(arrTimeIn[0], 10) * 60) + parseInt(arrTimeIn[1], 10);
                    let minuteTo = (parseInt(arrTimeOut[0], 10) * 60) + parseInt(arrTimeOut[1], 10);
                    let isFromAfterday = data.IsInAfterDay;
                    let isToAfterday = false;
                    if (data.IsInAfterDay) {
                        minuteFrom += 1440; //60*24
                    }

                    if (data.IsOutAfterDay) {
                        minuteTo += 1440;
                    }

                    let averageTime = (minuteFrom + minuteTo) / 2;
                    if (averageTime >= 1440) {
                        averageTime = averageTime - 1440;
                        isToAfterday = true;
                    }

                    let hour = Math.floor(averageTime / 60);
                    let minute = Math.floor((averageTime % 60) / 30) * 30;
                    minute = minute === 0 ? '00' : minute;
                    if (hour < 10) {
                        hour = '0' + hour;
                    }

                    let toTime = hour + ':' + minute;
                    trObject.find('.ddlfromtime').val(data.BeginCheckInTime);
                    trObject.find('.ddltotime').val(toTime);

                    trObject.find('.chkIsFromAfterday').prop('checked', isFromAfterday);
                    trObject.find('.chkIsToAfterday').prop('checked', isToAfterday);
                    trObject.find('.ddltotime').attr('data-outtime', data.BeginCheckOutTime);
                    trObject.find('.ddltotime').attr('data-isOutAfterday', data.IsOutAfterDay);
                }
            }).catch(axiosError);
        },
        enableControlForEdit: function () {
            LeaveRequestDetail.showHideHalfDay();
        },
        showHideCheckboxHalfDay: function (trObject, leaveTypeId) {
            var $chkhalfday = trObject.find('.chkhalfday');
            if (leaveTypeId !== LeaveType.UnpaidLeaveInsurance) {
                $chkhalfday.show();
            } else {
                if ($chkhalfday.prop('checked')) {
                    $chkhalfday.prop('checked', false)
                    $chkhalfday.trigger('change');
                }

                $chkhalfday.hide();
            }
        },
        showHideHalfDay: function () {
            var isShow = false;
            var objectTr = $('.tabledata tbody tr');
            var length = objectTr.length;
            if (length > 0) {
                var oldObject = objectTr[0];
                var object = objectTr[0];
                for (var i = 0; i < objectTr.length; i++) {
                    object = objectTr[i];
                    if ($(object).find('.chkhalfday').first().prop('checked')) {
                        $(object).find('.addhalfday').first().html('+ Add a half');
                        if (i > 0) {
                            oldObject = objectTr[i - 1];
                            LeaveRequestDetail.bindSecondHalfDay(object, oldObject);
                        }

                        isShow = true;
                        $(object).find('.ddlfromtime').show();
                        $(object).find('.ddltotime').show();
                        $(object).find('.timehalfday').show();
                    }
                    else {
                        $(object).find('.ddlfromtime').hide();
                        $(object).find('.ddltotime').hide();
                        $(object).find('.timehalfday').hide();
                    }
                }
            }

            if (isShow) {
                $('.tdhalfday').show();
            } else {
                $('.tdhalfday').hide();
                $('.ddlfromtime').hide();
                $('.ddltotime').hide();
                $('.timehalfday').hide();
            }
        },
        bindSecondHalfDay: function (object, oldObject) {
            if ($.trim($(object).find('.tdleavedate').first().html()) === $.trim($(oldObject).find('.tdleavedate').first().html())) {
                var halfdayObject = $(object).find('.addhalfday').first().html('- Remove a half');
                halfdayObject.removeClass('addhalfday').addClass('removehaflday');
                $(oldObject).find('.addhalfday').first().html('');
                $(object).find('.chkhalfday').attr('disabled', 'disabled');
                $(oldObject).find('.chkhalfday').attr('disabled', 'disabled');

                halfdayObject.click(function () {
                    $(this).parent().parent().remove();
                    $(oldObject).find('.addhalfday').first().html('+ Add a half');
                    $(oldObject).find('.chkhalfday').removeAttr('disabled');
                    getTotalLeave();
                });
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    LeaveRequestDetail.init();
    $('form').areYouSure();
});