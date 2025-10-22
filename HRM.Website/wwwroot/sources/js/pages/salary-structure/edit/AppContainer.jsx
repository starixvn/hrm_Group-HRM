import React from 'react';
import useSalaryStructure from '../shared/useSalaryStructure';
import SalaryStructureForm from '../shared/SalaryStructureForm';

const AppContainer = () => {
    const {
        jobLevels,
        initialValues,
        handleChangeSalary,
        validateFormValues,
        handleSubmit
    } = useSalaryStructure(true);

    return (
        <SalaryStructureForm
            jobLevels={jobLevels}
            initialValues={initialValues}
            handleChangeSalary={handleChangeSalary}
            validateFormValues={validateFormValues}
            handleSubmit={handleSubmit}
            isEdit={true}
        />
    );
};

export default AppContainer;
