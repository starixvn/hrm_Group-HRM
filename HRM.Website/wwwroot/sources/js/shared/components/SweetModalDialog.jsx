import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { Modal, Button } from 'react-bootstrap';
import Draggable from 'react-draggable';

const SweetModalDialog = ({ show, contents, onSubmit, onHide }) => {

    let title = '';
    switch (window.siteId) {
        case 1:
            title = 'SMS HRM';
            break;
        case 2:
            title = 'HM HRM';
            break;
        case 3:
            title = 'CTS HRM';
            break;
    };

    return (
        <Draggable handle=".modal-header" >
            <Modal keyboard={false} backdrop="static" show={show} onHide={onHide}>
                <Modal.Header>
                    <Modal.Title>{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>{contents}</Modal.Body>
                <Modal.Footer>
                    <Button bsStyle="primary" onClick={onSubmit}> OK </Button>
                    <Button className="btn-cancel" onClick={onHide}> Cancel </Button>
                </Modal.Footer>
            </Modal>
        </Draggable>
    );
};

SweetModalDialog.propTypes = {
    show: PropTypes.bool,
    contents: PropTypes.element.isRequired,
    onHide: PropTypes.func,
    onSubmit: PropTypes.func
};

SweetModalDialog.defaultProps = {
    show: false,
    contents: <Fragment />,
    onHide: function () {
        // This is intentional
    },
    onSubmit: function () {
        // This is intentional
    }
};

export default SweetModalDialog;