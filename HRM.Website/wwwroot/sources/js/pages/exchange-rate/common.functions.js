import * as Yup from 'yup';

export const validationSchema = Yup.object({
    currencyCode: Yup.string()
        .trim()
        .required('Currency is required!')
        .max(3, 'Currency Code must be 3 characters or less'),
    exchangeRate: Yup.number()
        .required('Exchange Rate is required!')
        .positive('Exchange Rate must be greater than 0'),
    effectiveDate: Yup.date()
        .required('Effective Date is required!')
});


