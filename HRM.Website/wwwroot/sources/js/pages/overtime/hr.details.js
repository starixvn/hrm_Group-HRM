import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import 'tipsy/src/javascripts/jquery.tipsy';
import toastr from 'toastr';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, floatThead, toastOptions, resolveClientUrl } from '../../shared/common.functions';
import { RequestCommand, RoleSystem } from '../../shared/constants';

$(document).ready(function () {
    const requestId = parseInt($('#RequestId').val(), 10);
    let $confirmModal = $('#confirmModal'),
        $btnConfirm = $confirmModal.find('#btnConfirm');

    $('.tooltip-tipsy').tipsy({
        gravity: 'e'
    });

    floatThead('table#data');

    let onUpdateRequestStatus = function (requestIds, command, roleId) {
        axios({
            url: resolveClientUrl('overtime/EditMonthlyRequestStatus'),
            method: 'POST',
            data: stringify({ requestIds, command, roleId }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;
            if (data.IsSuccessful) {
                toastr.success('Updated Successfully!', '', toastOptions());
                setTimeout(function () { window.location.reload(); }, 1000);
            } else {
                toastr.error(data.Message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    $('#btnProcess').on('click', function () {
        $btnConfirm.off('click');
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Process</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, RequestCommand.Process, RoleSystem.HR);
        });
    });

    $('#btnVoid').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Void</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, RequestCommand.Void, RoleSystem.HR);
        });
    });

    $('#btnReset').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reset</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, RequestCommand.Reset, RoleSystem.HR);
        });
    });
});