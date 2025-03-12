import 'jquery-ui-bundle/jquery-ui';
import React, { useEffect, useState } from 'react';
import { Formik } from 'formik';
import * as Yup from 'yup';
import axios from 'axios';
import toastr from 'toastr';

import RenderForm from '../../RenderForm';

import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [selectExtension, setSelectExtension] = useState(null);
    const [isInternal, setIsInternal] = useState(false);
    const [employees, setEmployees] = useState([]);
    const [selectEmployee, setSelectEmployee] = useState(null);
    const [selectGroup, setSelectGroup] = useState('');

    const createExtensionSchema = Yup.object({
        ExtensionNo: Yup.string()
            .trim()
            .required('Extension No field is required!'),
        Employee: isInternal ? Yup.string()
            .trim()
            .required('Employee field is required!') : false
    });

    const getEmployees = query => {
        if (query.trim().length >= 2) {
            axios({
                url: resolveClientUrl('employee/getemployees'),
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: { query }
            }).then(response => {
                const { data } = response;
                const employeeOptions = data.map(item => {
                    return {
                        id: item.Id,
                        value: `${item.NickName} (${item.FullName})`,
                        fullName: item.FullName,
                        nickName: item.NickName
                    };
                });

                setEmployees(employeeOptions);
            }).catch(axiosError);
        } else {
            setEmployees([]);
        }
    };

    const getGroupExtension = isInternal => {
        axios({
            url: resolveClientUrl('ExtensionNo/GetGroupExtension'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: { isInternal }
        }).then(response => {
            setSelectExtension(response.data);
            setSelectGroup(response.data[0].value);
        }).catch(axiosError);
    };

    useEffect(() => {
        getGroupExtension(isInternal);
    }, [isInternal]);

    const handleSelectEmployee = selectedItem => {
        setSelectEmployee(selectedItem);
    };

    const handleChangeInternal = event => {
        setIsInternal(event.target.checked);
    };

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const model = {
            fullName: values.FullName,
            nickName: values.NickName,
            extensionNo: values.ExtensionNo,
            isInternal: values.IsInternal,
            empId: isNaN(parseInt(values.EmpId, 10)) ? 0 : values.EmpId,
            teamId: values.TeamId
        };
        axios({
            method: 'POST',
            url: resolveClientUrl('ExtensionNo/Create'),
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            data: model
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Created successfully!', '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl('ExtensionNo/Index');
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        }).catch(axiosError);
    };

    if (!selectExtension) {
        return null;
    }

    const fullName = selectEmployee ? selectEmployee.fullName : '';
    const teamId = selectGroup || selectExtension[0].value;
    const nickName = selectEmployee ? selectEmployee.nickName : '';
    const empId = selectEmployee ? selectEmployee.id : '';
    const empName = selectEmployee ? selectEmployee.value : '';

    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={{
                EmpId: empId,
                Employee: empName,
                FullName: fullName,
                TeamId: teamId,
                NickName: nickName,
                ExtensionNo: '',
                IsInternal: isInternal || false
            }}
            enableReinitialize={true}
            validationSchema={createExtensionSchema}
            onSubmit={handleSubmit}
        >
            {({ errors, isSubmitting }) => (
                <RenderForm
                    errors={errors}
                    isInternal={isInternal}
                    onChangeInternal={handleChangeInternal}
                    selectExtension={selectExtension}
                    employees={employees}
                    onSearchEmployee={getEmployees}
                    onSelectEmployee={handleSelectEmployee}
                    isSubmitting={isSubmitting}
                    setSelectGroup={setSelectGroup}
                    title='Create'
                />
            )}
        </Formik>
    );
};
export default AppContainer;