import React from 'react';
import ReactDOM from 'react-dom';
import LeaderTakeoverManager from './LeaderTakeoverManager';
import { PageType } from '../../../shared/constants';

const getPageType = () => {
    const currentPath = window.location.pathname.toLowerCase();
    if (currentPath.includes('/groupteam/')) {
        return PageType.GroupTeam;
    }
    return PageType.Team;
};

ReactDOM.render(
    <LeaderTakeoverManager pageType={getPageType()} objectId={document.getElementById('TeamId').value} />,
    document.getElementById('leader-takeover-manager')
);