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
                // Handle both old format (array) and new format (object with ExchangeRates and RoundingRules)
                if (Array.isArray(response.data)) {
                    return {
                        exchangeRates: response.data,
                        roundingRules: []
                    };
                }
                // Return data as-is (ASP.NET Core uses PascalCase by default)
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
};

export default ExchangeRateService;

