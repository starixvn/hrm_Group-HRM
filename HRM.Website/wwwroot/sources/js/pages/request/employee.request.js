import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'jquery-contextmenu/dist/jquery.contextMenu';
import 'floatthead/dist/jquery.floatThead';
import toastr from 'toastr';
import axios from 'axios';
import { stringify } from 'qs';

import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import './date.filter';
import {
    axiosError, blockUI, unblockUI, floatThead, confirm, initDatepicker,
    toastOptions, resolveClientUrl, sortTable
} from '../../shared/common.functions';
import { RequestCommand, RoleSystem } from '../../shared/constants';

$(function () {
    function processRequest(requestId, requestTypeId, roleId, command, comment, disciplines, viewType) {
        blockUI();
        const model = {
            requestId: requestId,
            requestTypeId: requestTypeId,
            roleId: roleId,
            command: command,
            comment: comment,
            disciplines: disciplines,
            viewType: viewType
        };

        axios({
            url: resolveClientUrl('request/ProcessRequest'),
            method: 'POST',
            data: stringify({ model }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            unblockUI();
            const data = response.data;
            if (data.isSuccessful) {
                window.location.reload();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    }

    function renderProcessRequestDialog(requestId, requestTypeId, roleId, employeeId, command, title, viewType) {
        $('#dialogProcessRequest').dialog({
            title: title,
            height: 'auto',
            width: 'auto',
            modal: true,
            buttons: [{
                text: 'Confirm',
                class: 'btn btn-primary',
                click: function () {
                    let ignoredDisciplines = [],
                        comment = $(this).find('textarea#Comment').val();

                    $(this).find('table.table tbody tr').each(function () {
                        let $tr = $(this),
                            chkIgnore = $tr.find('.chk-ignore'),
                            chkWeekendLeaveIgnore = $tr.find('.chk-weekend-ignore');

                        if (chkIgnore.length === 1) {
                            ignoredDisciplines.push({
                                WorkDate: chkIgnore.data('workdate'),
                                EmployeeId: employeeId,
                                DisciplineTypeId: chkIgnore.data('disciplinetypeid'),
                                IsIgnored: chkIgnore.prop('checked')
                            });
                        }

                        if (chkWeekendLeaveIgnore.length === 1) {
                            ignoredDisciplines.push({
                                WorkDate: chkWeekendLeaveIgnore.data('workdate'),
                                EmployeeId: employeeId,
                                DisciplineTypeId: 10,
                                IsIgnored: chkWeekendLeaveIgnore.prop('checked')
                            });
                        }
                    });

                    $(this).find('.validation-summary-valid').hide();
                    processRequest(requestId, requestTypeId, roleId, command, comment, ignoredDisciplines, viewType);
                    $(this).dialog('close');
                }
            },
            {
                text: 'Cancel',
                class: 'btn btn-default',
                click: function () {
                    $(this).dialog('close');
                }
            }]
        });
    }

    function getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, command, title, viewType) {
        const model = {
            RequestId: requestId,
            RoleId: roleId,
            RequestTypeId: requestTypeId,
            EmployeeId: employeeId,
            Command: command,
            ViewType: viewType
        };

        blockUI();
        axios({
            method: 'GET',
            url: resolveClientUrl('request/PartialViewProcessRequest'),
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: model,
            responseType: 'text'
        }).then(response => {
            $('#dialogProcessRequest').html(response.data);
            $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });
            renderProcessRequestDialog(requestId, requestTypeId, roleId, employeeId, command, title, viewType);

            let chkIgnoreAll = $('.chk-ignoreAll'),
                chkWeekendLeaveIgnoreAll = $('.chk-weekend-ignoreAll'),
                chkIgnores = $('.chk-ignore'),
                chkWeekendLeaveIgnores = $('.chk-weekend-ignore');

            chkIgnoreAll.on('change', function () {
                const checked = $(this).prop('checked');
                chkIgnores.prop('checked', checked);
            });

            chkWeekendLeaveIgnoreAll.on('change', function () {
                const checked = $(this).prop('checked');
                chkWeekendLeaveIgnores.prop('checked', checked);
            });

            let updateParent = function ($parent, $childs) {
                const $childschecked = $childs.filter(':checked');
                $parent.prop('checked', $childs.length === $childschecked.length);
            };

            $('#dialogProcessRequest').find('table.table tbody tr').each(function () {
                let chkIgnore = $(this).find('.chk-ignore'),
                    chkWeekendLeaveIgnore = $(this).find('.chk-weekend-ignore');

                chkIgnore.on('change', function () {
                    updateParent(chkIgnoreAll, chkIgnores);
                });

                chkWeekendLeaveIgnore.on('change', function () {
                    updateParent(chkWeekendLeaveIgnoreAll, chkWeekendLeaveIgnores);
                });
            });
        }).then(() => unblockUI()).catch(axiosError);
    }

    function processCommand(command, options) {
        const eventElement = options.$trigger,
            requestTypeId = parseInt(eventElement.attr('requestTypeId'), 10),
            requestId = parseInt(eventElement.attr('requestId'), 10),
            roleId = parseInt(eventElement.attr('roleId'), 10),
            employeeId = parseInt(eventElement.attr('employeeId'), 10),
            viewType = eventElement.attr('viewType');
        let title;
        switch (command) {
            case 'Accept':
                title = 'Are you sure you want to verify?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Accept, title);
                break;
            case 'NotAccept':
                title = 'Are you sure you don\'t want to verify?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.NotAccept, title);
                break;
            case 'Approve':
                title = 'Are you sure you want to approve?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Approve, title, viewType);
                break;
            case 'Reject':
                title = 'Are you sure you want to reject?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Reject, title);
                break;
            case 'Process':
                title = 'Are you sure you want to process?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Process, title);
                break;
            case 'Void':
                title = 'Are you sure you want to void?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Void, title);
                break;
            case 'VoidBySA':
                title = 'Are you sure you want to void?';
                getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.VoidBySA, title);
                break;
            case 'Reset':
                confirm('Are you sure you want to reset?', function () {
                    processRequest(requestId, requestTypeId, roleId, RequestCommand.Reset);
                });
                break;
        }
    }

    function processMultipleRequest(commandType) {
        let objSelected = $('table#data tbody input[type=checkbox]:checked');

        switch (objSelected.length) {
            case 0:
                toastr.error('Please choose at least one request to continue.', '', toastOptions());
                return;
            case 1:
                const request = objSelected[0],
                    requestTypeId = parseInt(request.getAttribute('requestTypeId'), 10),
                    requestId = parseInt(request.getAttribute('id'), 10),
                    roleId = parseInt(request.getAttribute('roleId'), 10),
                    employeeId = parseInt(request.getAttribute('employeeId'), 10),
                    viewType = request.getAttribute('viewType');
                let title;
                switch (commandType) {
                    case 'Accept':
                        title = 'Are you sure you want to verify?';
                        getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Accept, title);
                        break;
                    case 'Approve':
                        title = 'Are you sure you want to approve?';
                        getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Approve, title, viewType);
                        break;
                    case 'Process':
                        title = 'Are you sure you want to process?';
                        getProcessRequestDialogContent(requestId, requestTypeId, roleId, employeeId, RequestCommand.Process, title);
                        break;
                }
                return;

            default:
                break;
        }

        switch (commandType) {
            case 'Accept':
                openDialogVerifyMultipleRequest();
                break;
            case 'Approve':
                openDialogApproveMultipleRequest();
                break;
            case 'Process':
                openDialogProcessMultipleRequest();
                break;
        }
    }

    function openDialogApproveMultipleRequest() {
        let objSelected = $('table#data tbody input[type=checkbox]:checked');

        $('#dialogApproveMultipleRequest').dialog({
            title: 'Are you sure you want to approve these requests ?',
            height: 'auto',
            width: 'auto',
            modal: true,
            buttons: [{
                text: 'Confirm',
                class: 'btn btn-primary',
                click: function () {
                    let requests = [],
                        modal = $(this),
                        comment = $(this).find('textarea#Comment').val(),
                        chkIgnore = $(this).find('#chk-ignore-all')

                    objSelected.each(function () {
                        const request = this;
                        const role = request.getAttribute('roleId');

                        if (role != RoleSystem.Approver) {
                            toastr.error('You don\'t have permission to approve some requests', '', toastOptions());
                        }

                        requests.push({
                            RequestId: parseInt(request.getAttribute('Id'), 10),
                            RequestTypeID: parseInt(request.getAttribute('RequestTypeID'), 10)
                        });
                    });

                    $(this).find('.validation-summary-valid').hide();
                    processMultiRequests(requests, comment, RoleSystem.Approver, RequestCommand.Approve, chkIgnore.prop('checked'));
                    modal.dialog('close');
                }
            },
            {
                text: 'Cancel',
                class: 'btn btn-default',
                click: function () {
                    $(this).dialog('close');
                }
            }]
        });
    }

    function openDialogVerifyMultipleRequest() {
        let objSelected = $('table#data tbody input[type=checkbox]:checked');

        $('#dialogProcessMultipleRequest').dialog({
            title: 'Are you sure you want to verify these requests ?',
            height: 'auto',
            width: 'auto',
            modal: true,
            buttons: [{
                text: 'Confirm',
                class: 'btn btn-primary',
                click: function () {
                    let requests = [],
                        modal = $(this),
                        comment = $(this).find('textarea#Comment')[0].value;

                    objSelected.each(function () {
                        const request = this;
                        const role = request.getAttribute('roleId');

                        if (role != RoleSystem.Verifier) {
                            toastr.error('You don\'t have permission to verify some requests', '', toastOptions());
                        }

                        requests.push({
                            RequestId: parseInt(request.getAttribute('Id'), 10),
                            RequestTypeID: parseInt(request.getAttribute('RequestTypeID'), 10)
                        });
                    });

                    processMultiRequests(requests, comment, RoleSystem.Verifier, RequestCommand.Accept);
                    modal.dialog('close');
                }
            },
            {
                text: 'Cancel',
                class: 'btn btn-default',
                click: function () {
                    $(this).dialog('close');
                }
            }]
        });
    }

    function openDialogProcessMultipleRequest() {
        let objSelected = $('table#data tbody input[type=checkbox]:checked');

        $('#dialogProcessMultipleRequest').dialog({
            title: 'Are you sure you want to process these requests ?',
            height: 'auto',
            width: 'auto',
            modal: true,
            buttons: [{
                text: 'Confirm',
                class: 'btn btn-primary',
                click: function () {
                    const requests = [],
                        modal = $(this),
                        comment = $(this).find('textarea#Comment')[0].value;

                    objSelected.each(function () {
                        const request = this;
                        const role = request.getAttribute('roleId');

                        if (role != RoleSystem.HR) {
                            toastr.error('You don\'t have permission to process some requests', '', toastOptions());
                        }

                        requests.push({
                            RequestId: parseInt(request.getAttribute('Id'), 10),
                            RequestTypeID: parseInt(request.getAttribute('RequestTypeID'), 10)
                        });
                    });

                    processMultiRequests(requests, comment, RoleSystem.HR, RequestCommand.Process);
                    modal.dialog('close');
                }
            },
            {
                text: 'Cancel',
                class: 'btn btn-default',
                click: function () {
                    $(this).dialog('close');
                }
            }]
        });
    }

    function processMultiRequests(requests, comment, roleId, command, isIgnoredViolate) {
        blockUI();
        const model = {
            requests: requests,
            comment: comment,
            RoleId: roleId,
            Command: command,
            IsIgnoredViolate: isIgnoredViolate
        };

        axios({
            url: resolveClientUrl('request/ProcessMultipleRequests'),
            method: 'POST',
            data: stringify({ model }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            unblockUI();
            const data = response.data;
            if (data.isSuccessful) {
                window.location.reload();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    }

    function renderIconOk(opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="processed"><i class="fa fa-check"></i>' + item.name + '</span>');
        return '';
    }

    function renderIconRemove(opt, $itemElement, itemKey, item) {
        $itemElement.html('<span class="void"><i class="fa fa-times"></i>' + item.name + '</span>');
        return '';
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
            case 'voidMenu':
                listItem = {
                    'VoidBySA': { name: 'Void', icon: renderIconRemove }
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

    function initEventExportExcel() {
        $('#btnExcel').click(function () {
            const action = $('#ActionName').val() + 'WorkingTimeRequest';
            const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');

            const url = resolveClientUrl('ExportRequest/' + action
                + '?DateOption=' + $('#Date').val()
                + '&TeamIds=' + (teamIds !== '' ? teamIds : ' ')
                + '&Employee=' + $('#Employee').val()
                + '&EmployeeId=' + $('#EmployeeId').val()
                + '&FromDate=' + $('#FromDate').val()
                + '&ToDate=' + $('#ToDate').val()
                + '&StatusId=' + $('#statusId').val()
                + '&RequestType=' + $("input[type='radio'][name='RequestType']:checked").val()
                + '&IsLocal=' + $('#IsLocal').val()
                + '&IsFilterByAffectedDate=' + $('#IsFilterByAffectedDate').val());
            location.href = url;
        });
    }

    function initSortTable() {
        $('th.col-sorter').on('click', function () {
            sortTable('table#data', this);
            let hasCheckBoxColumn = $('#employee-request-body').hasClass('has-check-box-column');
            $('table#data tbody tr').each(function (index) {
                let rowIndex = index + 1;
                $(this).find(`td:eq(${hasCheckBoxColumn ? 1 : 0})`).html(rowIndex++);
            });
        });
    }

    $(document).ready(function () {
        initDatepicker();
        $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });

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

        bindMenu('.hr-actions', 'hrMenu');
        bindMenu('.verifier-actions', 'verifierMenu');
        bindMenu('.approver-actions', 'approverMenu');
        bindMenu('.verifier-actions-reset', 'resetMenu');
        bindMenu('.approver-actions-reset', 'resetMenu');
        bindMenu('.hr-actions-reset', 'resetMenu');
        bindMenu('.sa-void', 'voidMenu');

        $('.viewhistory').click(function () {
            blockUI();
            axios({
                url: $(this).attr('id'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'text'
            }).then(response => {
                $('#dialoghistorycontent').html(response.data);
                $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });
                $('#dialoghistorycontent').dialog({
                    modal: true,
                    height: 'auto',
                    width: 1100,
                    title: 'Request History'
                });
            }).catch(axiosError).then(() => unblockUI());
        });

        $('form').submit(function (e) {
            e.preventDefault();
            const from = new Date($('#FromDate').val());
            const to = new Date($('#ToDate').val());
            if (from > to) {
                alert('From Date must be less than To Date');
                $('#submit').removeClass('disabled');
                $('#submit').val('Submit');
                e.preventDefault();
            } else {
                const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
                const url = resolveClientUrl('request/' + $('#ActionName').val()
                    + '?DateOption=' + $('#Date').val()
                    + '&TeamIds=' + (teamIds === '' ? ' ' : teamIds)
                    + '&Employee=' + $('#Employee').val()
                    + '&EmployeeId=' + $('#EmployeeId').val()
                    + '&FromDate=' + $('#FromDate').val()
                    + '&ToDate=' + $('#ToDate').val()
                    + '&StatusId=' + $('#statusId').val()
                    + '&RequestType=' + $("#RequestType").val()
                    + '&IsLocal=' + $('#IsLocal').val()
                    + '&IsFilterByAffectedDate=' + $('#IsFilterByAffectedDate').val()
                    + '&leaveType=' + $('#LeaveType').val());
                location.href = url;
            }
        });

        $('#ckbAll').on('change', function (event) {
            $('table#data').find('tbody input[type=checkbox]').prop('checked', event.target.checked);
        });

        $('.btnApproveMultiple').on('click', function (event) {
            const commandType = event.target.getAttribute('command-type');
            processMultipleRequest(commandType);
        });

        floatThead('#data');
        initEventExportExcel();
        initSortTable();
    });
});