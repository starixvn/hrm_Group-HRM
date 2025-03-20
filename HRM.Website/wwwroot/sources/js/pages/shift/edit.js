import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import { stringify } from 'qs';
import axios from 'axios';
import { resolveClientUrl, confirm, axiosError, alert } from '../../shared/common.functions';

$(function () {
    function initDeleteShiftEvent() {
        $('.js-delete-shift').on('click', function () {
            const id = $(this).attr('data-id');
            confirm('Are you sure you want to delete this shift?', () => {
                axios({
                    url: resolveClientUrl('shift/delete'),
                    method: 'POST',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    data: stringify({ shiftId: id })
                }).then(response => {
                    if (response.data.isSuccessful) {
                        document.location = resolveClientUrl('shift/index');
                    }
                    else {
                        alert(response.message);
                    }
                }).catch(axiosError);
            });
        });
    }

    function initSubmitEvent() {
        $('form').bind('submit', function () {
            $(this).find(':input').removeAttr('disabled');
        });
    }

    function initInputAttr() {
        if ($('#IsUsed').val() === 'True') {
            $('#Name').attr('disabled', 'disabled');
            $('#HourTimeIn').attr('disabled', 'disabled');
            $('#MinuteTimeIn').attr('disabled', 'disabled');
            $('#HourTimeOut').attr('disabled', 'disabled');
            $('#MinuteTimeOut').attr('disabled', 'disabled');
            $('#IsInAfterDay').attr('disabled', 'disabled');
            $('#IsOutAfterDay').attr('disabled', 'disabled');
        }
    }

    $(document).ready(() => {
        initDeleteShiftEvent();
        initSubmitEvent();
        initInputAttr();
    });
});