import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { stringify, parse } from 'qs';
import toastr from 'toastr';
import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { renderCompanySelect } from '../../shared/components/CompanySelect';
import {
    axiosError,
    blockUI,
    floatThead,
    unblockUI,
    confirm,
    initDatepicker,
    initTooltip,
    resolveClientUrl,
    toastOptions,
    downloadFile,
} from '../../shared/common.functions';
import { getOrgChart } from '../group-team/supervisor/common.functions';
import { DisciplineType } from '../../shared/constants';

(function () {
    function initEventForIgnoreDiscipline() {
        $(
            '#discipline-detail table.discipline-detail td input.ignoredDiscipline:checkbox'
        ).on('click', function () {
            var disciplineScore = parseFloat($(this).data('discipline-score')),
                approverSubstractScore = parseFloat(
                    $(this)
                        .closest('tr')
                        .find('td.approverSubtractScore span')
                        .text()
                ),
                hasIgnored =
                    $(this).data('has-ignored').toLowerCase() === 'true';

            if ($(this).prop('checked')) {
                approverSubstractScore =
                    Math.abs(approverSubstractScore) - disciplineScore;
            } else {
                approverSubstractScore =
                    Math.abs(approverSubstractScore) + disciplineScore;
            }

            if ($(this).prop('checked') !== hasIgnored) {
                $(this).data('changed', true);
            } else {
                $(this).data('changed', false);
            }

            if (approverSubstractScore > 0) {
                $(this)
                    .closest('tr')
                    .find('td.approverSubtractScore span')
                    .text(-approverSubstractScore.toFixed(1));
                $(this)
                    .closest('tr')
                    .find('td.approverSubtractScore span')
                    .css('color', 'red');
            } else {
                $(this)
                    .closest('tr')
                    .find('td.approverSubtractScore span')
                    .text('0.0');
                $(this)
                    .closest('tr')
                    .find('td.approverSubtractScore span')
                    .css('color', 'inherit');
            }
        });

        $(
            '#discipline-detail table.discipline-detail td input.outOfControlReason:checkbox'
        ).on('click', function () {
            var changed = !$(this).data('changed');
            $(this).data('changed', changed);
        });

        $('#checkAll').on('click', function () {
            var isChecked = $(this).prop('checked');
            $('.outOfControlReason').each(function () {
                var $this = $(this);
                $this.prop('checked', isChecked);
                var changed =
                    isChecked !==
                    ($(this).data('originchecked') === 'true' ||
                        $(this).data('originchecked') === 'True');

                $(this).data('changed', changed);
            });
        });
    }

    function getDataChanged() {
        const ignoredDisciplines = [];
        $('#discipline-detail table.discipline-detail')
            .find('tbody tr')
            .each(function () {
                let approverSubstractScore = parseFloat(
                        $(this).find('td.approverSubtractScore span').text()
                    ),
                    isChanged = false;

                $(this)
                    .find('td input.ignoredDiscipline')
                    .each(function () {
                        if ($(this).data('changed')) {
                            isChanged = true;
                        }
                    });

                if (!isChanged) {
                    if (
                        $(this)
                            .find('td input.outOfControlReason')
                            .first()
                            .data('changed')
                    ) {
                        isChanged = true;
                    }
                }

                if (isChanged) {
                    let ignoredDiscipline = {
                        WorkDate: '',
                        EmployeeId: false,
                        IsIgnoredChangeShift: false,
                        IsIgnoredMissingCheckInOut: false,
                        IsIgnoredLateIn: false,
                        IsIgnoredEarlyOut: false,
                        IsIgnoredAbsent: false,
                        IsIgnoredUnplannedLeave: false,
                        IsIgnoredUnpaidLeave: false,
                        IsIgnoredWorkingUnderTime: false,
                        IsIgnoredUnpaidLeaveAbsent: false,
                        IsIgnoredWeekendLeave: false,
                        IsIgnoredSevenDaysLeave: false,
                        ApproverSubtractScore: Math.abs(approverSubstractScore),
                        IsOutOfControlReason: false,
                    };

                    ignoredDiscipline.IsOutOfControlReason = $(this)
                        .find('td input.outOfControlReason')
                        .prop('checked');

                    $(this)
                        .find('td input.ignoredDiscipline')
                        .each(function () {
                            const $checkbox = $(this);
                            const disciplineTypeId = parseInt(
                                $checkbox.data('discipline-type-id'),
                                10
                            );
                            ignoredDiscipline.WorkDate =
                                $checkbox.data('workdate');
                            ignoredDiscipline.EmployeeId =
                                $checkbox.data('employee-id');

                            switch (disciplineTypeId) {
                                case DisciplineType.ChangeShift:
                                    ignoredDiscipline.IsIgnoredChangeShift =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.MissingCheckInOut:
                                    ignoredDiscipline.IsIgnoredMissingCheckInOut =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.LateIn:
                                    ignoredDiscipline.IsIgnoredLateIn =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.EarlyOut:
                                    ignoredDiscipline.IsIgnoredEarlyOut =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.Absent:
                                    ignoredDiscipline.IsIgnoredAbsent =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.UnpaidLeave:
                                    ignoredDiscipline.IsIgnoredUnpaidLeave =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.WorkingUnderTime:
                                    ignoredDiscipline.IsIgnoredWorkingUnderTime =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.UnpaidLeaveAbsent:
                                    ignoredDiscipline.IsIgnoredUnpaidLeaveAbsent =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.UnplannedLeave:
                                    ignoredDiscipline.IsIgnoredUnplannedLeave =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.WeekendLeave:
                                    ignoredDiscipline.IsIgnoredWeekendLeave =
                                        $checkbox.prop('checked');
                                    break;
                                case DisciplineType.SevenDaysLeave:
                                    ignoredDiscipline.IsIgnoredSevenDaysLeave =
                                        $checkbox.prop('checked');
                                    break;
                            }
                        });

                    ignoredDisciplines.push(ignoredDiscipline);
                }
            });

        return ignoredDisciplines;
    }

    function getDataIgnore() {
        const disciplines = [];
        $('#discipline-detail table.discipline-detail')
            .find('tbody tr')
            .each(function () {
                $(this)
                    .find('td input.ignoredDiscipline')
                    .each(function () {
                        let discipline = {
                            WorkDate: '',
                            EmployeeId: 0,
                            DisciplineTypeId: 0,
                            IsIgnored: false,
                        };
                        const $checkbox = $(this);
                        if ($(this).data('changed')) {
                            const disciplineTypeId = parseInt(
                                $checkbox.data('discipline-type-id'),
                                10
                            );
                            discipline.WorkDate = $checkbox.data('workdate');
                            discipline.EmployeeId =
                                $checkbox.data('employee-id');
                            discipline.DisciplineTypeId = disciplineTypeId;
                            discipline.IsIgnored = $checkbox.prop('checked');

                            disciplines.push(discipline);
                        }
                    });
            });

        return disciplines;
    }

    function initEventForSaveChange() {
        $('#discipline-detail table.discipline-detail input#saveChange').on(
            'click',
            function () {
                const ignoreDetails = getDataChanged();
                const disciplineIgnores = getDataIgnore();
                if (ignoreDetails.length > 0) {
                    confirm('Are you sure you want to save?', function () {
                        axios({
                            url: resolveClientUrl(
                                'discipline/EditDisciplineIgnore'
                            ),
                            method: 'POST',
                            data: stringify({
                                ignoreDetails,
                                disciplineIgnores,
                            }),
                            headers: {
                                'X-Requested-With': 'XMLHttpRequest',
                                'Content-Type':
                                    'application/x-www-form-urlencoded',
                            },
                        })
                            .then((response) => {
                                const data = response.data;
                                if (data.isSuccessful) {
                                    toastr.success(
                                        'Updated successfully!',
                                        '',
                                        toastOptions()
                                    );
                                    query();
                                } else {
                                    toastr.error(
                                        data.message,
                                        '',
                                        toastOptions()
                                    );
                                }
                            })
                            .catch(axiosError);
                    });
                }
            }
        );
    }

    function initEventForDateRangeDropdown() {
        $('form.form-filter #DateRange').on('change', function () {
            var dateRange = $('form.form-filter #DateRange').val();
            if (parseInt(dateRange, 10) !== 0) {
                const quarter = parseInt(dateRange.charAt(0), 10),
                    year = parseInt(
                        dateRange.substring(1, dateRange.length),
                        10
                    );

                let fromDate = new Date($('form.form-filter #FromDate').val()),
                    toDate = new Date($('form.form-filter #ToDate').val());
                switch (quarter) {
                    case 1: // First Half Year 1H
                        fromDate.setFullYear(year - 1, 11, 1);
                        toDate.setFullYear(year, 5, 0);
                        break;

                    case 2: // Second Half Year 2H
                        fromDate.setFullYear(year, 5, 1);
                        toDate.setFullYear(year, 11, 0);
                        break;

                    default:
                        fromDate.setFullYear(year, 0, 1);
                        toDate.setFullYear(year, 11, 31);
                        break;
                }

                $('form.form-filter #FromDate').val(formatDate(fromDate));
                $('form.form-filter #ToDate').val(formatDate(toDate));
            }
        });
    }

    function formatDate(date) {
        const monthNames = [
            'Jan',
            'Feb',
            'Mar',
            'Apr',
            'May',
            'Jun',
            'Jul',
            'Aug',
            'Sep',
            'Oct',
            'Nov',
            'Dec',
        ];

        const day = date.getDate(),
            monthIndex = date.getMonth(),
            year = date.getFullYear();

        return (
            (day < 10 ? '0' + day : day) +
            ' ' +
            monthNames[monthIndex] +
            ', ' +
            year
        );
    }

    function query() {
        blockUI();
        const formData = parse($('form.form-filter').serialize());
        const companyId = $('#CompanyId').val();
        if (companyId) {
            formData.companyId = companyId;
        }

        axios({
            url: resolveClientUrl('discipline/getDisciplineDetailGrid'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: formData,
            responseType: 'text',
        })
            .then((response) => {
                $('#discipline-detail').html(response.data);
                floatThead('table#data');
                initEventForIgnoreDiscipline();
                initEventForSaveChange();
                initTooltip();
            })
            .catch(axiosError)
            .then(() => unblockUI());
    }

    $(document).ready(function () {
        initEventForDateRangeDropdown();
        initDatepicker();

        const roleId = $('#RoleId').val();
        const defaultCompanyId = $('#CompanyId').val();
        renderCompanySelect({
            defaultCompanyId: defaultCompanyId,
            roleId: roleId,
            onChange: (companyId) => {
                $('#CompanyId').val(companyId);
                getOrgChart(companyId)
                    .then((response) => {
                        $('#Team').multiSelectTreeView({
                            data: response.data,
                            isSelectedAll: true,
                            hasSearchBox: true,
                        });
                        $('#TeamId').val(
                            $('#Team').multiSelectTreeView('getSelectedItems')
                        );
                        query();
                    })
                    .catch(axiosError);
            },
        });

        $('form.form-filter #FromDate, form.form-filter #ToDate').on(
            'change',
            function () {
                $('form.form-filter #DateRange').val('0');
            }
        );

        $('form.form-filter #submit').on('click', function (event) {
            $('#TeamId').val(
                $('#Team').multiSelectTreeView('getSelectedItems')
            );
            query();
        });

        $('form.form-filter #btnExcel').on('click', function (event) {
            event.preventDefault();

            blockUI();
            const formData = parse($('form.form-filter').serialize());
            const companyId = $('#CompanyId').val();
            if (companyId) {
                formData.companyId = companyId;
            }

            axios({
                url: resolveClientUrl(
                    'ExportDiscipline/ExportDisciplineDetailReport'
                ),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: formData,
                responseType: 'blob',
            })
                .then((response) => {
                    const fileName =
                        response.headers['content-disposition'].match(
                            /filename=([^;]+)/
                        )[1];
                    downloadFile(response.data, fileName);
                })
                .catch(axiosError)
                .then(() => unblockUI());
        });
    });
})();
