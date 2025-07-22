import React from 'react';
import ReactDOM from 'react-dom';
import ShiftManager from '../../group-team/shift-manager/ShiftManager';

ReactDOM.render(
    <ShiftManager pageType={2} objectId={document.getElementById('EmployeeId').value} />,
    document.getElementById('shift-wrapper')
);