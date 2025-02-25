import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import { axiosError, confirm, autoIndex, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    const deleteMemo = function (memoId) {
        confirm('Are you sure you want to delete this memo?', function () {
            axios({
                url: resolveClientUrl('Memo/delete'),
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: { id: memoId }
            }).then(response => {
                $('#row_' + memoId).remove();
                autoIndex();
            }).catch(axiosError);
        });
    };

    $('.icon-delete').on('click', function () {
        const id = $(this).attr('data-id');
        deleteMemo(id);
    });
});