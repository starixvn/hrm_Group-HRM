import React from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';

const BasicSalaryConfigModal = ({
    show,
    onClose,
    onSubmit,
    selectedConfigs = [],
    onConfigChange,
    configOptions = []
}) => {
    const isOptionSelected = value => selectedConfigs.includes(value);

    const toggleOption = value => {
        const isSelected = isOptionSelected(value);
        
        if (isSelected) {
            onConfigChange(selectedConfigs.filter(config => config !== value));
        } else {
            onConfigChange([...selectedConfigs, value]);
        }
    };

    const handleSubmit = () => {
        onSubmit(selectedConfigs);
    };

    const isSubmitDisabled = selectedConfigs.length === 0;

    const renderOption = option => (
        <div key={option.value} className="checkbox-option">
            <label>
                <input
                    type="checkbox"
                    value={option.value}
                    checked={isOptionSelected(option.value)}
                    onChange={() => toggleOption(option.value)}
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

            <ModalFooter>
                <button
                    className="btn btn-primary"
                    onClick={handleSubmit}
                    disabled={isSubmitDisabled}
                >
                    Submit
                </button>
                <button
                    className="btn btn-default"
                    onClick={onClose}
                >
                    Cancel
                </button>
            </ModalFooter>
        </Modal>
    );
};

export default BasicSalaryConfigModal;
