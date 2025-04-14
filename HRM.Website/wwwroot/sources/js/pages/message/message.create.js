import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import 'chosen-js/chosen.jquery';
import '../../shared/multiselect-tree-view';
import axios from 'axios';
import {
    initDatepicker, showValidationSummaryErrors, hideValidationSummaryErrors,
    axiosError, resolveClientUrl
} from '../../shared/common.functions';
import '../../date';

function bindingDataIfEditMode() {
    let messageId = $('#messageId').val(),
        messageSubject = $('#messageSubject').val(),
        messageContents = $('#messageContents').val(),
        messageFromDate = $('#messageFromDate').val(),
        messageToDate = $('#messageToDate').val(),
        messageEmployeeIds = $('#messageEmployeeIds').val(),
        messageTeamIds = $('#messageTeamIds').val(),
        messageCategory = $('#messageCategory').val();

    $('#Subject').val(messageSubject);
    $('#Contents').val(messageContents);
    $('#MessageId').val(messageId);
    $('#category').val(messageCategory);

    if (messageFromDate) {
        var newFromDate = new Date(messageFromDate);
        $('#FromDate').val($.datepicker.formatDate('d M, yy', newFromDate));
    }
    if (messageToDate) {
        var newToDate = new Date(messageToDate);
        $('#ToDate').val($.datepicker.formatDate('d M, yy', newToDate));
    }
    if (messageEmployeeIds) {
        $('#Employees').val(messageEmployeeIds.split(','));
    }
    if (messageTeamIds) {
        $('#TeamIds').val(messageTeamIds);
    }
};

function initSearchEmployee() {
    const employeeIds = $('#EmployeeIds').val();

    if (employeeIds) {
        $('#Employees').val(employeeIds.split(','));
    }

    $('#Employees').chosen();

    $('#Employees').on('change', function () {
        const id = $(this).val(),
            employeeIds = id ? id.join(',') : '';
        $('#EmployeeIds').val(employeeIds);
    });
}

function initSearchTeam() {
    axios({
        url: resolveClientUrl('Team/GetOrgChart'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    }).then(response => {
        $('#Groups').multiSelectTreeView({
            data: response.data,
            selectedItems: $('#TeamIds').val(),
            hasSearchBox: true
        });
    }).catch(axiosError);
}

function GetSelectedEmployeeEmails() {
    let result = '';
    let employeeEmails = $('#Employees option:selected');

    if (employeeEmails) {
        for (var i = 0; i < employeeEmails.length; i++) {
            var item = employeeEmails[i],
                email = $(item).attr('email');

            if (email) {
                if (!result) {
                    result = email;
                }
                else {
                    result += ',' + email;
                }
            }
        }
    }

    return result;
}

function GetSelectedGroupEmails() {
    let selectedGroups = $('#Groups').multiSelectTreeView('getSelectedItems');

    let result = '';
    let selectedArr = selectedGroups.split(',');
    for (var i = 0; i < selectedArr.length; i++) {
        if (selectedArr[i]) {
            let email = GroupsCollection.filter(group => group.Id.toString() == selectedArr[i])[0].MailGroup;
            if (email) {
                if (!result) {
                    result = email;
                }
                else {
                    result += ',' + email;
                }
            }
        }
    }

    return result;
}

function getSendToEmails() {
    let employeeEmails = GetSelectedEmployeeEmails();
    let groupsEmail = GetSelectedGroupEmails();

    let result = employeeEmails;

    if (groupsEmail) {
        result = result + ',' + groupsEmail;
    }

    return result;
}

function validateForm() {
    let isValid = true,
        errorMessages = [];

    if (!$('#Subject').val()) {
        errorMessages.push('The Subject field is required!');
    }
    if (!$('#Category').val()) {
        errorMessages.push('The Category field is required!');
    }
    if (!$('#SendToEmail').val()) {
        errorMessages.push('The Send To field is required!');
    }
    if (!$('#Contents').val()) {
        errorMessages.push('The Contents field is required!');
    }

    if (errorMessages.length > 0) {
        $('.validation-summary-valid').find('ul:first').html('');
        showValidationSummaryErrors(errorMessages);
        isValid = false;
    }

    return isValid;
}

function Submit() {
    $('#btnSubmit').on('click', function () {
        //validation
        let selectedGroups = $('#Groups').multiSelectTreeView('getSelectedItems');
        let sendToEmail = getSendToEmails();
        var employeeIds = $('#Employees').val();

        $('#SendToEmail').val(sendToEmail);
        $('#TeamIds').val(selectedGroups);
        $('#EmployeeIds').val(employeeIds);

        let isValid = validateForm();

        let form = $('#CreateForm');
        let $error = $('#error');

        if (isValid) {
            hideValidationSummaryErrors(true);
            axios({
                url: form[0].action,
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                data: form.serialize()
            }).then(response => {
                if (response.data.Code === 0) {
                    $('#messageInfo').show();
                    setTimeout(function () {
                        document.location = resolveClientUrl('Message/Index');
                    }, 2000);
                }
                else {
                    $error.html(response.data.Message);
                    $error.show();
                }
            }).catch(axiosError);
        }
    });
}

$(document).ready(function () {
    bindingDataIfEditMode();
    initSearchEmployee();
    initSearchTeam();
    initDatepicker();
    Submit();
});