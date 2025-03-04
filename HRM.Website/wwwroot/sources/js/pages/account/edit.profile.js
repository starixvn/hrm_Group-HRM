import 'bootstrap/dist/js/bootstrap';
import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import toastr from 'toastr';
import { axiosError, confirm, alert, toastOptions, initDatepicker, resolveClientUrl } from '../../shared/common.functions';

var editAccount = (function ($) {
    const domElement = {
        EditInformationForm: '#info-update',
        Attachments: '#Attachments',
        DeleteFile: '.delete-file-upload',
        ListFileUpload: '#upload-list',
        Relationship: '#relationship',
        BtnAddRelationship: '#btn-add-relationship',
        CreateForm: '#create-form'
    };

    const constants = {
        FileMaxSize: 5242880,
        CreateRelationshipUrl: resolveClientUrl('Information/InsertRelationship'),
        EditRelationshipUrl: resolveClientUrl('Information/EditRelationship/'),
        DeleteRelationship: resolveClientUrl('Information/DeleteRelationship/')
    };

    function updateInformationEvent() {
        $(domElement.EditInformationForm).on('submit', function (e) {
            e.preventDefault();
            removeErrorMsg($(domElement.EditInformationForm));
            axios({
                url: $(domElement.EditInformationForm).attr('action'),
                method: $(domElement.EditInformationForm).attr('method'),
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                data: $(domElement.EditInformationForm).serialize()
            }).then(response => {
                if (response.data.isSuccessful) {
                    toastr.success('Updated information sucessfully!', '', toastOptions());
                } else {
                    showErrorMsg($(domElement.EditInformationForm), response.data.message);
                }
            }).catch(axiosError);
        });
    }

    function initUploadFileHandler() {
        $(domElement.Attachments).on('change', function (event) {
            var files = event.originalEvent.target.files;
            if (files.length > 0 && validateFileUpload(files)) {
                uploadFiles(files);
            } else {
                toastr.error('File(s) invalid!', '', toastOptions());
            }
        });
    }

    function uploadFiles(files) {
        if (files === undefined) {
            return;
        }

        let fd = new FormData();
        for (let i = 0; i < files.length; i++) {
            let file = files[i];

            if (file.size / 1024 > 5120) {
                alert('file is to large (' + file.size / 1024 + 'kb) to upload. maximum = 5120kb');
                return;
            }

            fd.append('files', file);
        }

        axios({
            method: 'POST',
            url: resolveClientUrl('File/MyProfileUploadFile'),
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            data: fd
        }).then(response => {
            getFileUpload();
            if (response.data.isSuccessful) {
                toastr.success('File upload successfully!', '', toastOptions());
            } else {
                toastr.error(response.data.message, '', toastOptions());
                $(domElement.Attachments).val('');
            }
        }).catch(axiosError);
    }

    function initDeleteFileUpload() {
        $(domElement.DeleteFile).on('click', function () {
            let $btn = $(this);
            confirm('Are you sure to want to delete this file?', function () {
                const fileName = $btn.attr('data-file-name');
                axios({
                    method: 'POST',
                    url: resolveClientUrl('File/DeleteMyProfileUploadedFile'),
                    headers: { 
                            'X-Requested-With': 'XMLHttpRequest',
                            'Content-Type': 'application/x-www-form-urlencoded' 
                    },
                    data: { fileName }
                }).then(response => {
                    if (response.data.isSuccessful) {
                        toastr.success('Deleted file successfully!', '', toastOptions());
                        getFileUpload();
                    } else {
                        toastr.error(response.data.message, '', toastOptions());
                    }
                }).catch(axiosError);
            });
        });
    }

    function validateFileUpload(files) {
        let isValid = true;
        for (let i = 0; i < files.length; i++) {
            if (files[i].size > constants.FileMaxSize) {
                isValid = false;
                break;
            }
        }

        return isValid;
    }

    function getFileUpload() {
        axios({
            method: 'GET',
            url: resolveClientUrl('Information/GetFileUploaded'),
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'text'
        }).then(response => {
            $(domElement.ListFileUpload).html(response.data);
            initUploadFileHandler();
            initDeleteFileUpload();
        }).catch(axiosError);
    }

    function getRelationship() {
        axios({
            method: 'GET',
            url: resolveClientUrl('Information/GetEmployeeRelationship'),
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            responseType: 'text'
        }).then(response => {
            $(domElement.Relationship).html(response.data);
            initEditEvent($(domElement.Relationship),
                constants.EditRelationshipUrl,
                'Edit Relationship',
                function () { getRelationship(); });
            initDeleteEvent($(domElement.Relationship),
                'Do you want to delete this relationship?',
                constants.DeleteRelationship,
                function () { getRelationship(); });
        }).catch(axiosError);
    }

    function initEditEvent($table, baseUrl, title, onEditSuccess) {
        $table.find('.edit-icon').on('click', function () {
            const id = $(this).data('id');
            if (id > 0) {
                axios({
                    url: baseUrl + id,
                    method: 'GET',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    responseType: 'text'
                }).then(response => {
                    $(domElement.CreateForm).html(response.data);
                    initDatepicker();
                    $(domElement.CreateForm).dialog({
                        title: title,
                        height: 'auto',
                        width: 'auto',
                        modal: true,
                        buttons: [
                            {
                                text: 'Submit',
                                class: 'btn btn-primary',
                                click: function () {
                                    axios({
                                        method: $(domElement.CreateForm).find('form').attr('method'),
                                        url: $(domElement.CreateForm).find('form').attr('action'),
                                        headers: { 'X-Requested-With': 'XMLHttpRequest' },
                                        data: $(domElement.CreateForm).find('form').serialize()
                                    }).then(response => {
                                        if (response.data.isSuccessful) {
                                            $(domElement.CreateForm).dialog('close');
                                            toastr.success('Updated successfully!', '', toastOptions());
                                            if (onEditSuccess) {
                                                onEditSuccess();
                                            }
                                        } else {
                                            showErrorMsg($(domElement.CreateForm), response.data.message);
                                        }
                                    }).catch(axiosError);
                                }
                            },
                            {
                                text: 'Cancel',
                                class: 'btn btn-default',
                                click: function () {
                                    $(this).dialog('close');
                                }
                            }
                        ]
                    });
                }).catch(axiosError);
            }
        });
    }

    function initButtonInsertEvent($btn, onCreateSuccess) {
        $btn.on('click', function () {
            const url = $(this).attr('data-url');
            const title = $(this).attr('title');
            axios({
                url,
                method: 'GET',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                responseType: 'text'
            }).then(response => {
                $(domElement.CreateForm).html(response.data);
                initDatepicker();
                $(domElement.CreateForm).dialog({
                    title: title,
                    height: 'auto',
                    width: 'auto',
                    modal: true,
                    buttons: [
                        {
                            text: 'Submit',
                            class: 'btn btn-primary',
                            click: function () {
                                axios({
                                    method: $(domElement.CreateForm).find('form').attr('method'),
                                    url: $(domElement.CreateForm).find('form').attr('action'),
                                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                                    data: $(domElement.CreateForm).find('form').serialize()
                                }).then(response => {
                                    if (response.data.isSuccessful) {
                                        $(domElement.CreateForm).dialog('close');
                                        if (onCreateSuccess) {
                                            onCreateSuccess();
                                        }
                                        toastr.success('Created successfully!', '', toastOptions());
                                    } else {
                                        showErrorMsg($(domElement.CreateForm), response.data.message);
                                    }
                                }).catch(axiosError);
                            }
                        },
                        {
                            text: 'Cancel',
                            class: 'btn btn-default',
                            click: function () {
                                $(this).dialog('close');
                            }
                        }
                    ]
                });
            }).catch(axiosError);
        });
    }

    function initDeleteEvent($table, title, url, onDeleteSuccess) {
        $table.find('.delete-icon').on('click', function () {
            const id = $(this).data('id');
            confirm(title, function () {
                axios({
                    method: 'POST',
                    url: url + id,
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                }).then(response => {
                    if (response.data.isSuccessful) {
                        onDeleteSuccess();
                        toastr.success('Deleted successfully!', '', toastOptions());
                    } else {
                        toastr.error(response.data.message, '', toastOptions());
                    }
                }).catch(axiosError);
            });
        });
    }

    function showErrorMsg($form, msg) {
        $form.find('.validation-summary-valid').find('ul:first').html('');
        let htmlObj = '<li>' + msg + '</li>';
        $form.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(htmlObj);
    }

    function removeErrorMsg($form) {
        if ($form.find('.validation-summary-valid')) {
            $form.find('.validation-summary-valid').hide();
        }
    }

    function init() {
        initDatepicker();
        getFileUpload();
        updateInformationEvent();
        getRelationship();
        initButtonInsertEvent($(domElement.BtnAddRelationship), function () {
            getRelationship();
        });
    }

    return {
        init: init
    };
})(jQuery);

$(document).ready(function () {
    editAccount.init();
});