import React from 'react';
import ReactDOM from 'react-dom';
import ShiftManager from './ShiftManager';

ReactDOM.render(
    <ShiftManager pageType={1} objectId={document.getElementById('TeamId').value} />,
    document.getElementById('shift-wrapper')
);