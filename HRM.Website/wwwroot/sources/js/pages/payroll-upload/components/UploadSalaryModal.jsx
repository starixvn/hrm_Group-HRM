import React, { useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';
import {
    resolveClientUrl,
    toastOptions,
} from '../../../shared/common.functions';
import {
    PERIOD_TYPES,
    PeriodType,
    PeriodTypeLabel,
} from '../constants/periodType';
import salaryService from '../Service';

const UploadSalaryModal = ({ onSuccess }) => {
    const [show, setShow] = useState(false);
    const [month, setMonth] = useState('');
    const [periodType, setPeriodType] = useState(PeriodType.FULL_MONTH);
    const [file, setFile] = useState(null);
    const [isDragging, setIsDragging] = useState(false);
    const [isUploading, setIsUploading] = useState(false);
    const [showConfirm, setShowConfirm] = useState(false);
    const [confirmMessage, setConfirmMessage] = useState('');
    const [pendingFormData, setPendingFormData] = useState(null);

    const handleClose = () => {
        setShow(false);
        setMonth('');
        setPeriodType(PeriodType.FULL_MONTH);
        setFile(null);
    };

    const handleOpen = () => {
        const now = new Date();
        const currentMonth = `${now.getFullYear()}-${String(
            now.getMonth() + 1
        ).padStart(2, '0')}`;
        setMonth(currentMonth);
        setShow(true);
    };

    const handleFileChange = (e) => {
        setFile(e.target.files[0]);
    };

    const handleDragOver = (e) => {
        e.preventDefault();
        setIsDragging(true);
    };

    const handleDragLeave = () => {
        setIsDragging(false);
    };

    const handleDrop = (e) => {
        e.preventDefault();
        setIsDragging(false);
        setFile(e.dataTransfer.files[0]);
    };

    const handleMonthClick = (e) => {
        e.target.showPicker();
    };

    const processUpload = async (formData) => {
        try {
            await salaryService.uploadPayroll(formData);
            toastr.success('Payroll uploaded successfully', '', toastOptions());
            onSuccess();
            handleClose();
        } catch (error) {
            toastr.error('Failed to upload Payroll', '', toastOptions());
        } finally {
            setIsUploading(false);
            setShowConfirm(false);
            setPendingFormData(null);
        }
    };

    const handleSubmit = async () => {
        if (!file || !month || !periodType) {
            toastr.error('Please fill all required fields', '', toastOptions());
            return;
        }

        setIsUploading(true);

        try {
            const formData = new FormData();
            formData.append('file', file);
            formData.append('month', month);
            formData.append('periodType', periodType);

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
                setIsUploading(false);
            }
        } catch (error) {
            toastr.error('Failed to upload salary', '', toastOptions());
            setIsUploading(false);
        }
    };

    const handleConfirm = async () => {
        if (pendingFormData) {
            setIsUploading(true);
            await processUpload(pendingFormData);
        }
    };

    return (
        <React.Fragment>
            <button className="btn btn-upload" onClick={handleOpen}>
                Upload Payroll
            </button>

            <Modal
                show={show}
                onHide={handleClose}
                centered="true"
                backdrop={!showConfirm}
                className="upload-salary-modal">
                <ModalHeader closeButton>
                    <Modal.Title>Upload Payroll</Modal.Title>
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
                    <div className="form-group">
                        <label>
                            Period Type<span className="required"> *</span>
                        </label>
                        <select
                            className="form-control"
                            value={periodType}
                            onChange={(e) => setPeriodType(e.target.value)}
                            required>
                            {PERIOD_TYPES.map((type) => (
                                <option key={type} value={type}>
                                    {PeriodTypeLabel[type]}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>
                            File<span className="required"> *</span>
                        </label>
                        <div
                            className={`file-upload-area ${
                                isDragging ? 'dragging' : ''
                            }`}
                            onDragOver={handleDragOver}
                            onDragLeave={handleDragLeave}
                            onDrop={handleDrop}>
                            <input
                                type="file"
                                onChange={handleFileChange}
                                className="file-input"
                                multiple={false}
                                accept=".xlsx, .xls"
                            />
                            <div className="upload-text">
                                {file
                                    ? file.name
                                    : 'Drag and drop file here or click to browse'}
                                {!file && (
                                    <div className="upload-text-small">
                                        Support single Excel file (.xlsx, .xls)
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                </ModalBody>
                <ModalFooter>
                    <button
                        className="btn btn-primary"
                        onClick={handleSubmit}
                        disabled={isUploading}>
                        {isUploading ? 'Uploading...' : 'Upload'}
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
                    <Modal.Title>Confirm Upload</Modal.Title>
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

export default UploadSalaryModal;
