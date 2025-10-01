import React from 'react';
import ReactDOM from 'react-dom';
import LeaderTakeoverManager from './LeaderTakeoverManager';

ReactDOM.render(
    <LeaderTakeoverManager pageType={1} objectId={document.getElementById('TeamId').value} />,
    document.getElementById('leader-takeover-manager')
);