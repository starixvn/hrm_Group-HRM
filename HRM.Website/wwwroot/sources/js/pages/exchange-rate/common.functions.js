import * as Yup from 'yup';
import moment from 'moment';

export const validationSchema = Yup.object({
    currencyCode: Yup.string()
        .trim()
        .required('Currency is required!')
        .length(3, 'Currency must be exactly 3 characters'),
    exchangeRate: Yup.number()
        .required('Exchange Rate is required!')
        .positive('Exchange Rate must be greater than 0'),
    effectiveDate: Yup.string()
        .required('Effective Date is required!')
        .test('is-valid-date', 'Effective Date must be a valid date', function(value) {
            if (!value) return false;
            return moment(value, 'YYYY-MM-DD', true).isValid();
        })
});
