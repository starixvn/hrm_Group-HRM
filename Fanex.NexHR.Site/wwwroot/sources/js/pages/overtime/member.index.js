import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-contextmenu/dist/jquery.contextMenu';
import '../../../libs/monthPicker/monPickr-0.0.3';
import toastr from 'toastr';
import axios from 'axios';
import { stringify, parse } from 'qs';

import { axiosError, floatThead, toastOptions, blockUI, unblockUI, resolveClientUrl, downloadFile, sortTable } from '../../shared/common.functions';
import { RequestCommand, CurrentRequestStatus, RoleSystem } from '../../shared/constants';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';

function sortMember() {
    $('th.col-sorter').on('click', function () {
        sortTable('table#data', this);
        let hasCheckBoxColumn = $('#listMemberOvertime').hasClass('has-check-box-column');
        $('table#data tbody tr').each(function (index) {
            let rowIndex = index + 1;
            $(this).find(`td:eq(${hasCheckBoxColumn ? 1 : 0})`).html(rowIndex++);
        });
    });
}

$(document).ready(function () {
    sortMember();
    const currentUserId = parseInt($('#CurrentUserId').val(), 10);
    let $confirmModal = $('#confirmModal'),
        $btnConfirm = $confirmModal.find('#btnConfirm'),
        $monthPicker = $('#MonthControl'),
        $table = $('table#data');
    $monthPicker.monPickr({
        showDateRange: false,
        sortDesc: true,
        minDate: $('#MinDate').val(),
        maxDate: $('#MaxDate').val(),
        today: undefined,
        month: $('#Month').val() === '' ? undefined : $('#Month').val(),
        afterChangeMonth: function () {
            const month = $monthPicker.monPickr('getMonth', 'MM');
            const year = $monthPicker.monPickr('getMonth', 'yyyy');
            $('#Month').val(`${year}-${month}-01`);
        }
    });

    $('.monpickr-dropdown').hide();

    if ($('#Month').val() === '') {
        $('.monpickr-selected-month.monpickr-input.ui-widget-content').val('');
    }

    axios({
        url: resolveClientUrl('Section/GetOrgChart'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    }).then(response => {
        $('#Team').multiSelectTreeView({
            data: response.data,
            selectedItems: $('#TeamIds').val(),
            hasSearchBox: true
        });
    }).catch(axiosError);

    floatThead('table#data');

    $('#btnSearch').on('click', function () {
        const statusId = parseInt($('#StatusId').val(), 10),
            month = $('#Month').val();

        if ((statusId === 0 || statusId === CurrentRequestStatus.Processed) && month === '') {
            toastr.error('Please select a month to search.', '', toastOptions());
        } else {
            const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
            $('#TeamIds').val(teamIds === '' ? ' ' : teamIds);

            $('form.filter').submit();
        }
    });

    $('#ckbAll').on('change', function (event) {
        $table.find('tbody input[type=checkbox]').prop('checked', event.target.checked);
    });

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
                toastr.success('Updated successfully!', '', toastOptions());
                setTimeout(function () { $('#btnSearch').trigger('click'); }, 1000);
            } else {
                toastr.error(data.Message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    $('#btnVerify').on('click', function () {
        let objSelected = $table.find('tbody input[type=checkbox]:checked');
        if (objSelected.length > 0) {
            let requestIds = [];
            for (let i = 0; i < objSelected.length; i++) {
                const $obj = $(objSelected[i]),
                    verifierId = parseInt($obj.data('verifierid'), 10),
                    statusId = parseInt($obj.data('statusid'), 10);
                if (verifierId === currentUserId) {
                    if (statusId === CurrentRequestStatus.Pending) {
                        requestIds.push($obj.data('requestid'));
                    } else {
                        toastr.error('The status of some requests is not pending.', '', toastOptions());
                        return;
                    }
                } else {
                    toastr.error('You are not permission on some requests.', '', toastOptions());
                    return;
                }
            }

            if (requestIds.length > 0) {
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Verify</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestIds.join(','), RequestCommand.Accept, RoleSystem.Verifier);
                });
            }
        } else {
            toastr.error('Please choose at least one request to continue.', '', toastOptions());
        }
    });

    $('#btnNotVerify').on('click', function () {
        let objSelected = $table.find('tbody input[type=checkbox]:checked');
        if (objSelected.length > 0) {
            let requestIds = [];
            for (let i = 0; i < objSelected.length; i++) {
                const $obj = $(objSelected[i]),
                    verifierId = parseInt($obj.data('verifierid'), 10),
                    statusId = parseInt($obj.data('statusid'), 10);
                if (verifierId === currentUserId) {
                    if (statusId === CurrentRequestStatus.Pending) {
                        requestIds.push($obj.data('requestid'));
                    } else {
                        toastr.error('The status of some requests is not pending.', '', toastOptions());
                        return;
                    }
                } else {
                    toastr.error('You are not permission on some requests.', '', toastOptions());
                    return;
                }
            }

            if (requestIds.length > 0) {
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Not Verify</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestIds.join(','), RequestCommand.NotAccept, RoleSystem.Verifier);
                });
            }
        } else {
            toastr.error('Please choose at least one request to continue.', '', toastOptions());
        }
    });

    $('#btnApprove').on('click', function () {
        let objSelected = $table.find('tbody input[type=checkbox]:checked');
        if (objSelected.length > 0) {
            let requestIds = [];
            for (let i = 0; i < objSelected.length; i++) {
                const $obj = $(objSelected[i]),
                    approverId = parseInt($obj.data('approverid'), 10),
                    statusId = parseInt($obj.data('statusid'), 10);
                if (approverId === currentUserId) {
                    if (statusId === CurrentRequestStatus.Pending || statusId === CurrentRequestStatus.Verified) {
                        requestIds.push($obj.data('requestid'));
                    } else {
                        toastr.error('The status of some requests is not pending or verified.', '', toastOptions());
                        return;
                    }
                } else {
                    toastr.error('You are not permission on some requests.', '', toastOptions());
                    return;
                }
            }

            if (requestIds.length > 0) {
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Approve</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestIds.join(','), RequestCommand.Approve, RoleSystem.Approver);
                });
            }
        } else {
            toastr.error('Please choose at least one request to continue.', '', toastOptions());
        }
    });

    $('#btnReject').on('click', function () {
        let objSelected = $table.find('tbody input[type=checkbox]:checked');
        if (objSelected.length > 0) {
            let requestIds = [];
            for (let i = 0; i < objSelected.length; i++) {
                const $obj = $(objSelected[i]),
                    approverId = parseInt($obj.data('approverid'), 10),
                    statusId = parseInt($obj.data('statusid'), 10);
                if (approverId === currentUserId) {
                    if (statusId === CurrentRequestStatus.Pending || statusId === CurrentRequestStatus.Verified) {
                        requestIds.push($obj.data('requestid'));
                    } else {
                        toastr.error('The status of some requests is not pending or verified.', '', toastOptions());
                        return;
                    }
                } else {
                    toastr.error('You are not permission on some requests.', '', toastOptions());
                    return;
                }
            }

            if (requestIds.length > 0) {
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reject</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestIds.join(','), RequestCommand.Reject, RoleSystem.Approver);
                });
            }
        } else {
            toastr.error('Please choose at least one request to continue.', '', toastOptions());
        }
    });

    $('form.filter #btnExcel').on('click', function (event) {
        event.preventDefault();

        blockUI();
        axios({
            url: resolveClientUrl('Overtime/ExportMemberOvertime'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename=(.+)/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });

    let renderIconOk = function (opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="processed"><i class="fa fa-check"></i>' + item.name + '</span>');
        return '';
    };
    let renderIconRemove = function (opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="void"><i class="fa fa-times"></i>' + item.name + '</span>');
        return '';
    };
    let processCommand = function (command, options) {
        const eventElement = options.$trigger,
            requestId = parseInt(eventElement.attr('requestid'), 10),
            roleId = parseInt(eventElement.attr('roleid'), 10);
        switch (command) {
            case 'Accept':
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Verify</strong> this request?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestId, RequestCommand.Accept, roleId);
                });
                break;
            case 'NotAccept':
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Not Verify</strong> this request?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestId, RequestCommand.NotAccept, roleId);
                });
                break;
            case 'Approve':
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Approve</strong> this request?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestId, RequestCommand.Approve, roleId);
                });
                break;
            case 'Reject':
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reject</strong> this request?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestId, RequestCommand.Reject, roleId);
                });
                break;
            case 'Reset':
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reset</strong> this request?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestId, RequestCommand.Reset, roleId);
                });
                break;
        }
    };
    let bindMenu = function (element, menu) {
        let listItem = {};
        switch (menu) {
            case 'verifierMenu':
                listItem = {
                    'Accept': { name: 'Verified', icon: renderIconOk },
                    'NotAccept': { name: 'Not Verified', icon: renderIconRemove }
                };
                break;
            case 'approverMenu':
                listItem = {
                    'Approve': { name: 'Approve', icon: renderIconOk },
                    'Reject': { name: 'Reject', icon: renderIconRemove }
                };
                break;
            case 'resetMenu':
                listItem = {
                    'Reset': { name: 'Reset', icon: renderIconRemove }
                };
                break;
        }

        $.contextMenu({
            selector: element,
            trigger: 'left',
            callback: processCommand,
            items: listItem
        });
    };

    bindMenu('.approver-pending', 'approverMenu');
    bindMenu('.approver-approved', 'resetMenu');
    bindMenu('.approver-rejected', 'resetMenu');
    bindMenu('.verifier-pending', 'verifierMenu');
    bindMenu('.verifier-verified', 'resetMenu');
    bindMenu('.verifier-notverified', 'resetMenu');
});