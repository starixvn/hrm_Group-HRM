import React, { useState, useEffect } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';
import Draggable from 'react-draggable';
import toastr from 'toastr';
import moment from 'moment';
import { toastOptions } from '../../../shared/common.functions';
import ExchangeRateService from '../service';
import { validationSchema } from '../common.functions';

const DATE_FORMAT = 'YYYY-MM-DD';

const ExchangeRateModal = ({ show, onClose, onSuccess, editData, isEdit = false }) => {
    const getTodayDateString = () => moment().format(DATE_FORMAT);

    const formatDateForInput = date => {
        if (!date) {
            return getTodayDateString();
        }
        let dateMoment;
        if (typeof date === 'string' && date.includes(',')) {
            const match = date.match(/(\d+)\s+(\w+)[,\s]+(\d+)/);
            if (match) {
                const day = match[1];
                const month = match[2];
                const year = match[3];
                dateMoment = moment(`${day} ${month} ${year}`, 'DD MMM YYYY', true);
            } else {
                dateMoment = moment(date);
            }
        } else {
            dateMoment = moment(date);
        }
        if (!dateMoment.isValid()) {
            return getTodayDateString();
        }
        return dateMoment.format(DATE_FORMAT);
    };

    const [formData, setFormData] = useState({
        currencyCode: '',
        exchangeRate: '',
        effectiveDate: getTodayDateString(),
        rounding: '0',
    });
    const [oldEffectiveDate, setOldEffectiveDate] = useState(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        if (show) {
            if (isEdit && editData) {
                const baseCurrency = editData.baseCurrency || '';
                const toUsd = editData.toUsd || '';
                const effectiveDate = editData.effectiveDate || '';
                const rounding = editData.rounding !== undefined ? editData.rounding : 0;

                setOldEffectiveDate(effectiveDate);
                setFormData({
                    currencyCode: baseCurrency,
                    exchangeRate: toUsd.toString(),
                    effectiveDate: formatDateForInput(effectiveDate),
                    rounding: rounding.toString(),
                });
            } else {
                setOldEffectiveDate(null);
                setFormData({
                    currencyCode: '',
                    exchangeRate: '',
                    effectiveDate: getTodayDateString(),
                    rounding: '0',
                });
            }
        }
    }, [show, isEdit, editData]);

    const handleChange = e => {
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
                ? `${parts[0]}.${parts.slice(1).join('')}`
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

    const formatDateWithTime = date => `${moment(date).format(DATE_FORMAT)}T12:00:00`;

    const getErrorMessage = (error, isEdit) => {
        const defaultMessage = `Failed to ${isEdit ? 'update' : 'add'} exchange rate`;
        
        if (error.inner && error.inner.length > 0) {
            return error.inner.map(err => err.message).join(', ');
        }
        if (error.response && error.response.data) {
            return error.response.data.message || error.response.data.Message || defaultMessage;
        }
        if (error.message) {
            return error.message;
        }
        return defaultMessage;
    };

    const prepareSubmitData = (validatedData) => {
        const submitData = {
            currencyCode: validatedData.currencyCode.toUpperCase(),
            exchangeRate: validatedData.exchangeRate,
            effectiveDate: formatDateWithTime(validatedData.effectiveDate),
            rounding: parseInt(formData.rounding) || 0,
        };

        if (isEdit && editData) {
            submitData.currencyId = editData.currencyId;
            if (oldEffectiveDate) {
                submitData.oldEffectiveDate = formatDateWithTime(oldEffectiveDate);
            }
        }

        return submitData;
    };

    const performSubmit = async (submitData) => {
        if (isEdit && editData) {
            await ExchangeRateService.updateExchangeRate(editData.currencyId, submitData);
            toastr.success('Exchange Rate updated successfully', '', toastOptions());
        } else {
            await ExchangeRateService.createExchangeRate(submitData);
            toastr.success('Exchange Rate added successfully', '', toastOptions());
        }
    };

    const handleSubmit = async e => {
        e.preventDefault();
        setIsSubmitting(true);

        try {
            const validatedData = await validationSchema.validate({
                currencyCode: formData.currencyCode.trim(),
                exchangeRate: parseFloat(formData.exchangeRate),
                effectiveDate: formData.effectiveDate,
            }, { abortEarly: false });

            const submitData = prepareSubmitData(validatedData);
            await performSubmit(submitData);

            onSuccess();
            onClose();
        } catch (error) {
            const errorMessage = getErrorMessage(error, isEdit);
            toastr.error(errorMessage, '', toastOptions());
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

export default ExchangeRateModal;

