import { object, string, number } from 'yup';
import axios from 'axios';
import toastr from 'toastr';
import { resolveClientUrl, axiosError, toastOptions } from '../../shared/common.functions';

export const validationSchema = object({
    code: string()
        .trim()
        .max(50, 'Business Unit Code field must not over 50 characters long!')
        .required('Business Unit Code field is required!'),
    groupUnitId: number()
        .required('Business Unit ID field is required!'),
    category: string()
        .max(200, 'Business Unit Category field must not over 200 characters long!'),
    type: string()
        .max(200, 'Business Unit Type field must not over 200 characters long!'),
    accountableArea: string()
        .max(200, 'Accountable Area field must not over 200 characters long!'),
});

export const submitUnit = (endpoint, model, setSubmitting) => {
    setSubmitting(true);
    
    axios
        .post(
            resolveClientUrl(endpoint),
            model,
            {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }
        )
        .then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                const successMessage = endpoint.includes('Create') ? "Created successfully!" : "Updated successfully!";
                toastr.success(successMessage, '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl('Unit/Index');
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        })
        .catch(axiosError);
};