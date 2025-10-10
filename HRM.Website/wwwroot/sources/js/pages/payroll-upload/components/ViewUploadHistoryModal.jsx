import React, { useState } from 'react';
import { Modal, ModalBody, ModalHeader, ModalTitle } from 'react-bootstrap';
import toastr from 'toastr';
import { toastOptions } from '../../../shared/common.functions';
import { getPeriodTypeLabel } from '../constants/periodType';
import salaryService from '../service';

const ViewUploadHistoryModal = () => {
    const [show, setShow] = useState(false);
    const [history, setHistory] = useState([]);

    const handleClose = () => {
        setShow(false);
    };

    const fetchHistory = () => {
        salaryService
            .getUploadHistory()
            .then(data => {
                setHistory(data);
            })
            .catch(_ => {
                toastr.error(
                    'Failed to fetch upload history',
                    '',
                    toastOptions()
                );
            });
    };

    const handleDownload = async (savedFileName, fileName) => {
        try {
            window.location.href = salaryService.downloadFile(
                savedFileName,
                fileName
            );
        } catch (error) {
            toastr.error('Failed to download file', '', toastOptions());
        }
    };

    return (
        <React.Fragment>
            <button
                className="btn btn-default"
                onClick={() => {
                    setShow(true);
                    fetchHistory();
                }}>
                <i className="fa fa-history"></i> View History
            </button>

            <Modal
                show={show}
                onHide={handleClose}
                size="lg"
                className="view-history">
                <ModalHeader closeButton>
                    <ModalTitle>Upload History</ModalTitle>
                </ModalHeader>
                <ModalBody>
                    <div className="table-container">
                        <table className="table table-custom">
                            <thead>
                                <tr>
                                    <th>Month</th>
                                    <th>Period Type</th>
                                    <th>File Name</th>
                                    <th>Upload Date Time</th>
                                    <th>Uploaded By</th>
                                </tr>
                            </thead>
                            <tbody>
                                {history.map((item, index) => (
                                    <tr key={index}>
                                        <td>{item.monthFormatted}</td>
                                        <td>
                                            {getPeriodTypeLabel(
                                                item.periodType
                                            )}
                                        </td>
                                        <td>
                                            <a
                                                href="#"
                                                onClick={e => {
                                                    e.preventDefault();
                                                    handleDownload(
                                                        item.savedFileName,
                                                        item.fileName
                                                    );
                                                }}
                                                className="file-link">
                                                {item.fileName}
                                            </a>
                                        </td>
                                        <td>{item.clientCreatedTime}</td>
                                        <td>{item.nickName}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </ModalBody>
            </Modal>
        </React.Fragment>
    );
};

export default ViewUploadHistoryModal;

