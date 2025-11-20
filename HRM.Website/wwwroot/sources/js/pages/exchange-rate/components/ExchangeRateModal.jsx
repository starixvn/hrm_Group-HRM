import React, { useState, useEffect } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'react-bootstrap';
import Draggable from 'react-draggable';
import axios from 'axios';
import toastr from 'toastr';
import moment from 'moment';
import { toastOptions, resolveClientUrl, axiosError } from '../../../shared/common.functions';
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
                const [, day, month, year] = match;
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
    });
    const [oldEffectiveDate, setOldEffectiveDate] = useState(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        if (show) {
            if (isEdit && editData) {
                const baseCurrency = editData.baseCurrency || '';
                const toUsd = editData.toUsd || '';
                const effectiveDate = editData.effectiveDate || '';

                setOldEffectiveDate(effectiveDate);
                setFormData({
                    currencyCode: baseCurrency,
                    exchangeRate: toUsd.toString(),
                    effectiveDate: formatDateForInput(effectiveDate),
                });
            } else {
                setOldEffectiveDate(null);
                setFormData({
                    currencyCode: '',
                    exchangeRate: '',
                    effectiveDate: getTodayDateString(),
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

    const prepareSubmitData = validatedData => {
        const submitData = {
            currencyCode: validatedData.currencyCode.toUpperCase(),
            exchangeRate: validatedData.exchangeRate,
            effectiveDate: formatDateWithTime(validatedData.effectiveDate),
        };

        if (isEdit && editData) {
            submitData.currencyId = editData.currencyId;
            if (oldEffectiveDate) {
                submitData.oldEffectiveDate = formatDateWithTime(oldEffectiveDate);
            }
        }

        return submitData;
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
            const url = isEdit && editData 
                ? resolveClientUrl('ExchangeRate/Update')
                : resolveClientUrl('ExchangeRate/Create');
            
            const requestData = isEdit && editData
                ? {
                    currencyId: editData.currencyId,
                    currencyCode: submitData.currencyCode,
                    exchangeRate: submitData.exchangeRate,
                    effectiveDate: submitData.effectiveDate,
                    oldEffectiveDate: submitData.oldEffectiveDate,
                }
                : submitData;

            axios.post(url, requestData, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success(
                        isEdit ? 'Exchange Rate updated successfully!' : 'Exchange Rate added successfully!',
                        '',
                        toastOptions()
                    );
                    onSuccess();
                    onClose();
                } else {
                    toastr.error(data.message, '', toastOptions());
                    setIsSubmitting(false);
                }
            }).catch(axiosError);
        } catch (error) {
            const errorMessage = getErrorMessage(error, isEdit);
            toastr.error(errorMessage, '', toastOptions());
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
                                placeholder="EUR, VND, etc."
                                style={{ textTransform: 'uppercase' }}
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
                            Submit
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

