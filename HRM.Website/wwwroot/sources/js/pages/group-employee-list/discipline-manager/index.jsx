import React from 'react';
import ReactDOM from 'react-dom';
import DisciplineManager from '../../group-team/discipline-manager/DisciplineManager';

ReactDOM.render(
    <DisciplineManager pageType={2} objectId={document.getElementById('EmployeeId').value} />,
    document.getElementById('discipline-wrapper')
);