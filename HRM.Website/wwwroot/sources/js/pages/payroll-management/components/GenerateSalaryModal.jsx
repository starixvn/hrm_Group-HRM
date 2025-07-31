import React, { useState } from 'react';
import toastr from 'toastr';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';
import { toastOptions } from '../../../shared/common.functions';
import salaryService from '../Service';

const GenerateSalaryModal = ({ onSuccess }) => {
    const [show, setShow] = useState(false);
    const [month, setMonth] = useState('');
    const [isGenerating, setIsGenerating] = useState(false);
    const [showConfirm, setShowConfirm] = useState(false);
    const [confirmMessage, setConfirmMessage] = useState('');
    const [pendingFormData, setPendingFormData] = useState(null);

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

    const handleMonthClick = (e) => {
        e.target.showPicker();
    };

    const processUpload = async (formData) => {
        try {
            await salaryService.uploadPayroll(formData);
            toastr.success(
                'Payroll generated successfully',
                '',
                toastOptions()
            );
            onSuccess();
            handleClose();
        } catch (error) {
            toastr.error('Failed to generate Payroll', '', toastOptions());
        } finally {
            setIsGenerating(false);
            setShowConfirm(false);
            setPendingFormData(null);
        }
    };

    const handleSubmit = async () => {
        if (!month) {
            toastr.error('Please fill all required fields', '', toastOptions());
            return;
        }

        setIsGenerating(true);

        try {
            const formData = new FormData();
            formData.append('month', month);

            const confirmResult = await salaryService.confirmUpload(formData);

            if (confirmResult.isSuccessful) {
                await processUpload(formData);
            } else {
                setPendingFormData(formData);
                setConfirmMessage(
                    confirmResult.message ||
                        'Are you sure you want to proceed with the upload?'
                );
                setShowConfirm(true);
                setIsGenerating(false);
            }
        } catch (error) {
            toastr.error('Failed to upload salary', '', toastOptions());
            setIsGenerating(false);
        }
    };

    const handleConfirm = async () => {
        if (pendingFormData) {
            setIsGenerating(true);
            await processUpload(pendingFormData);
        }
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
                    <div className="form-group">
                        <label>
                            Month<span className="required"> *</span>
                        </label>
                        <input
                            type="month"
                            className="form-control"
                            value={month}
                            onChange={(e) => setMonth(e.target.value)}
                            required
                            onClick={handleMonthClick}
                        />
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
                show={showConfirm}
                onHide={() => setShowConfirm(false)}
                zIndex={1050}>
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
