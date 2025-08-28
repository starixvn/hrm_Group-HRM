import {object, string} from 'yup';
import axios from 'axios';
import toastr from 'toastr';
import { resolveClientUrl, axiosError, toastOptions } from '../../shared/common.functions';

export const validationSchema = object({
    employee: string()
        .required('Employee field is required!'),
    companyId: string()
        .required('Company field is required!')
});

export const submitSecondmentCompany = (endpoint, model, setSubmitting) => {
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
                const successMessage = endpoint.includes('Create') ? "Created successfully!" : "Updated successfully!";
                toastr.success(successMessage, '', toastOptions());
                setTimeout(function () {
                    document.location =
                        resolveClientUrl("secondmentcompany/index");
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        })
        .catch(axiosError);
};