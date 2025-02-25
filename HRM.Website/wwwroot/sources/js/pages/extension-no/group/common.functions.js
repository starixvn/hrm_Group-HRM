import * as Yup from 'yup';

export const validationSchema = Yup.object({
    TeamName: Yup.string()
        .trim()
        .min(3, 'Length of external group must be 3 between and 100.')
        .max(100, 'Length of external group must be 3 between and 100.')
        .required('Name field is required!'),
});