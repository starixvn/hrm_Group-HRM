import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import { initDatepicker } from '../../shared/common.functions';
import '../../date';

$(document).ready(function () {
    initDatepicker();
    setInterval(function () { $('.success').fadeOut("slow"); }, 3000);
});