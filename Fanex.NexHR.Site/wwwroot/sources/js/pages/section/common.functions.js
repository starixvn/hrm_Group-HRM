import * as Yup from 'yup';

export const validationSchema = Yup.object({
    code: Yup.string()
        .trim()
        .max(50, 'Code field must not over 50 characters long!')
        .required('Code field is required!'),
    name: Yup.string()
        .trim()
        .max(200, 'Name must not over 200 characters long!')
        .required('Name field is required!'),
    department: Yup.string().required('Deparment field is required!')
});