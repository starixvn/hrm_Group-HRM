import moment from 'moment';
import 'jquery-ui-bundle/jquery-ui';

const _rootUrl = document.getElementById('RootURL').value ? document.getElementById('RootURL').value : '/';

$.extend({
    getOrCreateDialog: function (id) {
        var $box = $('#' + id);
        if (!$box.length) {
            $box = $('<div id="' + id + '"><p></p></div>').hide().appendTo('body');
        }

        return $box;
    }
});

function showMessage(message) {
    $('#message').html(message + ' <img src="/images/ajax-loader.gif" />').css('left', $(window).width() / 2);
    $('#message').slideDown();
}

function fadeOutMessage() {
    $('#message').fadeOut(500);
}

function confirm(message, callback, options) {
    const title = "HRM";
    const defaults = {
        title: title,
        modal: true,
        resizable: false,
        buttons: [
            {
                text: 'OK',
                class: 'btn btn-primary',
                click: function () {
                    $(this).dialog('close');
                    if (callback !== null) {
                        callback();
                    }

                    return true;
                }
            },
            {
                text: 'Cancel',
                class: 'btn btn-default',
                click: function () {
                    $(this).dialog('close');
                    return false;
                }
            }
        ],
        minWidth: 300,
        minHeight: 100,
        dialogClass: 'modal-shadow'
    };

    let $confirm = $.getOrCreateDialog('confirm');
    // set message
    $('p', $confirm).html(message);
    // init dialog
    $confirm.dialog($.extend({}, defaults, options));
}

function showFullAvatar(code, nickname) {
    const defaults = {
        title: nickname,
        modal: true,
        resizable: false,
        dialogClass: 'popup-avatar'
    };

    const path = resolveClientUrl(`employee/avatar/${code}`);
    const avatarHtml = `<img src="${path}" style="width: 100%; height: 100%" />`;

    const $confirm = $.getOrCreateDialog('fullAvatar');

    $($confirm).html(avatarHtml);

    // init dialog
    $confirm.dialog($.extend({}, defaults));
}

function getParam(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results === null)
        return null;
    else
        return results[1];
}

function alert(message, callback) {
    const title = "HRM";
    const defaults = {
        title: title,
        modal: true,
        resizable: false,
        buttons: [
            {
                text: 'OK',
                class: 'btn btn-primary',
                click: function () {
                    $(this).dialog('close');
                    if (callback) {
                        callback();
                    }

                    return false;
                }
            }
        ],
        minHeight: 50,
        width: 680,
        dialogClass: 'modal-shadow'
    };

    let $alert = $.getOrCreateDialog('alert');
    // set message
    $('p', $alert).html(message);
    // init dialog
    $alert.dialog($.extend({}, defaults));
}

function blockUI() {
    document.getElementById('loadingProcess').style.display = 'block';
}

function unblockUI() {
    document.getElementById('loadingProcess').style.display = 'none';
}

function toastOptions() {
    return {
        tapToDismiss: false,
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-top-center',
        showDuration: 0,
        hideDuration: 0,
        timeOut: 3000,
        extendedTimeOut: 0,
        closeOnHover: false,
        preventDuplicates: true,
        closeHtml: '<button type="button">x</button>'
    };
}

function axiosError(error) {
    if (error.response) {
        let url;

        switch (error.response.status) {
            case 401:
                url = 'auth/login';
                break;

            case 404:
                url = 'error/PageNotFound';
                break;

            case 403:
            case 405:
                url = 'error/AccessDenied';
                break;

            case 500:
                url = 'error/InternalError';
                break;
        }

        window.location = resolveClientUrl(url);
    }
}

function reorder(items, startIndex, endIndex) {
    const [removed] = items.splice(startIndex, 1);
    items.splice(endIndex, 0, removed);

    return [...items];
}

function floatThead(el) {
    $(el).floatThead({
        scrollContainer: function ($table) {
            return $table.closest('.content_scroll');
        },
        position: 'fixed',
        top: 72
    });

    $('#toggle-left-navigation').on('click', function () {
        $(el).floatThead('reflow');
    });
}

function initFromDateOnChange($fromDate, $toDate) {
    if ($fromDate && $toDate) {
        $fromDate.on("change", function () {
            let toMonth = new Date($toDate.val()).getMonth();
            let fromMonth = new Date($fromDate.val()).getMonth();
            if (fromMonth !== toMonth) {
                $toDate.val($(this).val());
            }
        });
    }
}

function initDatepicker() {
    $('.datepicker').each(function () {
        let $this = $(this);

        let beforeShowDayHandler = function (mydate) {
            let isEnable = true;
            const disabledDates = $this.attr('disabledDate');
            if (disabledDates && disabledDates.length > 0) {
                const checkdate = moment(mydate),
                    myBadDates = disabledDates.split(',');
                for (let i = 0; i < myBadDates.length; i++) {
                    const date = moment(myBadDates[i]);
                    if (date.isSame(checkdate)) {
                        isEnable = false;
                    }
                }
            }

            return [isEnable];
        };
        $this.datepicker({
            dateFormat: 'dd M, yy',
            beforeShowDay: beforeShowDayHandler,
            changeMonth: true,
            changeYear: true
        });

        let mindate = $this.attr('mindate'),
            maxdate = $this.attr('maxdate');
        if (mindate && mindate !== 'null') {
            mindate = mindate.split('/');
            $this.datepicker('option', 'minDate', new Date(mindate[0], mindate[1] - 1, mindate[2]));
        }

        if (maxdate && maxdate !== 'null') {
            maxdate = maxdate.split('/');
            $this.datepicker('option', 'maxDate', new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
        }

        $this.attr('readonly', 'readonly');
        $this.keydown(function (e) {
            const tab = 9, enter = 13;
            if (e.keyCode === tab || e.keyCode === enter) {
                return true;
            }

            return false;
        });

        $this.closest('.calendar-form').find('.icon-calendar:not(.disabled)').off('click').on('click', function () {
            if ($this.datepicker('widget').is(':visible')) {
                $this.datepicker('hide');
            } else {
                $this.datepicker('show');
            }
        });
    });
}

function setDateRange(selectedIndex) {
    let fromDate = new Date($('#FromDate').val()).toString('dd MMM, yyyy');
    let toDate = new Date($('#ToDate').val()).toString('dd MMM, yyyy');
    let now = $('#Today').val();

    switch (parseInt(selectedIndex, 10)) {
        case 0: //default
            break;

        case 1: //today
            toDate = fromDate = Date.parse(now).toString('dd MMM, yyyy');
            break;

        case 2: // yesterday
            toDate = fromDate = Date.parse(now).add(-1).days().toString('dd MMM, yyyy');
            break;

        case 3: //current week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;

        case 4: //last week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-7).days().toString('dd MMM, yyyy');
            toDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-1).days().toString('dd MMM, yyyy');
            break;

        case 5: //current month
            fromDate = Date.parse(now).moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;

        case 6: //last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).add(-1).months().moveToLastDayOfMonth().toString('dd MMM, yyyy');
            break;

        case 7: //since last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
    }

    $('#FromDate').val(fromDate);
    $('#ToDate').val(toDate);
}

function showValidationSummaryErrors(messages) {
    $('.validation-summary-valid').show();
    $('.validation-summary-valid').addClass('validation-summary-errors');
    let msgWrp = $('.validation-summary-valid').find('ul:first');
    messages.forEach(function (msg) {
        msgWrp.append('<li>' + msg + '</li>');
    });
}

function hideValidationSummaryErrors(isReset) {
    if (isReset) {
        $('.validation-summary-errors').addClass('validation-summary-valid').removeClass('validation-summary-errors');
    }
    let msgWrp = $('.validation-summary-valid').removeClass('validation-summary-errors');
    msgWrp.find('ul:first').html('');
    $('.validation-summary-valid').hide();
}

function autoIndex() {
    var index = 0;
    $('tr').each(function () {
        $(this).children("td").eq(0).html(index);
        index = index + 1;
    });
}

function resolveClientUrl(relativePath) {
    return _rootUrl + relativePath;
}

function initTooltip() {
    $('.tooltip-tipsy').tipsy({
        html: true,
        gravity: 'e'
    });
}

function downloadFile(data, fileName) {
    const url = window.URL.createObjectURL(new Blob([data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', fileName);
    document.body.appendChild(link);
    link.click();
    link.parentNode.removeChild(link);
    window.URL.revokeObjectURL(url);
}

function sortTable(table, th) {
    let getCellValue = function (row, index) {
        let $td = $(row).children('td').eq(index);
        let datetimeAttr = $td.attr('data-datetime');

        if (typeof datetimeAttr !== typeof undefined && datetimeAttr !== false) {
            return $td.data('datetime');
        }

        return $td.text().trim().toLowerCase();
    };
    let refreshSorting = function () {
        $('th.col-sorter').each(function () {
            $(this).find('div.fa').removeClass('fa-sort-asc').removeClass('fa-sort-desc');
        });
    };

    let $icon = $(th).find('div.fa');
    if ($icon.hasClass('fa-sort-desc')) {
        $icon.removeClass('fa-sort-desc').addClass('fa-sort-asc');
    } else if ($icon.hasClass('fa-sort-asc')) {
        $icon.removeClass('fa-sort-asc').addClass('fa-sort-desc');
    } else {
        refreshSorting();
        $icon.addClass('fa-sort-desc');
    }

    const colIndex = $(th).index();
    let rows = $(table).find('tbody tr').toArray().sort(function (a, b) {
        let valA = getCellValue(a, colIndex),
            valB = getCellValue(b, colIndex);

        if ($.isNumeric(valA) && $.isNumeric(valB)) {
            return valA - valB;
        } else if (moment(valA).isValid() && moment(valB).isValid()) {
            const dateA = new Date(valA),
                dateB = new Date(valB);
            return dateA - dateB;
        } else {
            return valA.localeCompare(valB, 'vi', { sensitivity: 'base' });
        }
    });

    if (!$icon.hasClass('fa-sort-asc')) {
        rows = rows.reverse();
    }

    for (let i = 0; i < rows.length; i++) {
        $(table).append(rows[i]);
    }
}

function debounce(func, wait) {
    let timeout;
    return function (...theArgs) {
        const context = this,
            args = theArgs;

        const later = () => {
            func.apply(context, args);
        };

        clearTimeout(timeout);
        timeout = setTimeout(later, wait || 500);
    };
}

export {
    showMessage,
    fadeOutMessage,
    confirm,
    blockUI,
    alert,
    unblockUI,
    toastOptions,
    axiosError,
    reorder,
    floatThead,
    initDatepicker,
    initFromDateOnChange,
    setDateRange,
    getParam,
    showValidationSummaryErrors,
    hideValidationSummaryErrors,
    autoIndex,
    resolveClientUrl,
    initTooltip,
    downloadFile,
    sortTable,
    showFullAvatar,
    debounce
};