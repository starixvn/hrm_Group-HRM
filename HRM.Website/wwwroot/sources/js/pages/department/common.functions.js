import * as Yup from 'yup';

export const validationSchema = Yup.object({
    code: Yup.string()
        .trim()
        .max(50, 'Code field must not over 50 characters long!')
        .required('Code field is required!'),
    unit: Yup.string()
        .required('Unit field is required!'),
    groupDeptId: Yup.number()
        .required('ID field is required!'),
    accountableArea: Yup.string()
        .max(200, 'Accountable Area field must not over 200 characters long!')
});