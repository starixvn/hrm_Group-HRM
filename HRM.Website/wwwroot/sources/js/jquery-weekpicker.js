import moment from 'moment';

(function ($) {
    $.widget('lugolabs.weekpicker', {
        _weekOptions: {
            showOtherMonths: true,
            selectOtherMonths: true
        },

        _create: function () {
            let self = this;
            this._dateFormat = this.options.dateFormat || $.datepicker._defaults.dateFormat;
            const date = this._initialDate();
            this._setWeek(date);
            let onSelect = this.options.onSelect;
            this._picker = $(this.element).datepicker($.extend(this.options, this._weekOptions, {
                onSelect: function (dateText, inst) {
                    self._select(dateText, inst, onSelect);
                },
                beforeShowDay: function (date) {
                    return self._showDay(date);
                },
                onChangeMonthYear: function (year, month, inst) {
                    self._selectCurrentWeek();
                }
            }));

            $(document)
                .on('mousemove', '.ui-datepicker-calendar tr', function () { $(this).find('td a').addClass('ui-state-hover'); })
                .on('mouseleave', '.ui-datepicker-calendar tr', function () { $(this).find('td a').removeClass('ui-state-hover'); });
            this._select(moment(date).format('MM/DD/YYYY'), this._picker, onSelect);
        },

        _initialDate: function () {
            if (this.options.currentText) {
                return $.datepicker.parseDate(this._dateFormat, this.options.currentText);
            } else {
                return new Date;
            }
        },

        _select: function (dateText, inst, onSelect) {
            this._setWeek(this._picker.datepicker('getDate'));
            const startDateText = $.datepicker.formatDate(this._dateFormat, this._startDate, inst.settings);
            this._picker.val(startDateText);
            if (onSelect) {
                onSelect(dateText, startDateText, this._startDate, this._endDate, inst);
            }
        },

        _showDay: function (date) {
            const cssClass = date >= this._startDate && date <= this._endDate ? 'ui-datepicker-current-day' : '';
            return [true, cssClass];
        },

        _setWeek: function (date) {
            const selected = moment(date, 'YYYY-MM-DD');

            const fromDateClone = selected.clone();
            const from_date = fromDateClone.startOf('isoWeek'); // why use isoWeek - https://en.wikipedia.org/wiki/ISO_8601#Week_dates
            this._startDate = from_date.toDate();

            const toDateClone = selected.clone();
            const to_date = toDateClone.endOf('isoWeek');
            this._endDate = to_date.toDate();
        },

        _selectCurrentWeek: function () {
            $('.ui-datepicker-calendar').find('.ui-datepicker-current-day a').addClass('ui-state-active');
        },

        setDate: function (date) {
            this._setWeek(date);
            this._picker.datepicker('setDate', $.datepicker.formatDate(this._dateFormat, date));
            this._picker.hide();
            let onSelect = this.options.onSelect;
            this._select(moment(date).format('DD/MM/YYYY'), this._picker, onSelect);
        }
    });
})(jQuery);