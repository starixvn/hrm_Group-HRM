import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'jquery-contextmenu/dist/jquery.contextMenu';
import toastr from 'toastr';
import axios from 'axios';

import { axiosError, confirm, toastOptions, resolveClientUrl } from '../../../shared/common.functions';

$(function () {
    function cancelRequest(id) {
        confirm('Are you sure you want to cancel this request?', function () {
            axios({
                url: resolveClientUrl('checkinout/delete' + '/' + id),
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;
                if (data.isSuccessful) {
                    $('form #btnSearch').trigger('click');
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        });
    }

    function processRequest(command, options) {
        const eventElement = options.$trigger;
        const requestId = parseInt(eventElement.attr('requestId'), 10);

        switch (command) {
            case 'Edit':
                location.href = resolveClientUrl('checkinout/edit/' + requestId);
                break;
            case 'Cancel':
                cancelRequest(requestId);
                break;
        }
    }

    function bindMenu(element) {
        let renderIconOk = function (opt, $itemElement, itemKey, item) {
            $itemElement.html('<span class="edit"><i class="fa fa-pencil"></i>' + item.name + '</span>');
            return '';
        };
        let renderIconRemove = function (opt, $itemElement, itemKey, item) {
            $itemElement.html('<span class="void"><i class="fa fa-times"></i>' + item.name + '</span>');
            return '';
        };

        $.contextMenu({
            selector: element,
            trigger: 'left',
            callback: processRequest,
            items: {
                'Edit': { name: 'Edit', icon: renderIconOk },
                'Cancel': { name: 'Cancel', icon: renderIconRemove }
            }
        });
    }

    $(document).ready(function () {
        $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });
        bindMenu('.menu-actions');

        $('.viewhistory').click(function () {
            axios({
                url: resolveClientUrl('checkinout/ViewHistory/' + $(this).attr('requestid')),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'text'
            }).then(response => {
                $('#dialoghistorycontent').html(response.data);
                $('.tooltip-tipsy').tipsy({ html: true, gravity: $.fn.tipsy.autoWE });
                $('#dialoghistorycontent').dialog({
                    modal: true,
                    width: 1100,
                    title: 'Request History'
                });
            }).catch(axiosError);
        });
    });
});