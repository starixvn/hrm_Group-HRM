import * as Yup from 'yup';

export const validationSchema = Yup.object({
  name: Yup.string()
    .required('Salary Structure field is required!')
    .trim(),

  jobLevelId: Yup.number()
    .nullable()
    .typeError('Job Level field is required !')
    .required('Job Level field is required !'),

  basicSalary: Yup.number()
    .nullable()
    .transform((value, originalValue) => (originalValue === '' ? null : value)),

  expatSalary: Yup.number()
    .nullable()
    .transform((value, originalValue) => (originalValue === '' ? null : value)),
}).test(
  'at-least-one-salary',
  'Local Salary or Expat Salary is required !',
  function (values) {
    if (!values) return false;
    const { basicSalary, expatSalary } = values;

    const isBasicFilled = typeof basicSalary === 'number' && !isNaN(basicSalary);
    const isExpatFilled = typeof expatSalary === 'number' && !isNaN(expatSalary);

    return isBasicFilled || isExpatFilled;
  }
);
