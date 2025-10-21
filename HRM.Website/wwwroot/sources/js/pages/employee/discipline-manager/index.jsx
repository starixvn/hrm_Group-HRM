import React from 'react';
import ReactDOM from 'react-dom';
import DisciplineManager from '../../team/discipline-manager/DisciplineManager';
import { PageType } from '../../../shared/constants';

ReactDOM.render(
    <DisciplineManager pageType={PageType.Employee} objectId={document.getElementById('EmployeeId').value} />,
    document.getElementById('discipline-wrapper')
);