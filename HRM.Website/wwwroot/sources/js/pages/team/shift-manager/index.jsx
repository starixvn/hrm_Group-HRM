import React from 'react';
import ReactDOM from 'react-dom';
import ShiftManager from './ShiftManager';
import { PageType } from '../../../shared/constants';

ReactDOM.render(
    <ShiftManager
        pageType={PageType.Team}
        objectId={document.getElementById('TeamId').value}
    />,
    document.getElementById('shift-wrapper')
);
