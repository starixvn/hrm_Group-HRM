import { showFullAvatar } from '../../shared/common.functions';

$(document).ready(function () {
    $('.fullSizeAvatar').on('click', function () {
        const codeId = $(this).data('code');
        const nickname = $(this).data('nickname');
        showFullAvatar(codeId, nickname);
    });
});