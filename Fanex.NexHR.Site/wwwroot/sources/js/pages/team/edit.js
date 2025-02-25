import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js';
import 'jquery-ui-bundle/jquery-ui';
import 'chosen-js/chosen.jquery';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../shared/common.functions';

(function () {
    function initSearchUnitHead() {
        $('#UnitHead').autocomplete({
            source: function (request, response) {
                axios({
                    url: resolveClientUrl('employee/getemployees'),
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    params: {
                        query: request.term,
                        enable: true,
                        isDelete: false,
                        $top: 10
                    }
                }).then(res => {
                    response($.map(res.data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')',
                            value: item.Id
                        };
                    }));
                }).catch(axiosError);
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $('#UnitHead').val(ui.item.label);
                $('#HeadId').val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $('#UnitHead').val(ui.item.label);
                $('#HeadId').val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $('#UnitHead').val(ui.item.label);
                    $('#HeadId').val(ui.item.value);
                } else {
                    if ($('#UnitHead').val() === '') {
                        $('#HeadId').val('');
                    } else {
                        $('#HeadId').val('0');
                    }
                }
            }
        });
    }

    function initSearchManager() {
        const assistIds = $('#ManagerIds').val();
        if (assistIds) {
            $('#Managers').val(assistIds.split(','));
        }
        $('#Managers').chosen();
        $('#Managers').on('change', function () {
            const values = $(this).val(),
                managerIds = values ? values.join(',') : '';
            $('#ManagerIds').val(managerIds);
        });
    }

    function initSearchSupervisor() {
        const assistIds = $('#SupervisorIds').val();
        if (assistIds) {
            $('#Supervisors').val(assistIds.split(','));
        }
        $('#Supervisors').chosen();
        $('#Supervisors').on('change', function () {
            const values = $(this).val(),
                supervisorIds = values ? values.join(',') : '';
            $('#SupervisorIds').val(supervisorIds);
        });
    }

    function initSearchLeader() {
        const leaderIds = $('#LeaderIds').val();
        if (leaderIds) {
            $('#Leaders').val(leaderIds.split(','));
        }
        $('#Leaders').chosen();

        $('#Leaders').on('change', function () {
            const values = $(this).val(),
                leaderIds = values ? values.join(',') : '';
            $('#LeaderIds').val(leaderIds);
        });
    }

    function validate() {
        var mutationObserver = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.type == 'childList') {
                    if (mutation.addedNodes.length >= 1) {
                        var x = document.querySelectorAll("[class='field-validation-error']");
                        var count = 0;
                        for (var i = 0; i < x.length; i++) {
                            if (x[i].innerText == mutation.target.innerText) {
                                count++;
                                if (count > 1) {
                                    x[i].className = "field-validation-valid"
                                }
                            }
                        }
                    }
                    if (mutation.removedNodes.length >= 1) {
                        var messageRemove = mutation.removedNodes[0].innerText;
                        var y = document.querySelectorAll("[class='field-validation-valid']");
                        var count = 0;
                        for (var i = 0; i < y.length; i++) {
                            if (y[i].innerText == messageRemove) {
                                count++;
                                if (count == 1) {
                                    y[i].className = "field-validation-error"
                                }
                            }
                        }
                    }
                }
            });
        });
        var config = {
            attributes: true,
            characterData: true,
            childList: true,
            subtree: true
        };
        var targetNode = document.getElementById('form');

        mutationObserver.observe(targetNode, config);
    }

    $(document).ready(function () {
        initSearchUnitHead();
        initSearchManager();
        initSearchSupervisor();
        initSearchLeader();
        validate();
    });
})();