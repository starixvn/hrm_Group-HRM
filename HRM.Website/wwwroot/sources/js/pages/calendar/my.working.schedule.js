import { Calendar } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';

import moment from 'moment';
import axios from 'axios';

import { LeaveType } from '../../shared/constants';
import { axiosError, blockUI, unblockUI, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    let calendarEl = document.getElementById('calendar');
    let calendar = new Calendar(calendarEl, {
        plugins: [dayGridPlugin],
        defaultView: 'dayGridMonth',
        height: 500,
        firstDay: 1,
        editable: false,
        showNonCurrentDates: false,
        fixedWeekCount: false,
        header: {
            left: 'prev',
            center: 'title',
            right: 'next'
        },
        events: function (fetchInfo, successCallback, failureCallback) {
            blockUI();
            axios({
                url: resolveClientUrl('calendar/GetWorkingSchedule'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    date: moment(fetchInfo.start).format('DD-MMM-YYYY')
                }
            }).then(response => {
                let data = [];
                response.data.map(item => {
                    let shift = '',
                        className = '';

                    if (item.isLeaveOffDay) {
                        shift = 'Off';
                        className = 'leave offday';
                    } else if (item.isLeaveDay) {
                        if (item.isHoliday) {
                            shift = 'PH';
                            className = 'holiday';
                        } else if (item.isOffDay) {
                            shift = 'Off';
                            className = 'offday';
                        } else {
                            if (item.leaveType !== null && (item.leaveType2 === null || item.leaveType.code === item.leaveType2.code)) {
                                switch (item.leaveType.id) {
                                    case LeaveType.AnnualLeave:
                                        className = 'annualleave';
                                        break;
                                    case LeaveType.UnpaidLeave:
                                        className = 'unpaidleave';
                                        break;
                                    case LeaveType.BusinessTrip:
                                        className = 'businesstrip';
                                        break;
                                    case LeaveType.WeddingLeave:
                                        className = 'weddingleave';
                                        break;
                                    default:
                                        className = 'leaveday';
                                        break;
                                }
                            }

                            if (item.isLeaveHalfDay) {
                                shift = item.leaveType.code + ' ½';
                            } else {
                                shift = item.leaveType.code;

                                if (item.leaveType2 !== null && item.leaveType.code !== item.leaveType2.code) {
                                    className = 'leaveday leavehalfday2';
                                    shift = item.leaveType.code + ' / ' + item.leaveType2.code;
                                }
                            }
                        }
                    } else if (item.isHoliday) {
                        className = 'holiday';
                        if (item.isOffDay) {
                            shift = 'Off';
                        } else {
                            shift = item.shift !== null ? item.shift.code : 'PH';
                        }
                    } else if (item.isOffDay) {
                        className = 'offday';
                        shift = item.shift !== null ? item.shift.code : 'Off';
                    } else {
                        shift = item.shift !== null ? item.shift.code : '';
                    }

                    data.push({
                        title: shift,
                        start: moment(item.workDate).format('YYYY-MM-DD'),
                        classNames: className
                    });
                });

                successCallback(data);
            }).catch(axiosError).then(() => unblockUI());
        }
    });
    calendar.render();
});