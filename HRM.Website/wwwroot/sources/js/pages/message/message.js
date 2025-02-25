import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios'
import { confirm, initDatepicker, autoIndex, axiosError, resolveClientUrl } from '../../shared/common.functions';
import '../../date';

var deleteMessage = function (messageId) {
    confirm('Are you sure you want to delete this message?', function () {
        axios({
            url: resolveClientUrl('Message/Delete'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                id: messageId
            }
        }).then(() => {
            $('#row_' + messageId).remove();
            autoIndex();
        }).catch(axiosError);
    });
};

var detailMessage = function (messageId) {
    axios({
        url: resolveClientUrl('Message/EditMessageStatus'),
        method: 'POST',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        params: {
            id: messageId
        }
    }).then(() => {
        window.location.href = resolveClientUrl('message/detailByEmployee/' + messageId);
    }).catch(axiosError);
};

var init = function () {
    initDatepicker();

    $('.message-detail').on('click', function () {
        var id = $(this).attr('data-id');
        detailMessage(id);
    });

    $('.icon-delete').on('click', function () {
        var id = $(this).attr('data-id');
        deleteMessage(id);
    });
}

$(document).ready(function () {
    init();
});