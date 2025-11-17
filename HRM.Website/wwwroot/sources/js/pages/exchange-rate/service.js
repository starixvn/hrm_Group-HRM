import axios from 'axios';
import {
    blockUI,
    unblockUI,
    resolveClientUrl,
} from '../../shared/common.functions';

const ExchangeRateService = {
    getExchangeRates: () => {
        blockUI();
        return axios({
            url: resolveClientUrl('ExchangeRate/GetExchangeRates'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'json',
        })
            .then(response => {
                return response.data;
            })
            .finally(() => unblockUI());
    },
    getCurrencies: () => {
        return axios({
            url: resolveClientUrl('ExchangeRate/GetCurrencies'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'json',
        })
            .then(response => response.data || []);
    },
    createExchangeRate: (data) => {
        blockUI();
        return axios({
            url: resolveClientUrl('ExchangeRate/Create'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            data: data,
            responseType: 'json',
        })
            .finally(() => unblockUI());
    },
    updateExchangeRate: (currencyId, data) => {
        blockUI();
        return axios({
            url: resolveClientUrl('ExchangeRate/Update'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            data: {
                currencyId: currencyId,
                currencyCode: data.currencyCode,
                exchangeRate: data.exchangeRate,
                effectiveDate: data.effectiveDate,
                rounding: data.rounding,
            },
            responseType: 'json',
        })
            .finally(() => unblockUI());
    },
    deleteExchangeRate: (currencyId, effectiveDate) => {
        blockUI();
        return axios({
            url: resolveClientUrl('ExchangeRate/Delete'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            data: { 
                currencyId: currencyId,
                effectiveDate: effectiveDate
            },
            responseType: 'json',
        })
            .finally(() => unblockUI());
    },
    getRoundingRules: () => {
        return axios({
            url: resolveClientUrl('ExchangeRate/GetRoundingRules'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'json',
        })
            .then(response => response.data || []);
    },
};

export default ExchangeRateService;

