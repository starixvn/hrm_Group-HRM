import React, { useState, useEffect } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';
import Draggable from 'react-draggable';
import toastr from 'toastr';
import { toastOptions } from '../../../shared/common.functions';
import ExchangeRateService from '../service';
import { validationSchema } from '../common.functions';

const AddExchangeRateModal = ({ show, onClose, onSuccess, editData, isEdit = false }) => {
    const [formData, setFormData] = useState({
        currencyCode: '',
        exchangeRate: '',
        effectiveDate: new Date().toISOString().split('T')[0],
        rounding: '0',
    });
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        if (show) {
            if (isEdit && editData) {
                const baseCurrency = editData.baseCurrency || '';
                const toUsd = editData.toUsd || '';
                const effectiveDate = editData.effectiveDate || '';
                const rounding = editData.rounding !== undefined ? editData.rounding : 0;
                
                let formattedDate = new Date().toISOString().split('T')[0];
                if (effectiveDate) {
                    try {
                        if (typeof effectiveDate === 'string') {
                            const dateStr = effectiveDate.split('T')[0].split(' ')[0].trim();
                            const dateObj = new Date(dateStr);
                            if (!isNaN(dateObj.getTime())) {
                                formattedDate = dateObj.toISOString().split('T')[0];
                            } else {
                                console.warn('Invalid date string:', effectiveDate);
                            }
                        } else if (effectiveDate instanceof Date) {
                            if (!isNaN(effectiveDate.getTime())) {
                                formattedDate = effectiveDate.toISOString().split('T')[0];
                            } else {
                                console.warn('Invalid Date object:', effectiveDate);
                            }
                        } else {
                            const dateObj = new Date(effectiveDate);
                            if (!isNaN(dateObj.getTime())) {
                                formattedDate = dateObj.toISOString().split('T')[0];
                            } else {
                                console.warn('Invalid date value:', effectiveDate);
                            }
                        }
                    } catch (error) {
                        console.error('Error parsing effectiveDate:', error, 'effectiveDate:', effectiveDate);
                    }
                } else {
                    console.warn('effectiveDate is empty or undefined in editData:', editData);
                }
                
                setFormData({
                    currencyCode: baseCurrency,
                    exchangeRate: toUsd.toString(),
                    effectiveDate: formattedDate,
                    rounding: rounding.toString(),
                });
            } else {
                setFormData({
                    currencyCode: '',
                    exchangeRate: '',
                    effectiveDate: new Date().toISOString().split('T')[0],
                    rounding: '0',
                });
            }
        }
    }, [show, isEdit, editData]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        if (name === 'currencyCode') {
            setFormData(prev => ({
                ...prev,
                [name]: value.toUpperCase(),
            }));
        } else if (name === 'exchangeRate') {
            const sanitizedValue = value.replace(/[^0-9.]/g, '');
            const parts = sanitizedValue.split('.');
            const formattedValue = parts.length > 2
                ? parts[0] + '.' + parts.slice(1).join('')
                : sanitizedValue;
            setFormData(prev => ({
                ...prev,
                [name]: formattedValue,
            }));
        } else {
            setFormData(prev => ({
                ...prev,
                [name]: value,
            }));
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setIsSubmitting(true);
        try {
            // Validate using Yup schema
            const validatedData = await validationSchema.validate({
                currencyCode: formData.currencyCode.trim(),
                exchangeRate: parseFloat(formData.exchangeRate),
                effectiveDate: formData.effectiveDate,
            }, { abortEarly: false });

            const submitData = {
                currencyCode: validatedData.currencyCode.toUpperCase(),
                exchangeRate: validatedData.exchangeRate,
                effectiveDate: validatedData.effectiveDate,
            };

            // Add rounding (default to 0 if empty, ensure it's a smallint)
            const roundingValue = formData.rounding !== undefined && formData.rounding !== ''
                ? parseInt(formData.rounding, 10)
                : 0;
            // Ensure rounding is within smallint range (-32,768 to 32,767)
            submitData.rounding = Math.max(-32768, Math.min(32767, roundingValue));

            if (isEdit && editData) {
                const id = editData.currencyId;
                await ExchangeRateService.updateExchangeRate(id, submitData);
                toastr.success('Exchange rate updated successfully', '', toastOptions());
            } else {
                await ExchangeRateService.createExchangeRate(submitData);
                toastr.success('Exchange rate added successfully', '', toastOptions());
            }
            onSuccess();
            onClose();
        } catch (error) {
            if (error.inner && error.inner.length > 0) {
                // Yup validation errors
                const errorMessage = error.inner.map(err => err.message).join(', ');
                toastr.error(errorMessage, '', toastOptions());
            } else {
                // API errors
                const errorMessage = (error.response && error.response.data && (error.response.data.message || error.response.data.Message)) || 'Failed to add exchange rate';
                toastr.error(errorMessage, '', toastOptions());
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <Draggable handle=".modal-header">
            <Modal show={show} onHide={onClose} dialogClassName="modal-dialog-centered" backdrop="static" size="lg">
                <ModalHeader closeButton>
                    <Modal.Title>{isEdit ? 'Edit Exchange Rate' : 'Add Exchange Rate'}</Modal.Title>
                </ModalHeader>
                <form onSubmit={handleSubmit}>
                    <ModalBody>
                        <div className="form-group">
                            <label>
                                Currency<span className="required"> *</span>
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
                        </div>
                        <div className="form-group">
                            <label>
                                Rounding
                            </label>
                            <input
                                type="number"
                                className="form-control"
                                name="rounding"
                                value={formData.rounding}
                                onChange={handleChange}
                                placeholder="e.g., -4, -2, -1, 0, 2"
                            />
                        </div>
                        <div className="form-group">
                            <label>
                                Exchange Rate (1 unit to USD)<span className="required"> *</span>
                            </label>
                            <input
                                type="text"
                                className="form-control"
                                name="exchangeRate"
                                value={formData.exchangeRate}
                                onChange={handleChange}
                                required
                                placeholder="0.00000000"
                                inputMode="decimal"
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
                            {isSubmitting ? (isEdit ? 'Updating...' : 'Submitting...') : (isEdit ? 'Update' : 'Submit')}
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

