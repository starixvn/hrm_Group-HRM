import axios from 'axios';
import {
    blockUI,
    unblockUI,
    resolveClientUrl,
} from '../../shared/common.functions';

const PayrollService = {
    getSalaryData: (filters = {}) => {
        blockUI();
        return axios({
            url: resolveClientUrl('PayrollManagement/GetPayrollData'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                month: filters.month || null,
                isLocal: filters.isLocal || false,
            },
            responseType: 'json',
        })
            .then(response => response.data)
            .finally(() => unblockUI());
    },

    generatePayroll: month => {
        blockUI();
        return axios({
            url: resolveClientUrl('PayrollManagement/GeneratePayroll'),
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                month: month || null,
            },
            responseType: 'json',
        })
            .then(response => response.data)
            .finally(() => unblockUI());
    },
};

export default PayrollService;
