import React, { useEffect, useState, useCallback } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { Formik, Form, Field } from 'formik';
import { TagsInput } from 'react-tag-input-component';

import Errors from '../../../../shared/components/Errors';
import {
    resolveClientUrl,
    axiosError,
    toastOptions
} from '../../../../shared/common.functions';

import Currency from './Currency';
import SubCompany from './SubCompany';

const AppContainer = () => {
    const [formKey, setFormKey] = useState(0);
    const [whitelistIPs, setWhitelistIPs] = useState([]);
    const [currencyOptions, setCurrencyOptions] = useState([]);
    const [subCompanies, setSubCompanies] = useState([]);

    const isValidIP = (ip) => {
        const ipv4WithCIDR = /^(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}(\/([0-9]|[12][0-9]|3[0-2]))?$/;
        const ipv6WithCIDR = /^(([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4})(\/([0-9]|[1-9][0-9]|1[01][0-9]|12[0-8]))?$/;
        return ipv4WithCIDR.test(ip) || ipv6WithCIDR.test(ip);
    };

    const [initialValues, setInitialValues] = useState({
        companyId: '',
        name: '',
        hasBIO: false,
        enableLetstalk: false,
        currency: '',
        subCompany: [],
    });

    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Company/GetCurrency`),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
        })
        .then((res) => {
            setCurrencyOptions(res.data);
        })
        .catch(axiosError);
    }, []);

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Company/GetCompanyById/${window.companyId}`),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
        })
        .then((response) => {
            const data = response.data;
    
            let parsedIPs = [];
            if (data.whitelistIPs) {
                const rawIPs = JSON.parse(data.whitelistIPs);
                parsedIPs = rawIPs;
            }

            let parsedCurrency = [];
            if (data.currency) {
                parsedCurrency = data.currency.split(',').map(c => c.trim()).filter(c => c);
            }

            let subCompaniesData = [];
            if (data.subCompanies && Array.isArray(data.subCompanies)) {
                subCompaniesData = data.subCompanies.map(item => ({
                    subCompanyId: item.subCompanyId,
                    companyId: item.companyId,
                    name: item.name,
                    employeeCount: item.employeeCount || 0,
                    actionType: 'none'
                }));
            }
            setSubCompanies(subCompaniesData);

            setInitialValues({
                companyId: data.companyId || '',
                name: data.name || '',
                hasBIO: String(data.hasBIO).toLowerCase() === 'true',
                enableLetstalk: String(data.enableLetstalk).toLowerCase() === 'true',
                currency: parsedCurrency,
                subCompany: subCompaniesData,
            });

            setWhitelistIPs(parsedIPs);
            setFormKey((prev) => prev + 1);
        }).catch(axiosError);
    }, []);

    const handleWhitelistIPsChange = useCallback((newIPs) => {
        setWhitelistIPs(newIPs);
    }, []);

    const validateAndAddIP = useCallback((ip) => {
        if (!isValidIP(ip)) {
            toastr.warning('Invalid IP address format', '', toastOptions());
            return false;
        }

        if (whitelistIPs.includes(ip)) {
            toastr.info(`'${ip}' is already exists!`, '', toastOptions());
            return false;
        }

        return true;
    }, [whitelistIPs]);

    const handleSubmit = useCallback((values, { setSubmitting }) => {
        setSubmitting(true);

        const settingsJson = JSON.stringify({
            hasBIO: values.hasBIO ? 'true' : 'false',
            enableLetstalk: values.enableLetstalk ? 'true' : 'false',
            currency: Array.isArray(values.currency) && values.currency.length > 0 ? values.currency.join(',') : '',
            whitelistIPs: whitelistIPs.length > 0 ? JSON.stringify(whitelistIPs) : ''
        });

        const processedSubCompanies = subCompanies.map(company => ({
            subCompanyId: company.subCompanyId,
            companyId: company.companyId,
            name: company.name,
            actionType: company.actionType || 'none'
        }));

        axios.post(
            resolveClientUrl('company/edit'),
            {
                companyId: values.companyId,
                subCompanies: processedSubCompanies,
                settingsJson: settingsJson
            },
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
            }
        )
            .then((response) => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success('Updated successfully!', '', toastOptions());
                    setTimeout(() => {
                        document.location = resolveClientUrl('company/index');
                    }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
                    setSubmitting(false);
                }
            })
            .catch(axiosError);
    }, [whitelistIPs, subCompanies]);

    const renderForm = ({ errors, isSubmitting, values, setFieldValue }) => (
        <div className='card'>
            <Errors errors={errors} />
            <Form className='form-horizontal'>
                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>Company</label>
                    </div>
                    <div className='input-form'>
                        <Field
                            name='name'
                            className='form-control field-name'
                            autoComplete='off'
                            readOnly
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>BIO</label>
                    </div>
                    <div className='input-form'>
                        <Field name='hasBIO'>
                            {({ field }) => (
                                <input
                                    type='checkbox'
                                    className='checkbox'
                                    checked={field.value}
                                    onChange={(e) =>
                                        setFieldValue('hasBIO', e.target.checked)
                                    }
                                />
                            )}
                        </Field>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>Letstalk</label>
                    </div>
                    <div className='input-form'>
                        <Field name='enableLetstalk'>
                            {({ field }) => (
                                <input
                                    type='checkbox'
                                    className='checkbox'
                                    checked={field.value}
                                    onChange={(e) =>
                                        setFieldValue('enableLetstalk', e.target.checked)
                                    }
                                />
                            )}
                        </Field>
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>Currency</label>
                    </div>
                    <div className='input-form'>
                        <Currency
                            options={currencyOptions || []}
                            value={values.currency || []}
                            onChange={(selectedValues) => setFieldValue('currency', selectedValues)}
                            isDisabled={isSubmitting}
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>Whitelist IPs</label>
                    </div>
                    <div className='input-form flex-form'>
                        <TagsInput
                            value={whitelistIPs}
                            onChange={handleWhitelistIPsChange}
                            name='whitelistIPs'
                            placeHolder='Enter IP address and press Enter'
                            autoComplete='off'
                            beforeAddValidate={validateAndAddIP}
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <div className='label-form'>
                        <label className='control-label'>Sub Company</label>
                    </div>
                    <div className='input-form flex-form'>
                        <SubCompany 
                            companyId={values.companyId}
                            initialTags={subCompanies}
                            validTags={subCompanies.filter(company => company.actionType === 'none')}
                            onTagsChange={(newTags) => {
                                setSubCompanies(newTags);
                            }}
                        />
                    </div>
                </div>

                <div className='form-group'>
                    <button
                        type='submit'
                        className='btn btn-primary'
                        id='btnSubmit'
                        disabled={isSubmitting}
                    >
                        Update
                    </button>
                    <button
                        type='button'
                        className='btn btn-default'
                        onClick={handleGoBack}
                    >
                        Back
                    </button>
                </div>
            </Form>
        </div>
    );

    return (
        <Formik
            key={formKey}
            enableReinitialize
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={initialValues}
            onSubmit={handleSubmit}
        >
            {renderForm}
        </Formik>
    );
};

export default AppContainer;