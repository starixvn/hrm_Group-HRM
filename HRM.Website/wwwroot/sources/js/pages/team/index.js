import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';

import { axiosError, floatThead, confirm, resolveClientUrl, alert, sortTable } from '../../shared/common.functions';

function deleteTeam(id) {
    confirm('Are you sure you want to delete this team ?', function () {
        axios({
            url: resolveClientUrl('groupteam/delete' + '/' + id),
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;
            if (data.isSuccessful) {
                alert('Delete successfully!');
                $('#row' + id).remove();
            } else {
                alert(response.data.Message);
            }
        }).catch(axiosError);
    });
}

function initSortTable() {
    $('th.col-sorter').on('click', function () {
        sortTable('table#data', this);
        $("table#data tbody tr").each(function (index) {
            var initIndex = index + 1;
            $(this).find("td:eq(0)").html(initIndex++);
        });
    });
}

function exportExcel() {
    var url = null,
        controllerPath;

    var nameSearchValue = $('#nameSearch').val();
    controllerPath = "GroupTeam/ExportGroupTeamList?";

    var query = `keyword=${nameSearchValue}`;

    url = resolveClientUrl(controllerPath + query);
    location.href = url;
}

$(document).ready(function () {
    $("#exportExcel").on('click', function () {
        exportExcel($(this));
    });
    floatThead('table#data');
    initSortTable();
    $('table#data a.deleteTeam').on('click', function () {
        const teamId = $(this).data('team-id');
        deleteTeam(teamId);
    });
});