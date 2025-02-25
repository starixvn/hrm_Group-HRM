import * as Yup from 'yup';

export const validationSchema = Yup.object({
    name: Yup.string().trim().required('Job Title is required!')
});