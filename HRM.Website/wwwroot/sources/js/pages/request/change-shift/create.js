import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery.are-you-sure/jquery.are-you-sure';
import toastr from 'toastr';
import axios from 'axios';
import moment from 'moment';

import {
    axiosError, blockUI, unblockUI, initDatepicker,
    showValidationSummaryErrors, hideValidationSummaryErrors, toastOptions, resolveClientUrl
} from '../../../shared/common.functions';
import { RequestType, RoleSystem, ChangeShiftType } from '../../../shared/constants';

$(document).ready(function () {
    let domElements = {
        $employeeId: $('#EmployeeId'),
        $typeId: $('#TypeId'),
        $fromDate: $('#FromDate'),
        $toDate: $('#ToDate'),
        $verifierId: $('#VerifierId'),
        $approverId: $('#ApproverId'),
        $remark: $('#Remark'),
        $btnSubmit: $('#btnSubmit')
    };
    let getDataAfterChangeEmployee = function () {
        const employeeId = $('#EmployeeId').val();
        const promises = [
            axios({
                url: resolveClientUrl('request/GetVerifiers'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.ChangeShift
                }
            }),
            axios({
                url: resolveClientUrl('request/GetApprovers'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    employeeId: employeeId,
                    requestType: RequestType.ChangeShift
                }
            })
        ];

        Promise.all(promises)
            .then(([verifierResponse, approverResponse]) => {
                const verifiers = verifierResponse.data,
                    approvers = approverResponse.data;
                let options = '';
                for (let i = 0; i < verifiers.length; i++) {
                    options += "<option value='" + verifiers[i].ownerId + "'>" + verifiers[i].nickName + " (" + verifiers[i].fullName + ")</option>";
                }
                domElements.$verifierId.html(options);

                options = '';
                for (var i = 0; i < approvers.length; i++) {
                    options += "<option value='" + approvers[i].ownerId + "'>" + approvers[i].nickName + " (" + approvers[i].fullName + ")</option>";
                }
                domElements.$approverId.html(options);
            }).catch(axiosError);
    };
    let getChangeShiftDetails = function () {
        blockUI();
        axios({
            url: resolveClientUrl('changeshift/GetChangeShiftDetail'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                employeeId: domElements.$employeeId.val(),
                fromDate: domElements.$fromDate.val(),
                toDate: domElements.$toDate.val(),
                typeId: domElements.$typeId.val()
            },
            responseType: 'text'
        }).then(response => {
            $('#changeShiftDetails').html(response.data);
            initDatepicker();
            $('#tblchangeshift input.datepicker').off('change').on('change', function () {
                $('#tblchangeshift input.datepicker').each(function () {
                    let disabledDate = $(this).attr('original-disabledDate');
                    $('#tblchangeshift input.datepicker').not(this).each(function () {
                        if ($(this).val) {
                            disabledDate = disabledDate + ',' + moment($(this).val()).format('YYYY/MM/DD');
                        }
                    });

                    $(this).attr('disabledDate', disabledDate);
                });
            });

            if ($('#tblchangeshift tbody tr').length > 0) {
                domElements.$btnSubmit.prop('disabled', false);
            } else {
                domElements.$btnSubmit.prop('disabled', true);
            }
        }).catch(axiosError).then(() => unblockUI());
    };
    let validateForm = function () {
        let isValid = true,
            errorMessages = [];
        const employeeId = parseInt(domElements.$employeeId.val(), 10),
            typeId = parseInt(domElements.$typeId.val(), 10),
            fromDate = moment(domElements.$fromDate.val()),
            toDate = moment(domElements.$toDate.val()),
            approverId = domElements.$approverId.val(),
            verifierId = domElements.$verifierId.val();
        let files = document.getElementById('file').files,
            allowedExtensions = /(\.jpg|\.png|\.jpeg|\.pdf|\.doc|\.docx|\.xls|\.xlsx|\.txt)$/i;

        if (employeeId === 0) {
            errorMessages.push('Employee field is required.');
        }

        if (!$('#IsTopManager').prop('checked')) {
            if (verifierId === null || verifierId === '') {
                errorMessages.push('Verified by field is required.');
            }
            if (approverId === null || approverId === '') {
                errorMessages.push('Approved by field is required.');
            }
        }

        if (!toDate.isSameOrAfter(fromDate)) {
            errorMessages.push('To Date must be greater than or equal From Date.');
        }

        if ($.trim(domElements.$remark.val()).length === 0) {
            errorMessages.push('Remark field is required.');
        }

        if (typeId === ChangeShiftType.MoveOffDay) {
            const moveToDates = $('#tblchangeshift input.datepicker').map(function () {
                if (!$(this).val()) { return $(this).val(); }
            });

            if (moveToDates.length > 0) {
                errorMessages.push('Move To Date field is required.');
            }
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

    initDatepicker();
    getChangeShiftDetails();

    $('#Employee').autocomplete({
        source: function (request, response) {
            axios({
                url: resolveClientUrl('request/GetEmployeesByActingUser'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    role: RoleSystem.OnBehalfRequest,
                    requestType: RequestType.ChangeShift,
                    query: request.term
                }
            }).then(rs => {
                response($.map(rs.data, function (item) {
                    return {
                        label: item.nickName + ' (' + item.fullName + ')',
                        value: item.ownerId,
                        isTopManager: item.isTopManager
                    };
                }));
            }).catch(axiosError);
        },
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
            $('#IsTopManager').prop('checked', ui.item.isTopManager);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
            $('#IsTopManager').prop('checked', ui.item.isTopManager);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#Employee').val(ui.item.label);
                $('#EmployeeId').val(ui.item.value);
                $('#IsTopManager').prop('checked', ui.item.isTopManager);
                getDataAfterChangeEmployee();
                domElements.$fromDate.trigger('change');
            } else {
                $('#EmployeeId').val('0');
            }
        }
    });

    domElements.$typeId.on('change', function () {
        getChangeShiftDetails();
    });
    domElements.$fromDate.on('change', function () {
        getChangeShiftDetails();
    });
    domElements.$toDate.on('change', function () {
        getChangeShiftDetails();
    });

    domElements.$btnSubmit.on('click', function () {
        hideValidationSummaryErrors(false);
        const typeId = parseInt(domElements.$typeId.val(), 10);

        let details = [];
        $('#tblchangeshift tbody tr').each(function () {
            let $tr = $(this);
            details.push({
                onDate: $tr.find('.onDate').val(),
                fromShiftId: $tr.find('.fromShiftId').val(),
                toShiftId: $tr.find('.toShiftId').val(),
                remark: $.trim($tr.find('.remark').val()),
                toDate: $tr.find('.moveToDate').val()
            });
        });
        const model = {
            employeeId: domElements.$employeeId.val(),
            typeId: typeId,
            fromDate: domElements.$fromDate.val(),
            toDate: domElements.$toDate.val(),
            remark: $.trim(domElements.$remark.val()),
            verifierId: domElements.$verifierId.val(),
            approverId: domElements.$approverId.val(),
            details: details
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
            fd.append('model', JSON.stringify(model));
            fd.append('details', JSON.stringify(details));
            var xhr = new XMLHttpRequest();
            xhr.addEventListener('load', uploadComplete, false);
            xhr.open('POST', resolveClientUrl('ChangeShift/Create'));
            xhr.send(fd);
        }
    });

    function uploadComplete(evt) {
        unblockUI();
        domElements.$btnSubmit.prop('disabled', false);

        const data = JSON.parse(evt.target.response);
        if (data.isSuccessful) {
            $('form').removeClass('dirty');
            toastr.success('Create successfully!', '', toastOptions());
            setTimeout(function () { location.href = resolveClientUrl('changeshift/employeeview'); }, 3000);
        } else {
            toastr.error(data.message, '', toastOptions());
        }
    }

    $('form').areYouSure();
});