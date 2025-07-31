import React from 'react';
import ReactDOM from 'react-dom';
import Supervisor from '../../group-team/supervisor/Supervisor';

ReactDOM.render(
    <Supervisor pageType={2} objectId={document.getElementById('EmployeeId').value} />,
    document.getElementById('supervisor-wrapper')
);