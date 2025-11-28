import React from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';

const BasicSalaryConfigModal = ({
    show,
    onClose,
    selectedConfigs = [],
    configOptions = []
}) => {
    const isOptionSelected = value => selectedConfigs.includes(value);

    const renderOption = option => (
        <div key={option.value} className="checkbox-option">
            <label>
                <input
                    type="checkbox"
                    value={option.value}
                    checked={isOptionSelected(option.value)}
                />
                <span className="checkbox-label">{option.label}</span>
            </label>
        </div>
    );

    return (
        <Modal
            show={show}
            onHide={onClose}
            className="config-modal"
        >
            <ModalHeader closeButton>
                <Modal.Title>Basic Salary Configuration</Modal.Title>
            </ModalHeader>

            <ModalBody>
                <div className="form-group">
                    {configOptions.map(renderOption)}
                </div>
            </ModalBody>
        </Modal>
    );
};

export default BasicSalaryConfigModal;
