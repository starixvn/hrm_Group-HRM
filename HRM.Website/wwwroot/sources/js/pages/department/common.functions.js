import { number, string, object } from 'yup'
import { useEffect, useState } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError } from '../../shared/common.functions';

export const useDepartmentUnits = () => {
    const [units, setUnits] = useState([]);

    useEffect(() => {
        axios({
            url: resolveClientUrl(`Department/GetUnits`),
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" },
        })
            .then(response => {
                setUnits(response.data);
            })
            .catch(axiosError);
    }, []);

    return units;
};

export const validationSchema = object({
    groupDeptId: number()
        .required('Department ID field is required!'),
    unit: string()
        .required('Business Unit field is required!'),
    code: string()
        .trim()
        .max(50, 'Department Code field must not over 50 characters long!')
        .required('Department Code field is required!'),
    name: string()
        .trim()
        .max(200, 'Department Name field must not over 200 characters long!')
        .required('Department Name field is required!'),
    accountableArea: string()
        .max(100, 'Accountable Area field must not over 100 characters long!')
});