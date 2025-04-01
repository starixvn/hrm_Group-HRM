import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import moment from 'moment';
import { initDatepicker } from '../../shared/common.functions';

const editHoliday = (function ($) {
    const me = {};
    let $numDay, $fromDate, $toDate;

    function setToDate() {
        var inputDay = parseInt($numDay.val(), 10) - 1;
        var fromDate = $fromDate.val();
        var toDate = new Date(Date.parse(fromDate));

        toDate.setDate(toDate.getDate() + inputDay);
        $toDate.val(moment(toDate).format('DD MMM, YYYY'));
    }

    function initEvent() {
        $numDay.change(function () {
            setToDate();
        });

        $numDay.click(function () {
            setToDate();
        });

        $('#FromDate, #ToDate').change(function () {
            const fromDate = $fromDate.datepicker('getDate');
            const toDate = $toDate.datepicker('getDate');
            const dayDiff = ((toDate - fromDate) / 1000 / 60 / 60 / 24) + 1;

            $numDay.val(dayDiff);
        });
    };

    me.init = function () {
        $numDay = $('#numDay');
        $fromDate = $('#FromDate');
        $toDate = $('#ToDate');

        $('#FromDate, #ToDate').datepicker({
            changeMonth: true,
            changeYear: true
        });

        initEvent();
    };

    return me;
}(jQuery));

$(document).ready(function () {
    initDatepicker();
    editHoliday.init();
});