import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'chosen-js/chosen.jquery';
import moment from 'moment';
import axios from 'axios';
import { stringify } from 'qs';
import toastr from 'toastr';
import '../../../libs/birthdaypicker';

import '../../shared/multiselect-tree-view';
import { axiosError, confirm, alert, initDatepicker, toastOptions, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    function bindDisciplineRole(onload) {
        let combineDisciplineRole = $('#Roles2Array3');
        if ($('#Roles2Array8').prop('checked')) {
            combineDisciplineRole.prop('checked', true).prop('readonly', 'readonly');
        } else if (!onload) {
            combineDisciplineRole.prop('checked', false).prop('readonly', false);
        }
    }

    function uploadComplete(evt) {
        if (evt.target.status === 200) {
            const xhr = evt.target;
            const responseUrl = JSON.parse(xhr.responseText);
            if (!responseUrl) return;
            $('#avatar').attr('src', responseUrl + '?f=' + Math.random());
        }
    }

    function uploadFailed() {
        toastr.error('Upload fail, please try again!', '', toastOptions());
    }

    function upload() {
        const file = document.getElementById('file').files[0];
        if (file === undefined) {
            return;
        }

        if (file.size / 1024 > 5120) {
            alert('File is to big (' + file.size / 1024 + 'KB) to upload. Maximum = 5120KB');
            return;
        }

        const pass = /\.(gif|jpe?g|png)$/i.test(file.name);
        if (!pass) {
            alert('Please choose an image.');
            return;
        }

        let fd = new FormData();
        fd.append('code', $('#Code').val());
        fd.append('file', file);

        let xhr = new XMLHttpRequest();
        xhr.addEventListener('load', uploadComplete, false);
        xhr.addEventListener('error', uploadFailed, false);
        xhr.open('POST', resolveClientUrl('employee/uploadavatar'));
        xhr.send(fd);
    }

    function resetPassword() {
        const employeeId = $('#Id').val();
        const userId = $('#UserId').val();
        const nickname = $('#NickName').val();
        confirm('Are you sure you want to reset ' + nickname + "'s password?", function () {
            axios({
                url: resolveClientUrl('Account/ResetPassword'),
                method: 'POST',
                data: stringify({ employeeId: employeeId, userId: userId }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toastr.success('Reset password successfully!', '', toastOptions());
                } else {
                    toastr.error(response.Message, '', toastOptions());
                }
            }).catch(axiosError);
        });
    }

    function showExpatApplicableHoliday() {
        const value = $('#IsLocal').val();
        if (value === 'true') {
            $('#expatHoliday').hide();
        } else {
            $('#expatHoliday').show();
        }
    }

    initDatepicker();
    let $uploadButton = $('#upload');
    $('#CountryCode, #JobTitleId, #SubCompanyId, #JobLevelId, #SalaryStructureId, .allowance-select').chosen();
    
    // Function to collect all selected allowance IDs
    function collectAllowanceIds() {
        const selectedIds = [];
        $('.allowance-select').each(function() {
            const value = $(this).val();
            if (value) {
                if (Array.isArray(value)) {
                    // Multi-select: add all selected values
                    selectedIds.push(...value);
                } else if (value !== '') {
                    // Single select: add the value if not empty
                    selectedIds.push(value);
                }
            }
        });
        return selectedIds.join(',');
    }
    
    // Update hidden field when allowance selections change
    $('.allowance-select').on('change', function() {
        $('#AllowanceItemSettingIds').val(collectAllowanceIds());
    });
    
    // Initialize the hidden field with current selections
    $('#AllowanceItemSettingIds').val(collectAllowanceIds());

    bindDisciplineRole(true);
    $('#Roles2Array8').click(function () {
        bindDisciplineRole(false);
    });

    $('#file').change(function () {
        const filePath = $(this).val();

        if (filePath) {
            $uploadButton.show();
        } else {
            $uploadButton.hide();
        }
    });

    //Birthday picker.
    $('#DOBPicker').birthdaypicker({
        futureDates: true,
        maxYear: new Date().getFullYear(),
        maxAge: 75,
        defaultDate: $('#DateOfBirth').val()
    });

    axios({
        url: resolveClientUrl('Team/GetOrgChart'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    }).then(response => {
        $('#Team').multiSelectTreeView({
            data: response.data,
            noneSelectedText: 'Select a team',
            singleSelection: true,
            selectedItems: $('#Team_Id').val(),
            onSelect: function () {
                $('#Team_Id').val($('#Team').multiSelectTreeView('getSelectedItems'));
            }
        });
    }).catch(axiosError);

    $('#postform').on('click', function () {
        const isAutoApproveRequest = $("input[name='AutoApproveRequest']").prop('checked');
        if (isAutoApproveRequest === true) {
            $('#formEditUser')[0].submit();
        } else {
            const verifier = $('#lstVerifier').text(),
                approver = $('#lstApprover').text();
            if (verifier === null || verifier === '') {
                if (approver === null || approver === '') {
                    confirm('This user does not have any Verifier or Approver! Continue?', function () {
                        $('#formEditUser')[0].submit();
                    });
                } else {
                    confirm('This user does not have any Verifier! Continue?', function () {
                        $('#formEditUser')[0].submit();
                    });
                }
            } else {
                if (approver === null || approver === '') {
                    confirm('This user does not have any Approver! Continue?', function () {
                        $('#formEditUser')[0].submit();
                    });
                } else {
                    $('#formEditUser')[0].submit();
                }
            }
        }
    });

    $('#resetpassword').on('click', function () {
        resetPassword();
    });

    $('#month, #day, #year').change(function () {
        const year = $('#year').val();
        const month = $('#month').val();
        const day = $('#day').val();
        const date = new Date(year, month - 1, day);

        $('#DateOfBirth').val(moment(date).format('DD MMM YYYY'));
    });

    $uploadButton.click(function () {
        upload();
    });

    $('#delete').click(function () {
        confirm('Are you sure you want to delete this employee?', function () {
            const resignedDate = $('#ResignedDate').val();
            if (resignedDate.length === 0) {
                alert('Please input the resigned date first!');
                $('#ResignedDate').focus();
                return;
            }

            axios({
                url: resolveClientUrl('Employee/Delete'),
                method: 'POST',
                data: stringify({ employeeId: $('#Id').val(), resignedDate: resignedDate, userId: $('#UserId').val() }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    document.location = resolveClientUrl('employee/usermanagement');
                } else {
                    toastr.error(data.Message, '', toastOptions());
                }
            }).catch(axiosError);
        });
    });

    let collapse = $('.panel > .panel-heading'),
        itemContent = collapse.next();
    itemContent.show();

    $('.panel > .panel-heading').click(function () {
        let $header = $(this);
        //getting the next element
        let $content = $header.next();
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
            let icon = $header.find('> span.fa');
            if ($(icon).hasClass('fa-chevron-down')) {
                $(icon).removeClass('fa-chevron-down').removeClass('fa-chevron-right').addClass('fa-chevron-right');
            } else if ($(icon).hasClass('fa-chevron-right')) {
                $(icon).removeClass('fa-chevron-right').removeClass('fa-chevron-down').addClass('fa-chevron-down');
            }
        });
    });

    $('#IsLocal').on('change', function () {
        showExpatApplicableHoliday();
    });
    showExpatApplicableHoliday();

    $('input#ResignedDate').removeAttr('readonly');
    $('input#ResignedDate').keyup(function (e) {
        if (e.keyCode === 8 || e.keyCode === 46) {
            $.datepicker._clearDate(this);
        }
    });
});