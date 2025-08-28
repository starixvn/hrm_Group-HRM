import axios from 'axios';

import { axiosError, resolveClientUrl } from './common.functions';

$(document).ready(function () {
    $('#Employee').autocomplete({
        source: function (request, response) {
            axios({
                url: resolveClientUrl('employee/getmemberbymanager'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    keyword: request.term,
                    companyId: $('#CompanyId').val() || null,
                },
            })
                .then(res => {
                    response($.map(res.data, function (item) {
                            return {
                                label:
                                    item.nickName + ' (' + item.fullName + ')',
                                value: item.id,
                            };
                        })
                    );
                })
                .catch(axiosError);
        },
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmployeeId').val(ui.item.value);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#Employee').val(ui.item.label);
                $('#EmployeeId').val(ui.item.value);
            } else {
                if ($('#Employee').val() === '') {
                    $('#EmployeeId').val('');
                } else {
                    if ($('#EmployeeId').val() === '') {
                        $('#EmployeeId').val('0');
                    }
                }
            }
        },
    });
});
