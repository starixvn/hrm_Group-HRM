import $ from 'jquery/dist/jquery';
import axios from 'axios';
import 'jquery-ui-bundle/jquery-ui';
import { resolveClientUrl, confirm, axiosError, alert } from '../../shared/common.functions';

$(function () {
    function initDeleteEvent() {
        $('.icon-delete').on('click', function () {
            const id = $(this).attr('data-id');
            confirm('Are you sure you want to delete this menu?', function () {
                axios({
                    url: resolveClientUrl('Menu/delete'),
                    method: 'POST',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    data: { id: id }
                }).then(response => {
                    if (response.data.IsSuccessful) {
                        $('#row_' + id).remove();
                    }
                    else {
                        alert(response.Message);
                    }
                }).catch(axiosError);
            });
        })
    }

    $(document).ready(() => {
        initDeleteEvent();
    });
});