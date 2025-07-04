import * as Yup from 'yup';

export const validationSchema = Yup.object({
    salaryStructure: Yup.string()
        .required('Salary Structure is required')
        .trim(),
    jobLevel: Yup.string()
        .required('Job Level is required'),
    localSalary: Yup.number()
        .typeError('Local Salary must be a number')
        .min(0, 'Local Salary must be greater than or equal to 0')
        .nullable(),
    expatSalary: Yup.number()
        .typeError('Expat Salary must be a number')
        .min(0, 'Expat Salary must be greater than or equal to 0')
        .nullable(),
});