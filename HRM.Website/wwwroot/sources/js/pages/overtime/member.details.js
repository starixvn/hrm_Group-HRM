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

    $('#btnVerify').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Verify</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, RequestCommand.Accept, RoleSystem.Verifier);
        });
    });

    $('#btnNotVerify').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Not Verify</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, RequestCommand.NotAccept, RoleSystem.Verifier);
        });
    });

    $('#btnResetByVerifier').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reset</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, RequestCommand.Reset, RoleSystem.Verifier);
        });
    });

    $('#btnApprove').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Approve</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, RequestCommand.Approve, RoleSystem.Approver);
        });
    });

    $('#btnReject').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reject</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, RequestCommand.Reject, RoleSystem.Approver);
        });
    });

    $('#btnResetByApprover').on('click', function () {
        $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reset</strong> this request?');
        $confirmModal.modal('show');
        $btnConfirm.on('click', function () {
            onUpdateRequestStatus(requestId, RequestCommand.Reset, RoleSystem.Approver);
        });
    });
});