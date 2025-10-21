import React from 'react';
import ReactDOM from 'react-dom';
import DisciplineManager from './DisciplineManager';
import { PageType } from '../../../shared/constants';

ReactDOM.render(
    <DisciplineManager pageType={PageType.GroupTeam} objectId={document.getElementById('TeamId').value} />,
    document.getElementById('discipline-wrapper')
);
