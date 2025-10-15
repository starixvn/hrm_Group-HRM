import { object, string } from 'yup';
import axios from 'axios';
import toastr from 'toastr';
import { resolveClientUrl, axiosError, toastOptions } from '../../shared/common.functions';

export const validationSchema = object({
    employee: string()
        .required('Employee field is required!'),
    companyId: string()
        .required('Company field is required!'),
    teamId: string()
        .required('Team field is required!'),
    fullName: string()
        .required('Full Name field is required!'),
    code: string()
        .required('Employee ID field is required!')
});

export const submitTransfer = (endpoint, model, setSubmitting) => {
    setSubmitting(true);

    axios
        .post(
            resolveClientUrl(endpoint),
            model,
            {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            }
        )
        .then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                const successMessage = endpoint.includes('Create') ? "Transfer successfully!" : "Updated successfully!";
                toastr.success(successMessage, '', toastOptions());
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        })
        .catch(axiosError);
};