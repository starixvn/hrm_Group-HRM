import axios from 'axios';
import {
    blockUI,
    unblockUI,
    resolveClientUrl,
} from '../../shared/common.functions';

const ExchangeRateService = {
    getExchangeRates: async () => {
        blockUI();
        try {
            const response = await axios({
                url: resolveClientUrl('ExchangeRate/GetExchangeRates'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'json',
            });
            return response.data;
        } finally {
            unblockUI();
        }
    },
    createExchangeRate: async (data) => {
        blockUI();
        try {
            return await axios({
                url: resolveClientUrl('ExchangeRate/Create'),
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                data: data,
                responseType: 'json',
            });
        } finally {
            unblockUI();
        }
    },
    updateExchangeRate: async (currencyId, data) => {
        blockUI();
        try {
            return await axios({
                url: resolveClientUrl('ExchangeRate/Update'),
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                data: {
                    currencyId: currencyId,
                    currencyCode: data.currencyCode,
                    exchangeRate: data.exchangeRate,
                    effectiveDate: data.effectiveDate,
                    oldEffectiveDate: data.oldEffectiveDate,
                    rounding: data.rounding,
                },
                responseType: 'json',
            });
        } finally {
            unblockUI();
        }
    },
    deleteExchangeRate: async (currencyId, effectiveDate) => {
        blockUI();
        try {
            return await axios({
                url: resolveClientUrl('ExchangeRate/Delete'),
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                data: {
                    currencyId: currencyId,
                    effectiveDate: effectiveDate
                },
                responseType: 'json',
            });
        } finally {
            unblockUI();
        }
    },
    getRoundingRules: async () => {
        const response = await axios({
            url: resolveClientUrl('ExchangeRate/GetRoundingRules'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'json',
        });
        return response.data || [];
    },
};

export default ExchangeRateService;

