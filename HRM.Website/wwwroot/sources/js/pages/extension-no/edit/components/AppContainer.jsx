import 'jquery-ui-bundle/jquery-ui';
import React, { useEffect, useState } from 'react';
import * as Yup from 'yup';
import axios from 'axios';
import toastr from 'toastr';
import { Formik } from 'formik';

import RenderForm from '../../RenderForm';

import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [extensionDetail, setExtensionDetail] = useState(null);
    const [selectExtension, setSelectExtension] = useState(null);
    const [isInternal, setIsInternal] = useState(window.isInternal === 'True');
    const [employees, setEmployees] = useState([]);
    const [selectEmployee, setSelectEmployee] = useState(null);
    const [selectGroup, setSelectGroup] = useState('');

    const editExtensionSchema = Yup.object({
        ExtensionNo: Yup.string()
            .trim()
            .required('Extension No field is required!'),
        Employee: isInternal && extensionDetail && extensionDetail.empId === 0 ? Yup.string()
            .trim()
            .required('Employee field is required!') : false,
    });

    const getExtentionById = (isInternal, id) => {
        axios({
            url: resolveClientUrl('ExtensionNo/GetExtentionById'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: {
                id: id,
                isInternal: isInternal
            }
        }).then(response => {
            setExtensionDetail(response.data);
        }).catch(axiosError);
    };

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
                        value: `${item.nickName} (${item.fullName})`,
                        fullName: item.fullName,
                        nickName: item.nickName
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
        }).catch(axiosError);
    };

    const handleSelectEmployee = selectedItem => {
        setSelectEmployee(selectedItem);
    };

    useEffect(() => {
        getExtentionById(window.isInternal, window.id);
    }, [window.isInternal, window.id]);

    useEffect(() => {
        getGroupExtension(isInternal);
    }, []);

    const handleChangeInternal = event => {
        setIsInternal(event.target.checked);
        axios({
            url: resolveClientUrl('ExtensionNo/GetGroupExtension'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            params: { isInternal: event.target.checked }
        }).then(response => {
            setSelectExtension(response.data);
            setExtensionDetail({
                empId: extensionDetail.empId,
                teamId: response.data[0].value,
                extensionNo: extensionDetail.extensionNo,
                nickName: extensionDetail.nickName,
                fullName: extensionDetail.fullName,
                employeeExtensionId: extensionDetail.employeeExtensionId,
                employee: extensionDetail.employee
            });
            setSelectGroup(response.data[0].value);
        }).catch(axiosError);
    };

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        axios({
            method: 'POST',
            url: resolveClientUrl('ExtensionNo/Edit'),
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            data: {
                fullName: values.FullName,
                nickName: values.NickName,
                extensionNo: values.ExtensionNo,
                isInternal: values.IsInternal,
                empId: values.EmpId,
                employeeExtensionId: extensionDetail.employeeExtensionId,
                teamId: values.TeamId
            }
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Updated successfully!', '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl('ExtensionNo/Index');
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        }).catch(axiosError);
    };

    if (extensionDetail == null) {
        return null;
    };
    const empId = selectEmployee && selectEmployee.id || extensionDetail.empId;
    const emPloyee = selectEmployee && selectEmployee.value || '';
    const fullName = selectEmployee && selectEmployee.fullName || extensionDetail.fullName || '';
    const teamId = selectGroup || extensionDetail.teamId;
    const nickName = selectEmployee && selectEmployee.nickName || extensionDetail.nickName || '';
    const extensionNo = extensionDetail.extensionNo || '';

    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={{
                EmpId: empId,
                Employee: emPloyee,
                FullName: fullName,
                TeamId: teamId,
                NickName: nickName,
                ExtensionNo: extensionNo,
                IsInternal: isInternal
            }}
            enableReinitialize={true}
            validationSchema={editExtensionSchema}
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
                    title='Update'
                />
            )}
        </Formik>
    );
};

export default AppContainer;