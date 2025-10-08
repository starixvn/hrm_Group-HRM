import React, { useState } from 'react';
import toastr from 'toastr';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';
import { toastOptions } from '../../../shared/common.functions';
import salaryService from '../service';
import MonthPicker from '../../../shared/components/MonthPicker';

const GenerateSalaryModal = ({ onSuccess }) => {
    const [show, setShow] = useState(false);
    const [month, setMonth] = useState('');
    const [isGenerating, setIsGenerating] = useState(false);
    const [showConfirm, setShowConfirm] = useState(false);
    const [confirmMessage, setConfirmMessage] = useState('');

    const handleClose = () => {
        setShow(false);
        setMonth('');
    };

    const handleOpen = () => {
        const now = new Date();
        const currentMonth = `${now.getFullYear()}-${String(
            now.getMonth() + 1
        ).padStart(2, '0')}`;
        setMonth(currentMonth);
        setShow(true);
    };

    const processGenerate = async () => {
        try {
            await salaryService.generatePayroll(`${month}-01`);
            toastr.success(
                'Payroll generated successfully',
                '',
                toastOptions()
            );
            onSuccess(month);
            handleClose();
        } catch (error) {
            toastr.error('Failed to generate Payroll', '', toastOptions());
        } finally {
            setIsGenerating(false);
            setShowConfirm(false);
        }
    };

    const handleSubmit = async () => {
        if (!month) {
            toastr.error('Please fill all required fields', '', toastOptions());
            return;
        }

        setConfirmMessage(
            `Are you sure you want to generate payroll for ${month}?`
        );
        setShowConfirm(true);
    };

    const handleConfirm = async () => {
        setIsGenerating(true);
        await processGenerate();
    };

    return (
        <React.Fragment>
            <button className="btn btn-upload" onClick={handleOpen}>
                Generate Payroll
            </button>

            <Modal
                show={show}
                onHide={handleClose}
                centered="true"
                backdrop={!showConfirm}
                className="upload-salary-modal">
                <ModalHeader closeButton>
                    <Modal.Title>Generate Payroll</Modal.Title>
                </ModalHeader>
                <ModalBody>
                    <div
                        className="form-group"
                        style={{
                            display: 'flex',
                            alignItems: 'center',
                            gap: '10px',
                        }}>
                        <label className="control-label">
                            Month<span className="required"> * </span>
                        </label>
                        <div className="input-form">
                            <MonthPicker
                                value={month}
                                onChange={setMonth}
                                placeholder="Select month for payroll..."
                                minDate="2020/01/01"
                                maxDate={new Date()}
                                style={{ position: 'relative', zIndex: 1 }}
                            />
                        </div>
                    </div>
                </ModalBody>
                <ModalFooter>
                    <button
                        className="btn btn-primary"
                        onClick={handleSubmit}
                        disabled={isGenerating}>
                        {isGenerating ? 'Generating...' : 'Generate'}
                    </button>
                    <button className="btn btn-default" onClick={handleClose}>
                        Cancel
                    </button>
                </ModalFooter>
            </Modal>

            <Modal
                centered="true"
                show={showConfirm}
                onHide={() => setShowConfirm(false)}
                className="confirm-modal"
                style={{ zIndex: 1060 }}>
                <ModalHeader closeButton>
                    <Modal.Title>Confirm Generate</Modal.Title>
                </ModalHeader>
                <ModalBody>{confirmMessage}</ModalBody>
                <ModalFooter>
                    <button className="btn btn-primary" onClick={handleConfirm}>
                        OK
                    </button>
                    <button
                        className="btn btn-default"
                        onClick={() => setShowConfirm(false)}>
                        Cancel
                    </button>
                </ModalFooter>
            </Modal>
        </React.Fragment>
    );
};

export default GenerateSalaryModal;
