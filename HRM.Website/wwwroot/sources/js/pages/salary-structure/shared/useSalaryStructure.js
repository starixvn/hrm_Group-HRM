import { useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';

const useSalaryStructure = (isEdit = false) => {
    const [jobLevels, setJobLevels] = useState([]);
    const [initialValues, setInitialValues] = useState({
        salaryStructureId: '',
        name: ''
    });

    const loadJobLevels = async () => {
        try {
            const url = isEdit 
                ? `salarystructure/getById/${window.salaryStructureId}`
                : 'SalaryStructure/GetJobLevels';
            
            const response = await axios({
                url: resolveClientUrl(url),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
            });

            if (isEdit) {
                const data = response.data;
                setInitialValues(prev => ({
                    ...prev,
                    salaryStructureId: data.salaryStructureId,
                    name: data.name
                }));
                setJobLevels(data.details.map(item => ({
                    id: item.jobLevelId,
                    name: item.name,
                    basicSalary: item.basicSalary,
                    expatSalary: item.expatSalary,
                    jobLevelAddOn: item.jobLevelAddOn || ''
                })));
            } else {
                setJobLevels(response.data.map(item => ({
                    ...item,
                    basicSalary: '',
                    expatSalary: '',
                    jobLevelAddOn: ''
                })));
            }
        } catch (error) {
            axiosError(error);
        }
    };

    const handleChangeSalary = (jobLevelId, fieldName, value) => {
        setJobLevels(prev => prev.map(jobLevel => 
            jobLevel.id === jobLevelId 
                ? { ...jobLevel, [fieldName]: value }
                : jobLevel
        ));
    };

    const validateFormValues = values => {
        if (!values.name.trim()) {
            toastr.error('Salary Structure field is required!', '', toastOptions());
            return { name: '' };
        }
        return {};
    };

    const getSubmitData = (values) => ({
        ...(isEdit && { salaryStructureId: values.salaryStructureId }),
        name: values.name.trim(),
        details: jobLevels
            .filter(x => x.basicSalary || x.expatSalary || x.jobLevelAddOn)
            .map(x => ({
                jobLevelId: x.id,
                basicSalary: x.basicSalary,
                expatSalary: x.expatSalary,
                jobLevelAddOn: x.jobLevelAddOn
            }))
    });

    const handleSubmit = async (values, { setSubmitting }) => {
        setSubmitting(true);

        try {
            const endpoint = isEdit ? 'SalaryStructure/Edit' : 'SalaryStructure/Create';
            const response = await axios.post(
                resolveClientUrl(endpoint),
                getSubmitData(values),
                { headers: { 'X-Requested-With': 'XMLHttpRequest' } }
            );

            const { data } = response;
            if (data.isSuccessful) {
                const action = isEdit ? 'Updated' : 'Created';
                toastr.success(`${action} successfully!`, '', toastOptions());
                setTimeout(() => {
                    document.location = resolveClientUrl('SalaryStructure/Index');
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        } catch (error) {
            axiosError(error);
            setSubmitting(false);
        }
    };

    useEffect(() => {
        loadJobLevels();
    }, []);

    return {
        jobLevels,
        initialValues,
        handleChangeSalary,
        validateFormValues,
        handleSubmit
    };
};

export default useSalaryStructure;
