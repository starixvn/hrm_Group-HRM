import React, { useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { Formik, Form, Field } from 'formik';

import {
    resolveClientUrl,
    axiosError,
    toastOptions,
} from '../../../shared/common.functions';

const AppContainer = () => {
    const [jobLevels, setJobLevels] = useState([]);

    const [initialValues, setInitialValues] = useState({
        salaryStructureId: '',
        name: ''
    });

    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios({
            url: resolveClientUrl(`salarystructure/getById/${window.salaryStructureId}`),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
        })
            .then(response => {
                const data = response.data;
                setInitialValues(prev =>({
                    ...prev,
                    salaryStructureId: data.salaryStructureId,
                    name: data.name
                }));
                setJobLevels(data.details.map(item => {
                    return {
                        id: item.jobLevelId,
                        name: item.name,
                        localSalary: item.localSalary,
                        expatSalary: item.expatSalary
                    };
                }));
            })
            .catch(axiosError);
    }, []);

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios
            .post(
                resolveClientUrl('SalaryStructure/Edit'),
                {
                    salaryStructureId: values.salaryStructureId,
                    name: values.name.trim(),
                    details: jobLevels.filter(x => x.localSalary || x.expatSalary).map(x => {
                        return {
                            jobLevelId: x.id,
                            localSalary: x.localSalary,
                            expatSalary: x.expatSalary
                        }
                    })
                },
                {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                }
            )
            .then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success('Updated successfully!', '', toastOptions());
                    setTimeout(function () {
                        document.location =
                            resolveClientUrl('SalaryStructure/Index');
                    }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
                    setSubmitting(false);
                }
            })
            .catch(axiosError);
    };

    const handleChangeSalary = (jobLevelId, fieldName, value) => {
        const tmpJobLevels = [...jobLevels];
        const index = tmpJobLevels.findIndex(x => x.id === jobLevelId);
        tmpJobLevels[index] = {
            ...tmpJobLevels[index],
            [fieldName]: value
        }
        setJobLevels(tmpJobLevels);
    };

    const renderForm = ({ isSubmitting }) => {
        return (
            <div className='card'>
                <Form className='form-horizontal'>
                    <div className='form-group'>
                        <div className='label-form'>
                            <label className='control-label'>
                                Salary Structure
                                <span className='require'>*</span>
                            </label>
                        </div>
                        <div className='input-form'>
                            <Field
                                name='name'
                                className='form-control'
                                autoComplete='off'
                            />
                        </div>
                    </div>
                    <table className='table table-custom'>
                        <thead>
                            <tr>
                                <th>Job Level</th>
                                <th>Local Salary</th>
                                <th>Expat Salary</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                jobLevels.map(jobLevel => (
                                    <tr key={jobLevel.id}>
                                        <td>{jobLevel.name}</td>
                                        <td>
                                            <input type='number' min='0' step='0.01'
                                                className='form-control' autoComplete='off'
                                                value={jobLevel.localSalary}
                                                onChange={e => handleChangeSalary(jobLevel.id, 'localSalary', e.target.value)}
                                            />
                                        </td>
                                        <td>
                                            <input type='number' min='0' step='0.01'
                                                className='form-control' autoComplete='off'
                                                value={jobLevel.expatSalary}
                                                onChange={e => handleChangeSalary(jobLevel.id, 'expatSalary', e.target.value)}
                                            />
                                        </td>
                                    </tr>
                                ))
                            }
                        </tbody>
                    </table>
                    <div className='form-group'>
                        <button type='submit' className='btn btn-primary' id='btnSubmit' disabled={isSubmitting}>Update</button>
                        <a className='btn btn-default' onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        );
    };

    const validateFormValues = values => {
        if (!values.name.trim()) {
            toastr.error('Salary Structure field is required!', '', toastOptions());
            return {
                name: ''
            };
        }

        return {};
    };

    return (
        <Formik enableReinitialize
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={initialValues}
            validate={validateFormValues}
            onSubmit={handleSubmit}
        >
            {renderForm}
        </Formik>
    );
};

export default AppContainer;
