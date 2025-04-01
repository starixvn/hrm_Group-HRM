import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-contextmenu/dist/jquery.contextMenu';
import '../../../libs/monthPicker/monPickr-0.0.3';
import toastr from 'toastr';
import axios from 'axios';
import { stringify, parse } from 'qs';

import { axiosError, floatThead, toastOptions, resolveClientUrl, blockUI, unblockUI, downloadFile, sortTable } from '../../shared/common.functions';
import { RequestCommand, CurrentRequestStatus, RoleSystem } from '../../shared/constants';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';

let $confirmModal = $('#confirmModal'),
    $btnConfirm = $confirmModal.find('#btnConfirm'),
    $table = $('#listEmployeeOvertime');

function query() {
    blockUI();
    axios({
        url: resolveClientUrl('overtime/ListEmployeeOvertime'),
        method: 'POST',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        params: parse($('form.filter').serialize()),
        responseType: 'text'
    }).then(response => {
        $('#listEmployeeOvertime').html(response.data);
        floatThead('table#data');
        initSortTable();
        checkAll();
        voidRequest();
        processRequest();
    }).catch(axiosError).then(() => unblockUI());
}

function initSortTable() {
    $('th.col-sorter').on('click', function () {
        sortTable('#data', this);
    });
}

function search() {
    $('#btnSearch').on('click', function () {
        const statusId = parseInt($('#StatusId').val(), 10),
            month = $('#Month').val();

        if ((statusId === 0 || statusId === CurrentRequestStatus.Processed) && month === '') {
            toastr.error('Please select a month to search.', '', toastOptions());
        } else {
            $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
            query();
        }
    });
}

function checkAll() {
    $('#ckbAll').on('change', function (event) {
        $table.find('tbody input[type=checkbox]').prop('checked', event.target.checked);
    });
}

function onUpdateRequestStatus(requestIds, command) {
    axios({
        url: resolveClientUrl('overtime/EditMonthlyRequestStatus'),
        method: 'POST',
        data: stringify({ requestIds, command, roleId: RoleSystem.HR }),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    }).then(response => {
        let data = response.data;
        if (data.IsSuccessful) {
            toastr.success('Updated successfully!', '', toastOptions());
            $confirmModal.modal('hide');
            setTimeout(function () { $('#btnSearch').trigger('click'); }, 1000);
        } else {
            toastr.error(data.Message, '', toastOptions());
        }
    }).catch(axiosError);
}

function checkInvalidOvertime(requestIds) {
    axios({
        url: resolveClientUrl('overtime/CheckInvalidOvertime'),
        method: 'POST',
        data: stringify({ requestIds: requestIds }),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    }).then(response => {
        let isInvalid = response.data;
        if (!isInvalid) {
            onUpdateRequestStatus(requestIds, RequestCommand.Process);
        } else {
            $confirmModal.find('.modal-body')
                .html('There are some overtimes are invalid. Are you sure you want to <strong>Process</strong> this request(s)?');
            $confirmModal.modal('show');
            $btnConfirm.off('click');
            $btnConfirm.on('click', function () {
                onUpdateRequestStatus(requestIds, RequestCommand.Process);
            });
        }
    }).catch(axiosError);
}

function processRequest() {
    $('#btnProcess').on('click', function () {
        let objSelected = $table.find('tbody input[type=checkbox]:checked');
        if (objSelected.length > 0) {
            let requestIds = [];
            for (let i = 0; i < objSelected.length; i++) {
                let $obj = $(objSelected[i]);
                if (parseInt($obj.data('statusid'), 10) === CurrentRequestStatus.Approved) {
                    requestIds.push($obj.data('requestid'));
                } else {
                    toastr.error('The status of some requests is not approved.', '', toastOptions());
                    return;
                }
            }

            if (requestIds.length > 0) {
                $btnConfirm.off('click');
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Process</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    $confirmModal.modal('hide');
                    checkInvalidOvertime(requestIds.join(','));
                });
            }
        } else {
            toastr.error('Please choose at least one request to continue.', '', toastOptions());
        }
    });
}

function voidRequest() {
    $('#btnVoid').on('click', function () {
        let objSelected = $table.find('tbody input[type=checkbox]:checked');
        if (objSelected.length > 0) {
            let requestIds = [];
            for (let i = 0; i < objSelected.length; i++) {
                let $obj = $(objSelected[i]);
                if (parseInt($obj.data('statusid'), 10) === CurrentRequestStatus.Approved) {
                    requestIds.push($obj.data('requestid'));
                } else {
                    toastr.error('The status of some requests is not approved.', '', toastOptions());
                    return;
                }
            }

            if (requestIds.length > 0) {
                $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Void</strong> this request(s)?');
                $confirmModal.modal('show');
                $btnConfirm.on('click', function () {
                    onUpdateRequestStatus(requestIds.join(','), RequestCommand.Void);
                });
            }
        } else {
            toastr.error('Please choose at least one request to continue.', '', toastOptions());
        }
    });
}

function sendMail() {
    $('#lkSendMail').on('click', function () {
        let $reminderMailModal = $('#reminderMailModal');
        $reminderMailModal.modal('show');

        $('#btnSendMail').on('click', function () {
            $reminderMailModal.modal('hide');
            axios({
                url: resolveClientUrl('overtime/ReminderClaimOvertime'),
                method: 'POST',
                data: stringify({
                    subject: $('#txtSubject').val().trim(),
                    content: $('#txtContent').val().trim()
                }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                toastr.success('Send Successfully!', '', toastOptions());
            }).catch(axiosError);
        });
    });
}

function exportExcel() {
    $('form.filter #btnExcel').on('click', function (event) {
        event.preventDefault();

        blockUI();
        axios({
            url: resolveClientUrl('Overtime/Export'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename=([^;]+)/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
}

function renderIconOk(opt, $itemElement, itemKey, item) {
    $itemElement.html('<span class="processed"><i class="fa fa-check"></i>' + item.name + '</span>');
    return '';
}

function renderIconRemove(opt, $itemElement, itemKey, item) {
    $itemElement.html('<span class="void"><i class="fa fa-times"></i>' + item.name + '</span>');
    return '';
}

function processCommand(command, options) {
    const eventElement = options.$trigger,
        requestId = parseInt(eventElement.attr('requestid'), 10);
    $btnConfirm.unbind('click');
    switch (command) {
        case 'Process':
            $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Process</strong> this request?');
            $confirmModal.modal('show');
            $btnConfirm.on('click', function () {
                checkInvalidOvertime(requestId);
            });
            break;
        case 'Void':
            $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Void</strong> this request?');
            $confirmModal.modal('show');
            $btnConfirm.on('click', function () {
                onUpdateRequestStatus(requestId, RequestCommand.Void);
            });
            break;
        case 'Reset':
            $confirmModal.find('.modal-body').html('Are you sure you want to <strong>Reset</strong> this request?');
            $confirmModal.modal('show');
            $btnConfirm.on('click', function () {
                onUpdateRequestStatus(requestId, RequestCommand.Reset);
            });
            break;
    }
}

function bindMenu(element, menu) {
    let listItem = {};
    switch (menu) {
        case 'hrMenu':
            listItem = {
                'Process': { name: 'Process', icon: renderIconOk },
                'Void': { name: 'Void', icon: renderIconRemove },
                'Reset': { name: 'Reset', icon: renderIconRemove }
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
}

$(document).ready(function () {
    let $monthPicker = $('#MonthControl');
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

    query();
    sendMail();
    search();
    exportExcel();

    bindMenu('.hr-pending', 'hrMenu');
    bindMenu('.hr-processed', 'resetMenu');
    bindMenu('.hr-void', 'resetMenu');
});