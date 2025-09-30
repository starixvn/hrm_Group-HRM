import axios from 'axios';

import { axiosError, resolveClientUrl } from './shared/common.functions';

$(document).ready(function () {
    function toggleLeftNavigation(isShow) {
        let currentElement = $('#toggle-left-navigation');

        if (isShow) {
            currentElement.removeClass('large-next');
            currentElement.addClass('large-back');
            currentElement.attr({ title: 'Hide Left Navigation' });
            $('#left-navigation').show();
            $('#content-wrapper').removeClass('expand');
        } else {
            currentElement.removeClass('large-back');
            currentElement.addClass('large-next');
            currentElement.attr({ title: 'Show Left Navigation' });
            $('#left-navigation').hide();
            $('#content-wrapper').addClass('expand');
        }
    }

    function updateLeftNavigationSetting(isShow) {
        axios({
            url: resolveClientUrl('Employee/ToggleLeftNavigation'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: { isShow }
        }).catch(axiosError);
    }

    if (_global.isShowLeftNavigation === 'True') {
        toggleLeftNavigation(true);
    } else {
        toggleLeftNavigation(false);
    }

    $('.flash').animate({ opacity: 0.25 }, 3500, function () {
        $(this).slideUp().remove();
    });

    $('#searchIcon').on('click', function () {
        document.location = resolveClientUrl('employee/index?query=' + $('#searchbox').val());
    });

    $('#searchbox').keyup(function (event) {
        if (event.keyCode === 13) {
            $('#searchIcon').trigger('click');
        }
    });

    $('#toggle-left-navigation').on('click', function () {
        let currentElement = $(this);
        if (currentElement.hasClass('large-back')) {
            toggleLeftNavigation(false);
            updateLeftNavigationSetting(false);
        } else {
            toggleLeftNavigation(true);
            updateLeftNavigationSetting(true);
        }
    });

    const currentUrlSelected = $('#CurrentUrlSelected').val();
    if (currentUrlSelected) {
        $('#' + currentUrlSelected).addClass('selected');
    }

    //show/hide navigation section
    $('div.togglable_section').bind('click', function () {
        let currentSection = $(this);
        let divItems = currentSection.next('div.navItem');
        if (currentSection.hasClass('folded')) {
            currentSection.removeClass('folded');
            divItems.show(200);
        } else {
            currentSection.addClass('folded');
            divItems.hide(200);
        }
    });

    // add style for first item section
    $('div.togglable_section:first-child').children(':first').addClass('first');

    $('.el[data-hashovermenu=True], .el[data-hashovermenu=data-hashovermenu]').hover(
        function () {
            $(this).find('.hoveraction').fadeIn(400);
        }, function () {
            $(this).find('.hoveraction').hide();
        }
    );

    $('.quick-access-link').on('click', function(e) {
        e.preventDefault();
        const siteUrl = $(this).attr('data-site-url');
        
        axios({
            url: resolveClientUrl('Auth/GetTimeSSO'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            const finalUrl = siteUrl + '&time=' + response.data;
            window.open(finalUrl, '_blank');
        }).catch(axiosError);
    });
});