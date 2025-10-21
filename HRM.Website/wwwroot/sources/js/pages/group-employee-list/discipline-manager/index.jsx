import React from 'react';
import ReactDOM from 'react-dom';
import DisciplineManager from '../../group-team/discipline-manager/DisciplineManager';
import { PageType } from '../../../shared/constants';

ReactDOM.render(
    <DisciplineManager pageType={PageType.GroupTeam} objectId={document.getElementById('EmployeeId').value} />,
    document.getElementById('discipline-wrapper')
);