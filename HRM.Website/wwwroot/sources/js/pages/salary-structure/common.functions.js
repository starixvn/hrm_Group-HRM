import * as Yup from 'yup';

export const validationSchema = Yup.object({
    name: Yup.string()
        .required('Salary Structure field is required!')
        .trim(),
    jobLevelId: Yup.number()
        .required('Job Level field is required!')
});