import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    $('#Employee').autocomplete({
        source: function (request, response) {
            axios({
                url: resolveClientUrl('overtime/GetActingEmployees'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    query: request.term
                }
            }).then(res => {
                response($.map(res.data, function (item) {
                    return {
                        label: item.nickName + ' (' + item.fullName + ')',
                        value: item.ownerId
                    };
                }));
            }).catch(axiosError);
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
                    $('#EmployeeId').val('0');
                }
            }
        }
    });
});