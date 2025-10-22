import React from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';

const CONFIG_OPTIONS = [
    { value: "1", label: 'Basic Salary' },
    { value: "2", label: 'Basic Salary + Job Level Add-On' }
];

const BasicSalaryConfigModal = ({ 
    show, 
    onClose, 
    onSubmit, 
    selectedConfig, 
    onConfigChange 
}) => {
    const handleSubmit = () => {
        onSubmit(selectedConfig || null);
    };

    const handleRadioChange = value => {
        onConfigChange(value);
    };

    const renderRadioOption = option => (
        <div key={option.value} className="radio-option">
            <label>
                <input
                    type="radio"
                    name="config"
                    value={option.value}
                    checked={selectedConfig === option.value}
                    onChange={() => handleRadioChange(option.value)}
                />
                <span className="radio-label">{option.label}</span>
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
                    {CONFIG_OPTIONS.map(renderRadioOption)}
                </div>
            </ModalBody>
            
            <ModalFooter>
                <button
                    className="btn btn-primary"
                    onClick={handleSubmit}
                    disabled={!selectedConfig}
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
