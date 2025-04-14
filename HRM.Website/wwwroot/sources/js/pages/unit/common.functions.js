import * as Yup from 'yup';
export const validationSchema = Yup.object({
    code: Yup.string()
        .trim()
        .max(50, 'Business Unit Code field must not over 50 characters long!')
        .required('Business Unit Code field is required!'),
    groupUnitId: Yup.number()
        .required('Business Unit ID field is required!'),
    category: Yup.string()
        .max(200, 'Business Unit Category field must not over 200 characters long!'),
    type: Yup.string()
        .max(200, 'Business Unit Type field must not over 200 characters long!'),
    accountableArea: Yup.string()
        .max(200, 'Accountable Area field must not over 200 characters long!'),
});