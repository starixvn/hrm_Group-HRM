import React from 'react';
import ReactDOM from 'react-dom';
import Supervisor from './Supervisor';

ReactDOM.render(
    <Supervisor pageType={1} objectId={document.getElementById('TeamId').value} />,
    document.getElementById('supervisor-wrapper')
);