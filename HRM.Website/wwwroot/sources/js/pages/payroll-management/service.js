import axios from "axios";
import { resolveClientUrl, toastOptions } from "../../shared/common.functions";
import toastr from "toastr";

const PayrollService = {
    /**
     * Get salary data with optional filters
     * @param {Object} filters - Optional filters for salary data
     * @param {string} filters.month - Month in YYYY-MM format
     * @param {number} filters.periodType - Period type (1: FULL_MONTH, 2: FIRST_HALF, 3: SECOND_HALF)
     * @returns {Promise} Promise with salary data
     */
    getSalaryData: async (filters = {}) => {
        try {
            const response = await axios.get(
                resolveClientUrl("PayrollManagement/GetPayrollData"),
                {
                    params: {
                        month: filters.month || null,
                        periodType: filters.periodType || null,
                        employeeName: filters.employeeName || null,
                    },
                }
            );
            return response.data;
        } catch (error) {
            toastr.error("Failed to fetch salary data", "", toastOptions());
            throw error;
        }
    },

    /**
     * Get upload history
     * @returns {Promise} Promise with upload history data
     */
    getUploadHistory: async () => {
        try {
            const response = await axios.get(
                resolveClientUrl("PayrollManagement/GetUploadHistory")
            );
            return response.data;
        } catch (error) {
            toastr.error("Failed to fetch upload history", "", toastOptions());
            throw error;
        }
    },

    /**
     * Upload salary file
     * @param {FormData} formData - Form data containing file, month and periodType
     * @returns {Promise} Promise with upload result
     */
    uploadPayroll: async (formData) => {
        try {
            const response = await axios.post(
                resolveClientUrl("PayrollManagement/UploadPayroll"),
                formData,
                {
                    headers: {
                        "Content-Type": "multipart/form-data",
                    },
                }
            );
            return response.data;
        } catch (error) {
            toastr.error("Failed to upload salary", "", toastOptions());
            throw error;
        }
    },

    /**
     * Confirm salary upload
     * @param {FormData} formData - Form data containing month and periodType
     * @returns {Promise} Promise with confirmation result
     */
    confirmUpload: async (formData) => {
        try {
            const response = await axios.post(
                resolveClientUrl("PayrollManagement/ConfirmUpload"),
                formData
            );
            return response.data;
        } catch (error) {
            toastr.error("Failed to confirm upload", "", toastOptions());
            throw error;
        }
    },

    /**
     * Download salary file
     * @param {string} savedFileName - Name of the file to download
     * @param {string} fileName - Name of the file to download
     * @returns {string} URL of the file to download
     */
    downloadFile: (savedFileName, fileName) =>
        `/PayrollManagement/DownloadFile?savedFileName=${savedFileName}&fileName=${fileName}`,
};

export default PayrollService;
