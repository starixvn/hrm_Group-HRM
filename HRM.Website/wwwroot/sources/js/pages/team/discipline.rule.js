import axios from 'axios';
import { stringify } from 'qs';
import toastr from 'toastr';

import { axiosError, toastOptions, resolveClientUrl } from '../../shared/common.functions';

let disciplineRule = (function ($) {
    let $table,
        $btnSubmit;

    function registerEventSubmit() {
        $btnSubmit.on('click', function () {
            let teamId = $('#TeamId').val(),
                model = [],
                els = $table.find('tbody tr'),
                totalItems = els.length;

            for (let i = 0; i < totalItems; i++) {
                let el = $(els[i]),
                    disciplineTypeId = el.data('disciplinetypeid'),
                    isApplyForLocal = el.find('input[type=checkbox][name=local]').prop('checked'),
                    isApplyForExpat = el.find('input[type=checkbox][name=expat]').prop('checked');

                model.push({ disciplineTypeId, isApplyForLocal, isApplyForExpat });
            }

            if (totalItems > 0) {
                axios({
                    url: resolveClientUrl('team/EditDisciplineRule'),
                    method: 'POST',
                    data: stringify({ teamId: teamId, disciplineRules: model }),
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'Content-Type': 'application/x-www-form-urlencoded'
                    }
                }).then(response => {
                    let data = response.data;

                    if (data.isSuccessful) {
                        toastr.success('Updated Successfully!', '', toastOptions());
                    } else {
                        toastr.error(data.message, '', toastOptions());
                    }
                }).catch(axiosError);
            }
        });
    }

    function initModule() {
        $table = $('#tabledata');
        $btnSubmit = $('#btnSubmit');

        $('#ckbAllLocal').on('change', function () {
            $('.ckb-local').prop('checked', $(this).prop('checked'));
        });

        $('#ckbAllExpat').on('change', function () {
            $('.ckb-expat').prop('checked', $(this).prop('checked'));
        });

        registerEventSubmit();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    disciplineRule.initModule();
});