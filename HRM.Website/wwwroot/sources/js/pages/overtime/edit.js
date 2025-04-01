import 'jquery.are-you-sure/jquery.are-you-sure';
import axios from 'axios';
import { stringify } from 'qs';
import toastr from 'toastr';

import { axiosError, toastOptions, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    let rangeTextBox = function (textbox, min, max) {
        const value = parseFloat(textbox.val());
        if (value % 0.5 === 0) {
            if (value < min || value > max) {
                textbox.val(0);
            }
        } else {
            textbox.val(0);
        }

        textbox.keyup(function (event) {
            if ((event.which !== 46 || $(this).val().indexOf('.') !== -1) && (event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
        });

        textbox.blur(function () {
            if (textbox.val() === '') {
                textbox.val(0);
            }
        });

        return false;
    };

    let calculateTotalOTHours = function (selector) {
        let totalHours = 0;
        selector.each(function () {
            const value = $(this).val().trim();
            if (value !== '') {
                if (!isNaN(value) && value.lenght !== 0) {
                    totalHours += parseFloat(value);
                }
            }
        });

        return totalHours;
    };

    let calculateOTHours = function () {
        let totalHours = calculateTotalOTHours($('.cell-overtime input.normalHours'));
        $('#sumnormal').text(totalHours.toFixed(2));

        totalHours = calculateTotalOTHours($('.cell-overtime input.offHours'));
        $('#sumoff').text(totalHours.toFixed(2));

        totalHours = calculateTotalOTHours($('.cell-overtime input.publicHours'));
        $('#sumpublic').text(totalHours.toFixed(2));

        totalHours = calculateTotalOTHours($('.cell-overtime input.specialHours'));
        $('#sumspecial').text(totalHours.toFixed(2));
    };

    let validation = function () {
        let isValid = true;
        const verifierId = $('#VerifierId').val();
        const approverId = $('#ApproverId').val();

        if (verifierId === '' && approverId === '') {
            toastr.error('Please select Verifier and Approver.', '', toastOptions());
            isValid = false;
        } else if (verifierId === null || verifierId === '') {
            toastr.error('Please select a Verifier.', '', toastOptions());
            isValid = false;
        } else if (approverId === null || approverId === '') {
            toastr.error('Please select an Approver.', '', toastOptions());
            isValid = false;
        } else {
            $('input.remark').each(function () {
                let row = $(this).closest('tr');
                const workDate = row.find('.workdate').data('workdate');
                const workingHours = parseFloat($('#WorkingHours').val());
                const totalHours = parseFloat(row.find('.totalHours').html().trim());

                if (row.find('input.normalHours').length > 0
                    || (row.find('input.specialHours').length > 0 && row.find('input.specialHours.specialPH').length === 0)) {
                    const maxOTHours = parseFloat((totalHours - workingHours + 0.09).toFixed(2));
                    const normalHours = parseFloat(row.find('input.normalHours').val()) || 0,
                        specialHours = parseFloat(row.find('input.specialHours').val()) || 0;

                    if (normalHours !== 0 && specialHours !== 0) {
                        toastr.error('Can not claim both normal and special.', '', toastOptions());
                        isValid = false;
                    } else {
                        if (normalHours > maxOTHours || specialHours > maxOTHours) {
                            toastr.error(`Invalid input on ${workDate}`, '', toastOptions());
                            isValid = false;
                        }
                    }
                } else {
                    const maxOTHours = parseFloat((totalHours + 0.09).toFixed(2));
                    const offHours = parseFloat(row.find('input.offHours').val()) || 0,
                        publicHours = parseFloat(row.find('input.publicHours').val()) || 0,
                        specialHours = parseFloat(row.find('input.specialHours').val()) || 0;

                    if (offHours > maxOTHours || publicHours > maxOTHours || specialHours > maxOTHours) {
                        toastr.error(`Invalid input on ${workDate}`, '', toastOptions());
                        isValid = false;
                    }
                }
            });
        }

        return isValid;
    };

    $('form').areYouSure();

    $('.cell-overtime input').on('keyup', function () {
        let $this = $(this);
        let row = $this.closest('tr');
        const workingHours = parseFloat($('#WorkingHours').val());
        const totalHours = parseFloat(row.find('.totalHours').html().trim());

        if ($this.hasClass('normalHours') || ($this.hasClass('specialHours') && !$this.hasClass('specialPH'))) {
            if (totalHours >= workingHours) {
                const limit = parseFloat((totalHours - workingHours + 0.09).toFixed(2));
                rangeTextBox($this, 0, limit);
            } else {
                $this.val(0);
            }
        } else {
            const limit = parseFloat((totalHours + 0.09).toFixed(2));
            rangeTextBox($this, 0, limit);
        }

        calculateOTHours();
    });

    $('#btnSave').on('click', function () {
        const isValid = validation();
        if (isValid) {
            let model = {
                employeeId: $('#EmployeeId').val(),
                month: $('#Month').val(),
                verifierId: $('#VerifierId').val(),
                approverId: $('#ApproverId').val(),
                dailyOvertimes: []
            };

            $('input.remark').each(function () {
                let row = $(this).closest('tr');
                model.dailyOvertimes.push({
                    workDate: row.find('.workdate').data('workdate'),
                    shiftId: row.find('.shift').data('shiftid'),
                    timeIn: row.find('.timeIn').html().trim(),
                    timeOut: row.find('.timeOut').html().trim(),
                    actualWorkingHours: row.find('.totalHours').html().trim(),
                    normalHours: row.find('input.normalHours').val(),
                    offHours: row.find('input.offHours').val(),
                    publicHours: row.find('input.publicHours').val(),
                    specialHours: row.find('input.specialHours').val(),
                    remark: $(this).val()
                });
            });

            axios({
                url: resolveClientUrl('overtime/Edit'),
                method: 'POST',
                data: stringify({ model }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    $('form').removeClass('dirty');
                    toastr.success('Updated Successfully!', '', toastOptions());
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        }
    });
});