import React, { PureComponent } from 'react';
import moment from 'moment';

import { DateFormat } from '../constants';

export default class WeekPicker extends PureComponent {
    componentDidMount() {
        this.$el = $(this.el);
        this.updateWeekPicker();

        document.addEventListener('mousedown', this.handleClickOutside);
    }

    componentDidUpdate(prevProps) {
        if (this.props.fromDate !== prevProps.fromDate || this.props.toDate !== prevProps.toDate) {
            this.updateWeekPicker();
        }
    }

    componentWillUnmount() {
        this.$el.weekpicker('destroy');
        document.removeEventListener('mousedown', this.handleClickOutside);
    }

    handleClickOutside = event => {
        if (this.wrapperRef && !this.wrapperRef.contains(event.target)) {
            this.$el.hide();
        }
    };

    updateWeekPicker = () => {
        const vm = this;

        this.$el.weekpicker({
            firstDay: 1,
            onSelect: function (_unused1, _unused2, startDate, endDate) {
                vm.props.onChange(moment(startDate).format(DateFormat), moment(endDate).format(DateFormat));
                vm.$el.hide();
            }
        });

        this.$el.weekpicker('setDate', new Date(new Date(this.props.fromDate)));
    };

    handleShowCalendar = () => {
        this.$el.toggle();
    };

    render() {
        return (
            <div className="weekpicker-wrapper" ref={node => this.wrapperRef = node}>
                <input
                    value={this.props.fromDate}
                    onClick={this.handleShowCalendar}
                    className="fromDate form-control" readOnly
                />
                <input value={this.props.toDate}
                    onClick={this.handleShowCalendar}
                    className="toDate form-control" readOnly
                />
                <div className="calendar" ref={node => this.el = node} />
                <i className="icon-calendar fa fa-calendar" onClick={this.handleShowCalendar} />
            </div>
        );
    }
}