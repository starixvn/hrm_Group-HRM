import React, { useState, useEffect } from "react";
import axios from "axios";
import toastr from "toastr";
import {
    Modal,
    ModalBody,
    ModalFooter,
    ModalHeader,
    ModalTitle,
} from "react-bootstrap";
import {
    resolveClientUrl,
    toastOptions,
} from "../../../shared/common.functions";
import { getPeriodTypeLabel } from "../constants/periodType";

const ViewUploadHistoryModal = () => {
    const [show, setShow] = useState(false);
    const [history, setHistory] = useState([]);

    const handleClose = () => {
        setShow(false);
    };

    const fetchHistory = () => {
        axios
            .get(resolveClientUrl("SalaryManagement/GetUploadHistory"))
            .then((response) => {
                setHistory(response.data);
            })
            .catch((error) => {
                toastr.error(
                    "Failed to fetch upload history",
                    "",
                    toastOptions()
                );
            });
    };

    const handleDownload = async (savedFileName, fileName) => {
        try {
            const response = await axios.get(
                resolveClientUrl(
                    `SalaryManagement/DownloadFile?savedFileName=${encodeURIComponent(
                        savedFileName
                    )}&fileName=${encodeURIComponent(
                        fileName
                    )}`
                ),
                { responseType: "blob" }
            );

            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement("a");
            link.href = url;
            link.setAttribute("download", fileName);
            document.body.appendChild(link);
            link.click();
            link.remove();
            window.URL.revokeObjectURL(url);
        } catch (error) {
            toastr.error("Failed to download file", "", toastOptions());
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
                centered
                className="view-history">
                <ModalHeader closeButton>
                    <ModalTitle>Upload History</ModalTitle>
                </ModalHeader>
                <ModalBody>
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
                                        {getPeriodTypeLabel(item.periodType)}
                                    </td>
                                    <td>
                                        <a
                                            href="#"
                                            onClick={(e) => {
                                                e.preventDefault();
                                                handleDownload(item.savedFileName, item.fileName);
                                            }}
                                            className="file-link">
                                            {item.fileName}
                                        </a>
                                    </td>
                                    <td>{item.createdDate}</td>
                                    <td>{item.nickName}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </ModalBody>
            </Modal>
        </React.Fragment>
    );
};

export default ViewUploadHistoryModal;
