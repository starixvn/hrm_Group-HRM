import { object, string } from 'yup';
import axios from 'axios';
import toastr from 'toastr';
import { resolveClientUrl, axiosError, toastOptions } from '../../shared/common.functions';

export const validationSchema = object({
    FullName: string()
        .required('Full Name is required!'),
    Code: string()
        .required('Employee ID is required!'),
    NickName: string()
        .required('Nickname is required!')
});

export const submitTransfer = (endpoint, model, setSubmitting, redirectUrl = null) => {
    setSubmitting(true);

    axios
        .post(
            resolveClientUrl(endpoint),
            model,
            {
                headers: { 
                    "X-Requested-With": "XMLHttpRequest",
                    "Content-Type": "application/json"
                },
            }
        )
        .then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                const successMessage = "Transfer successfully!";
                toastr.success(successMessage, '', toastOptions());
                setSubmitting(false);
                if (redirectUrl) {
                    setTimeout(() => {
                        window.location.href = resolveClientUrl(redirectUrl);
                    }, 3000);
                }
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        })
        .catch(axiosError);
};