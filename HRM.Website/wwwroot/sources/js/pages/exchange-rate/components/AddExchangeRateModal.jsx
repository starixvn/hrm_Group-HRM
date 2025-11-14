import React, { useState, useEffect } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';
import Draggable from 'react-draggable';
import toastr from 'toastr';
import { toastOptions } from '../../../shared/common.functions';
import ExchangeRateService from '../service';

const AddExchangeRateModal = ({ show, onClose, onSuccess }) => {
    const [formData, setFormData] = useState({
        currencyCode: '',
        exchangeRate: '',
        effectiveDate: new Date().toISOString().split('T')[0],
    });
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        if (show) {
            setFormData({
                currencyCode: '',
                exchangeRate: '',
                effectiveDate: new Date().toISOString().split('T')[0],
            });
        }
    }, [show]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value.toUpperCase(),
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (!formData.currencyCode || !formData.exchangeRate || !formData.effectiveDate) {
            toastr.error('Please fill all required fields', '', toastOptions());
            return;
        }

        if (formData.currencyCode.length > 3) {
            toastr.error('Currency code must be 3 characters or less', '', toastOptions());
            return;
        }

        const exchangeRateValue = parseFloat(formData.exchangeRate);
        if (isNaN(exchangeRateValue) || exchangeRateValue <= 0) {
            toastr.error('Exchange rate must be a positive number', '', toastOptions());
            return;
        }

        setIsSubmitting(true);
        try {
            await ExchangeRateService.createExchangeRate({
                currencyCode: formData.currencyCode.trim().toUpperCase(),
                exchangeRate: exchangeRateValue,
                effectiveDate: formData.effectiveDate,
            });
            toastr.success('Exchange rate added successfully', '', toastOptions());
            onSuccess();
            onClose();
        } catch (error) {
            const errorMessage = (error.response && error.response.data && (error.response.data.message || error.response.data.Message)) || 'Failed to add exchange rate';
            toastr.error(errorMessage, '', toastOptions());
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <Draggable handle=".modal-header">
            <Modal show={show} onHide={onClose} centered backdrop="static" size="lg">
                <ModalHeader closeButton>
                    <Modal.Title>Add Exchange Rate</Modal.Title>
                </ModalHeader>
                <form onSubmit={handleSubmit}>
                    <ModalBody>
                        <div className="form-group">
                            <label>
                                Currency Code<span className="required"> *</span>
                            </label>
                            <input
                                type="text"
                                className="form-control"
                                name="currencyCode"
                                value={formData.currencyCode}
                                onChange={handleChange}
                                maxLength="3"
                                required
                                placeholder="USD, EUR, etc."
                                style={{ textTransform: 'uppercase' }}
                            />
                            <small className="form-text text-muted">Enter 3-letter currency code (e.g., USD, EUR, VND)</small>
                        </div>
                        <div className="form-group">
                            <label>
                                Exchange Rate (1 unit to USD)<span className="required"> *</span>
                            </label>
                            <input
                                type="number"
                                className="form-control"
                                name="exchangeRate"
                                value={formData.exchangeRate}
                                onChange={handleChange}
                                step="0.00000001"
                                min="0.00000001"
                                required
                                placeholder="0.00000000"
                            />
                        </div>
                        <div className="form-group">
                            <label>
                                Effective Date<span className="required"> *</span>
                            </label>
                            <input
                                type="date"
                                className="form-control"
                                name="effectiveDate"
                                value={formData.effectiveDate}
                                onChange={handleChange}
                                required
                            />
                        </div>
                    </ModalBody>
                    <ModalFooter>
                        <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
                            {isSubmitting ? 'Submitting...' : 'Submit'}
                        </button>
                        <button type="button" className="btn btn-secondary" onClick={onClose} disabled={isSubmitting}>
                            Cancel
                        </button>
                    </ModalFooter>
                </form>
            </Modal>
        </Draggable>
    );
};

export default AddExchangeRateModal;

