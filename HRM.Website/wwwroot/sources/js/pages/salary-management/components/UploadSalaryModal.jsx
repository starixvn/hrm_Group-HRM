import React, { useState } from "react";
import axios from "axios";
import toastr from "toastr";
import { Modal, ModalBody, ModalFooter, ModalHeader } from "react-bootstrap";
import {
    resolveClientUrl,
    toastOptions,
} from "../../../shared/common.functions";

const UploadSalaryModal = ({ onClose, onSuccess }) => {
    const [month, setMonth] = useState("");
    const [periodType, setPeriodType] = useState("1");
    const [file, setFile] = useState(null);
    const [isDragging, setIsDragging] = useState(false);
    const [isUploading, setIsUploading] = useState(false);

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

    const handleSubmit = async () => {
        if (!file || !month || !periodType) {
            toastr.error("Please fill all required fields", "", toastOptions());
            return;
        }

        const formData = new FormData();
        formData.append("file", file);
        formData.append("month", month);
        formData.append("periodType", periodType);

        setIsUploading(true);
        try {
            await axios.post(
                resolveClientUrl("SalaryManagement/UploadSalary"),
                formData,
                {
                    headers: {
                        "Content-Type": "multipart/form-data",
                    },
                }
            );
            toastr.success("Salary uploaded successfully", "", toastOptions());
            onSuccess();
        } catch (error) {
            toastr.error("Failed to upload salary", "", toastOptions());
        } finally {
            setIsUploading(false);
        }
    };

    return (
        <Modal show={true} onHide={onClose} centered>
            <ModalHeader closeButton>
                <Modal.Title>Upload Salary</Modal.Title>
            </ModalHeader>
            <ModalBody>
                <div className="form-group">
                    <label>Month</label>
                    <input
                        type="month"
                        className="form-control"
                        value={month}
                        onChange={(e) => setMonth(e.target.value)}
                        required
                    />
                </div>
                <div className="form-group">
                    <label>Period Type</label>
                    <select
                        className="form-control"
                        value={periodType}
                        onChange={(e) => setPeriodType(e.target.value)}
                        required>
                        <option value="1">1st Half (1-15)</option>
                        <option value="2">2nd Half (16-end)</option>
                        <option value="3">Full Month</option>
                    </select>
                </div>
                <div
                    className={`file-upload-area ${
                        isDragging ? "dragging" : ""
                    }`}
                    onDragOver={handleDragOver}
                    onDragLeave={handleDragLeave}
                    onDrop={handleDrop}>
                    <input
                        type="file"
                        onChange={handleFileChange}
                        className="file-input"
                    />
                    <div className="upload-text">
                        {file
                            ? file.name
                            : "Drag and drop file here or click to browse"}
                    </div>
                </div>
            </ModalBody>
            <ModalFooter>
                <button
                    className="btn btn-primary"
                    onClick={handleSubmit}
                    disabled={isUploading}>
                    {isUploading ? "Uploading..." : "Upload"}
                </button>
                <button className="btn btn-default" onClick={onClose}>
                    Cancel
                </button>
            </ModalFooter>
        </Modal>
    );
};

export default UploadSalaryModal;
