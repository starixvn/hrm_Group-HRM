import * as Yup from 'yup';

export const validationSchema = Yup.object({
    employee: Yup.string()
        .required('Employee field is required!'),
    companyId: Yup.string()
        .required('Company field is required!')
});