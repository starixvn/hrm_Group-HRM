import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import '../../shared/multiselect-tree-view';
import axios from 'axios';

import {
    axiosError,
    floatThead,
    confirm,
    resolveClientUrl,
    alert,
    sortTable,
    blockUI,
    unblockUI
} from '../../shared/common.functions';

function deleteTeam(id) {
    confirm('Are you sure you want to delete this team ?', function () {
        blockUI();
        axios({
            url: resolveClientUrl('GroupTeam/delete' + '/' + id),
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded',
            },
        })
            .then(response => {
                unblockUI();
                let data = response.data;
                if (data.isSuccessful) {
                    alert('Delete successfully!');
                    $('#row' + id).remove();
                } else {
                    alert(response.data.Message);
                }
            })
            .catch(axiosError);
    });
}

function initSortTable() {
    $('th.col-sorter').on('click', function () {
        sortTable('table#data', this);
        $('table#data tbody tr').each(function (index) {
            var initIndex = index + 1;
            $(this).find('td:eq(0)').html(initIndex++);
        });
    });
}

function exportExcel() {
    const selectedTeamIds = $('#GroupTeam').multiSelectTreeView('getSelectedItems');
    const query = `teamIds=${selectedTeamIds || ''}`;

    location.href = resolveClientUrl('GroupTeam/ExportGroupTeamList?' + query);
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    let regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    const results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

function loadTeamData(teamIds = null) {
    blockUI();
    axios({
        url: resolveClientUrl('GroupTeam/GetTeamList'),
        method: 'POST',
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        data: new URLSearchParams({ teamIds: teamIds || '' }),
    })
        .then(function (response) {
            unblockUI();
            $('#teamTableContainer').html(response.data);
            initSortTable();
            floatThead('table#data');
            $('table#data a.deleteTeam').on('click', function () {
                const teamId = $(this).data('team-id');
                deleteTeam(teamId);
            });
        })
        .catch(axiosError);
}

function getOrgChart() {
    blockUI();
    axios({
        url: resolveClientUrl('GroupEmployeeList/GetOrgChart'),
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
    })
        .then(response => {
            unblockUI();
            $('#GroupTeam').multiSelectTreeView({
                data: response.data.data,
                selectedItems: response.data.teamIds,
                hasSearchBox: true,
            });
        })
        .catch(axiosError);
}

$(document).ready(function () {
    getOrgChart();

    $('#exportExcel').on('click', function () {
        exportExcel($(this));
    });

    $('form.filter').on('submit', function (e) {
        e.preventDefault();

        var selectedTeamIds =
            $('#GroupTeam').multiSelectTreeView('getSelectedItems');

        loadTeamData(selectedTeamIds);
    });

    loadTeamData();

    floatThead('table#data');
    initSortTable();
    $('table#data a.deleteTeam').on('click', function () {
        const teamId = $(this).data('team-id');
        deleteTeam(teamId);
    });
});