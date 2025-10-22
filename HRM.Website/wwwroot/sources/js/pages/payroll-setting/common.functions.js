import * as Yup from 'yup';

export const validationSchema = Yup.object({
  name: Yup.string()
    .required('Salary Structure field is required!')
    .trim(),

  jobLevelId: Yup.number()
    .nullable()
    .typeError('Job Level field is required !')
    .required('Job Level field is required !'),

  localSalary: Yup.number()
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
    const { localSalary, expatSalary } = values;

    const isLocalFilled = typeof localSalary === 'number' && !isNaN(localSalary);
    const isExpatFilled = typeof expatSalary === 'number' && !isNaN(expatSalary);

    return isLocalFilled || isExpatFilled;
  }
);
