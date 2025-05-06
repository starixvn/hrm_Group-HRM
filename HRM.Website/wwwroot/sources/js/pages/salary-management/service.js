import axios from "axios";
import { resolveClientUrl, toastOptions } from "../../shared/common.functions";
import toastr from "toastr";

const salaryService = {
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
                resolveClientUrl("SalaryManagement/GetSalaryData"),
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
                resolveClientUrl("SalaryManagement/GetUploadHistory")
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
    uploadSalary: async (formData) => {
        try {
            const response = await axios.post(
                resolveClientUrl("SalaryManagement/UploadSalary"),
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
                resolveClientUrl("SalaryManagement/ConfirmUpload"),
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
     * @param {string} fileName - Name of the file to download
     * @returns {Promise} Promise with file blob
     */
    downloadFile: async (fileName) => {
        try {
            const response = await axios.get(
                resolveClientUrl(
                    `SalaryManagement/DownloadFile?fileName=${fileName}`
                ),
                { responseType: "blob" }
            );
            return response.data;
        } catch (error) {
            toastr.error("Failed to download file", "", toastOptions());
            throw error;
        }
    },
};

export default salaryService;
