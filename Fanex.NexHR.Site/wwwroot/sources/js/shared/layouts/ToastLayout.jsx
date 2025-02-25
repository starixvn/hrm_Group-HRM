import React, { PureComponent } from 'react';
import { ToastContainer, Slide } from 'react-toastify';

export default class ToastLayout extends PureComponent {
    render() {
        return (
            <ToastContainer
                position="top-center"
                autoClose={3000}
                hideProgressBar={false}
                transition={Slide}
                closeOnClick={false}
                draggable={false}
            >
                {this.props.children}
            </ToastContainer>
        );
    }
}