import React from 'react';
import ReactDOM from 'react-dom';
import DisciplineManager from './DisciplineManager';

ReactDOM.render(
    <DisciplineManager pageType={1} objectId={document.getElementById('TeamId').value} />,
    document.getElementById('discipline-wrapper')
);