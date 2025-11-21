import React from 'react';
import { Formik, Form, Field } from 'formik';

const SalaryStructureForm = ({ 
    jobLevels, 
    initialValues, 
    handleChangeSalary, 
    validateFormValues, 
    handleSubmit, 
    isEdit = false 
}) => {
    const handleGoBack = () => {
        history.back();
    };

    const renderForm = ({ isSubmitting }) => (
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
                            <th>Basic Salary</th>
                            <th>Job Level Add-On</th>
                            <th>Expat Salary</th>
                            <th>Expat Job Level Add-On</th>
                        </tr>
                    </thead>
                    <tbody>
                        {jobLevels.map(jobLevel => (
                            <tr key={jobLevel.id}>
                                <td>{jobLevel.name}</td>
                                <td>
                                    <input 
                                        type='number' 
                                        min='0' 
                                        step='0.01'
                                        className='form-control' 
                                        autoComplete='off'
                                        value={jobLevel.basicSalary}
                                        onChange={e => handleChangeSalary(jobLevel.id, 'basicSalary', e.target.value)}
                                    />
                                </td>
                                <td>
                                    <input 
                                        type='number' 
                                        min='0' 
                                        step='0.01'
                                        className='form-control' 
                                        autoComplete='off'
                                        value={jobLevel.jobLevelAddOn}
                                        onChange={e => handleChangeSalary(jobLevel.id, 'jobLevelAddOn', e.target.value)}
                                    />
                                </td>
                                <td>
                                    <input 
                                        type='number' 
                                        min='0' 
                                        step='0.01'
                                        className='form-control' 
                                        autoComplete='off'
                                        value={jobLevel.expatSalary}
                                        onChange={e => handleChangeSalary(jobLevel.id, 'expatSalary', e.target.value)}
                                    />
                                </td>
                                <td>
                                    <input 
                                        type='number' 
                                        min='0' 
                                        step='0.01'
                                        className='form-control' 
                                        autoComplete='off'
                                        value={jobLevel.expatJobLevelAddOn}
                                        onChange={e => handleChangeSalary(jobLevel.id, 'expatJobLevelAddOn', e.target.value)}
                                    />
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                
                <div className='form-group'>
                    <button 
                        type='submit' 
                        className='btn btn-primary' 
                        id='btnSubmit' 
                        disabled={isSubmitting}
                    >
                        {isEdit ? 'Update' : 'Add'}
                    </button>
                    <a className='btn btn-default' onClick={handleGoBack}>
                        Back
                    </a>
                </div>
            </Form>
        </div>
    );

    return (
        <Formik 
            enableReinitialize={isEdit}
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

export default SalaryStructureForm;
