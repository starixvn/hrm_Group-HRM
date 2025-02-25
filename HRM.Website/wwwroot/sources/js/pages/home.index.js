import 'bootstrap/dist/js/bootstrap';

import { resolveClientUrl } from '../shared/common.functions';

$(document).ready(function () {
    if ($('#IsResetPassword').prop('checked')) {
        $('#changePasswordModal').modal('show');

        $('#btnSubmit').on('click', function () {
            location.href = resolveClientUrl('Account/changepwd');
        });

        $('#btnCancel').on('click', function () {
            $('#changePasswordModal').modal('hide');
        });
    }
});