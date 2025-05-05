import React, { useState } from "react";
import axios from "axios";
import toastr from "toastr";
import { Modal } from "react-bootstrap";
import {
    resolveClientUrl,
    toastOptions,
} from "../../../shared/common.functions";
import UploadSalaryModal from "./UploadSalaryModal";
import ViewUploadHistoryModal from "./ViewUploadHistoryModal";

const Action = ({ onUploadSuccess, onUploadHistorySuccess }) => {
    const [showUploadModal, setShowUploadModal] = useState(false);
    const [showHistory, setShowHistory] = useState(false);

    const handleUploadSuccess = () => {
        onUploadSuccess();
        onUploadHistorySuccess();
        setShowUploadModal(false);
    };

    return (
        <div className="upload-section">
            <div className="upload-actions">
                <button
                    className="btn btn-primary"
                    onClick={() => setShowUploadModal(true)}>
                    Upload Salary
                </button>
                <button
                    className="btn btn-default"
                    onClick={() => setShowHistory(true)}>
                    <i className="fa fa-history"></i> View History
                </button>
            </div>

            <Modal
                show={showUploadModal}
                onHide={() => setShowUploadModal(false)}
                size="lg"
                centered>
                <UploadSalaryModal
                    onClose={() => setShowUploadModal(false)}
                    onSuccess={handleUploadSuccess}
                />
            </Modal>

            <Modal
                show={showHistory}
                onHide={() => setShowHistory(false)}
                size="lg"
                centered>
                <ViewUploadHistoryModal onClose={() => setShowHistory(false)} />
            </Modal>
        </div>
    );
};

export default Action;
