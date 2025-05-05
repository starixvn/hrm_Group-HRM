import React, { useState, useEffect } from "react";
import axios from "axios";
import toastr from "toastr";
import { Modal, ModalBody, ModalFooter, ModalHeader } from "react-bootstrap";
import {
    resolveClientUrl,
    toastOptions,
} from "../../../shared/common.functions";

const ViewUploadHistoryModal = ({ onClose }) => {
    const [history, setHistory] = useState([]);

    useEffect(() => {
        fetchHistory();
    }, []);

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

    const handleDownload = async (fileName) => {
        try {
            const response = await axios.get(
                resolveClientUrl(
                    `SalaryManagement/DownloadFile?fileName=${encodeURIComponent(
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
        <Modal show={true} onHide={onClose} size="lg" centered>
            <ModalHeader closeButton>
                <Modal.Title>Upload History</Modal.Title>
            </ModalHeader>
            <ModalBody>
                <div className="table-responsive">
                    <table className="table table-striped">
                        <thead>
                            <tr>
                                <th>File Name</th>
                                <th>Upload Date Time</th>
                                <th>Uploaded By</th>
                                <th>Month</th>
                                <th>Period Type</th>
                            </tr>
                        </thead>
                        <tbody>
                            {history.map((item, index) => (
                                <tr key={index}>
                                    <td>
                                        <a
                                            href="#"
                                            onClick={(e) => {
                                                e.preventDefault();
                                                handleDownload(item.fileName);
                                            }}
                                            className="file-link">
                                            {item.fileName}
                                        </a>
                                    </td>
                                    <td>{item.uploadDateTime}</td>
                                    <td>{item.uploadedBy}</td>
                                    <td>{item.month}</td>
                                    <td>
                                        {item.periodType === "1"
                                            ? "1st Half"
                                            : "2nd Half"}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </ModalBody>
            <ModalFooter>
                <button className="btn btn-default" onClick={onClose}>
                    Close
                </button>
            </ModalFooter>
        </Modal>
    );
};

export default ViewUploadHistoryModal;
