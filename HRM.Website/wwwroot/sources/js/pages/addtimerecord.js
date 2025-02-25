import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import axios from 'axios';

import { axiosError, initDatepicker, resolveClientUrl } from '../shared/common.functions';

$(document).ready(function () {
    initDatepicker();

    $('#Employee').autocomplete({
        source: function (request, response) {
            axios({
                url: resolveClientUrl('employee/getemployees'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    query: request.term,
                    enable: true,
                    isDelete: false,
                    $top: 10
                }
            }).then(rs => {
                response($.map(rs.data, function (item) {
                    return {
                        label: item.DisplayName,
                        value: item.Id
                    };
                }));
            }).catch(axiosError);
        },
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            $('#EmpId').val(ui.item.value);
            $('#Employee').val(ui.item.label);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#EmpId').val(ui.item.value);
            $('#Employee').val(ui.item.label);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                $('#EmpId').val(ui.item.value);
                $('#Employee').val(ui.item.label);
            } else {
                $('#EmpId').val('');
            }
        }
    });
});