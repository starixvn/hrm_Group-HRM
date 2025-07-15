import $ from "jquery/dist/jquery";
import "jquery-ui-bundle/jquery-ui";
import "floatthead/dist/jquery.floatThead";
import "bootstrap/dist/js/bootstrap";
import "../../shared/multiselect-tree-view";
import axios from "axios";

import {
    axiosError,
    floatThead,
    confirm,
    resolveClientUrl,
    alert,
    sortTable,
} from "../../shared/common.functions";

function deleteTeam(id) {
    confirm("Are you sure you want to delete this team ?", function () {
        axios({
            url: resolveClientUrl("GroupTeam/delete" + "/" + id),
            method: "POST",
            headers: {
                "X-Requested-With": "XMLHttpRequest",
                "Content-Type": "application/x-www-form-urlencoded",
            },
        })
            .then((response) => {
                let data = response.data;
                if (data.isSuccessful) {
                    alert("Delete successfully!");
                    $("#row" + id).remove();
                } else {
                    alert(response.data.Message);
                }
            })
            .catch(axiosError);
    });
}

function initSortTable() {
    $("th.col-sorter").on("click", function () {
        sortTable("table#data", this);
        $("table#data tbody tr").each(function (index) {
            var initIndex = index + 1;
            $(this).find("td:eq(0)").html(initIndex++);
        });
    });
}

function exportExcel() {
    var url = null,
        controllerPath;

    var selectedTeamIds =
        $("#GroupTeam").multiSelectTreeView("getSelectedItems");
    controllerPath = "GroupTeam/ExportGroupTeamList?";

    var query = `teamIds=${selectedTeamIds || ""}`;

    url = resolveClientUrl(controllerPath + query);
    location.href = url;
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
    var results = regex.exec(location.search);
    return results === null
        ? ""
        : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function loadTeamData(teamIds = null) {
    axios({
        url: resolveClientUrl("GroupTeam/GetTeamList"),
        method: "POST",
        headers: {
            "X-Requested-With": "XMLHttpRequest",
            "Content-Type": "application/x-www-form-urlencoded",
        },
        data: new URLSearchParams({ teamIds: teamIds || "" }),
    })
        .then(function (response) {
            $("#teamTableContainer").html(response.data);
            initSortTable();
            floatThead("table#data");
            $("table#data a.deleteTeam").on("click", function () {
                const teamId = $(this).data("team-id");
                deleteTeam(teamId);
            });
        })
        .catch(axiosError);
}

$(document).ready(function () {
    $("#exportExcel").on("click", function () {
        exportExcel($(this));
    });

    $("form.filter").on("submit", function (e) {
        e.preventDefault();

        var selectedTeamIds =
            $("#GroupTeam").multiSelectTreeView("getSelectedItems");

        loadTeamData(selectedTeamIds);
    });

    loadTeamData();

    floatThead("table#data");
    initSortTable();
    $("table#data a.deleteTeam").on("click", function () {
        const teamId = $(this).data("team-id");
        deleteTeam(teamId);
    });
});

$(document).ready(function () {
    axios({
        url: resolveClientUrl("GroupEmployeeList/GetOrgChart"),
        method: "GET",
        headers: { "X-Requested-With": "XMLHttpRequest" },
    })
        .then((response) => {
            $("#GroupTeam").multiSelectTreeView({
                data: response.data.data,
                selectedItems: response.data.teamIds,
                hasSearchBox: true,
            });
        })
        .catch(axiosError);
});
