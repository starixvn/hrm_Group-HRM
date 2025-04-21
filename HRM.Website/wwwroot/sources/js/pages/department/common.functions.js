import * as Yup from 'yup';

export const validationSchema = Yup.object({
    groupDeptId: Yup.number()
        .required('Department ID field is required!'),
    unit: Yup.string()
        .required('Business Unit field is required!'),
    code: Yup.string()
        .trim()
        .max(50, 'Department Code field must not over 50 characters long!')
        .required('Department Code field is required!'),
    name: Yup.string()
        .trim()
        .max(200, 'Department Name field must not over 200 characters long!')
        .required('Department Name field is required!'),
    accountableArea: Yup.string()
        .max(100, 'Accountable Area field must not over 100 characters long!')
});