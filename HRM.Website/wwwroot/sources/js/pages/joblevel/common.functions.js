import * as Yup from 'yup';

export const validationSchema = Yup.object({
    name: Yup.string().trim().required('Job Level is required!')
});