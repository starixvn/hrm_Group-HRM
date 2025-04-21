import React from 'react';
import ReactDOM from 'react-dom';
import AppContainer from './AppContainer';

const appContainer = document.getElementById('create-unit');
ReactDOM.render(<AppContainer nextId={appContainer.getAttribute('data-nextid')} />, appContainer);
