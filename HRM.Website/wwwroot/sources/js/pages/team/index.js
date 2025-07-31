import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';

import {
    axiosError,
    floatThead,
    confirm,
    resolveClientUrl,
    alert,
    sortTable,
    stringify,
} from '../../shared/common.functions';

function deleteTeam(id) {
    confirm('Are you sure you want to delete this team ?', function () {
        axios({
            url: resolveClientUrl('groupteam/delete' + '/' + id),
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded',
            },
        })
            .then((response) => {
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
    var url = null,
        controllerPath;

    var nameSearchValue = $('#nameSearch').val();
    controllerPath = 'GroupTeam/ExportGroupTeamList?';

    var query = `keyword=${nameSearchValue}`;

    url = resolveClientUrl(controllerPath + query);
    location.href = url;
}

$(document).ready(function () {
    $('#exportExcel').on('click', function () {
        exportExcel($(this));
    });
    floatThead('table#data');
    initSortTable();
    $('table#data a.deleteTeam').on('click', function () {
        const teamId = $(this).data('team-id');
        deleteTeam(teamId);
    });
});

$(document).ready(function () {
    // Initialize company dropdown
    console.log('Document ready, calling loadCompanies');
    loadCompanies();

    // Handle company selection change
    $('#companySelect').change(function () {
        const selectedCompanyId = $(this).val();
        console.log('Selected company ID:', selectedCompanyId);

        // Trigger custom event for parent page to handle
        $(document).trigger('companyChanged', [selectedCompanyId]);
    });
});

function loadCompanies() {
    // Hardcode for testing
    const employeeId = 424;
    const roleId = 4;

    axios({
        url: resolveClientUrl(
            `Team/GetCompaniesByRoleIdForEmployee?employeeId=${employeeId}&roleId=${roleId}`
        ),
        method: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
        },
    })
        .then((response) => {
            console.log('API response received:', response.data);
            const companies = response.data;
            const select = $('#companySelect');
            select.empty();
            select.append('<option value="">-- Select Company --</option>');

            if (companies && companies.length > 0) {
                companies.forEach(function (company) {
                    console.log('Adding company:', company);
                    select.append(
                        '<option value="' +
                            company.companyId +
                            '">' +
                            company.name +
                            '</option>'
                    );
                });
            } else {
                console.warn('No companies returned from API');
                select.append(
                    '<option value="">No companies available</option>'
                );
            }
        })
        .catch(axiosError);
}

// Listen for company filter changes
$(document).on('companyChanged', function (event, companyId) {
    console.log('Company filter changed to:', companyId);
    // Add logic to filter team data by company
    if (companyId) {
        // You can add filtering logic here
        console.log('Filtering teams by company:', companyId);
    }
});
