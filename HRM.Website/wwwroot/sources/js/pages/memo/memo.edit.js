import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-validation/dist/jquery.validate';
import axios from 'axios';
import tinymce from 'tinymce/tinymce';
import 'tinymce/themes/silver/index';
import 'tinymce/icons/default/index';

import '../memo/tinymce.plugins';
import '../../date';

import { initDatepicker, showValidationSummaryErrors, hideValidationSummaryErrors, resolveClientUrl, axiosError } from '../../shared/common.functions';

$(function () {
    var $memoType, $contentBlock, $fileList, $fileAttachContent, $memoContent, $fileUploader, $form, $memoTypeValue, $attachedFileBlock;

    function initAttachFiles() {
        $fileList.html('');

        var attachPath = $fileAttachContent.val();
        if (attachPath) {
            var attachPathFiles = attachPath.split(';');

            for (var i = 0; i < attachPathFiles.length; i++) {
                var attachLink = attachPathFiles[i],
                    attachLinkSplit = attachLink.split('/'),
                    attachFileName = attachLinkSplit[attachLinkSplit.length - 1],
                    pattern = '<div id="file_' + i + '"><a href="' + window.UploadBasePath + attachLink + '">' + attachFileName + '</a>&nbsp\
                                <a href="javascript:;">\
                                    <i class="fa fa-times fa-lg icon-delete" data-id="'+ attachLink + '"></i>\
                                </a></div>';

                $fileList.append(pattern);
            }
        }
    }

    function initTextEditor(element) {
        tinymce.init({
            mode: 'exact',
            height: 400,
            menubar: false,
            selector: 'textarea#' + element,
            plugins: 'spellchecker,pagebreak,table,save,emoticons,insertdatetime,preview,image,link,media,imagetools,searchreplace,contextmenu,paste,directionality,wordcount,fullscreen,noneditable,visualchars,nonbreaking,template,hr,charmap,autolink',
            toolbar1: 'undo redo | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | fontselect | fontsizeselect ',
            toolbar2: '  forecolor backcolor | bullist numlist | outdent indent | link image | table | hr | superscript subscript | charmap emoticons | ltr rtl |',
            content_css: false,
            fullpage_font_size: '13px',
            skin: false,
            force_br_newlines: true,
            force_p_newlines: false,
            branding: false,
        });
    }

    function initDatePicker() {
        initDatepicker();
    }

    function changeMemoType(value) {
        if (value === '1') {
            $contentBlock.hide();
            $fileUploader.show();
            $attachedFileBlock.show();
        } else {
            $contentBlock.show();
            $fileUploader.hide();
            $attachedFileBlock.hide();
        }
    }

    function initMemoType() {
        $memoTypeValue = $memoType.val();
        changeMemoType($memoTypeValue);
        $memoType.change(function () {
            let value = $(this).val();
            $memoTypeValue = value;
            changeMemoType(value);
        });
    }

    function validateForm() {
        let Subject = $('#Subject').val();
        let RefNo = $('#RefNo').val();
        let SentTo = $('#SentTo').val();
        let SentFrom = $('#SentFrom').val();
        let attachFile = $fileList.children().length;
        let fileName = $fileUploader.val();

        let isValid = true;
        let errorString = [];

        var editorText = tinyMCE.activeEditor.getContent();

        if (Subject === null || Subject === '') {
            errorString.push('The Subject field is required.');
        }
        if (RefNo === null || RefNo === '') {
            errorString.push('The RefNo field is required.');
        }
        if (SentTo === null || SentTo === '') {
            errorString.push('The SentTo field is required.');
        }
        if (SentFrom === null || SentFrom === '') {
            errorString.push('The SentFrom field is required.');
        }

        if ($memoTypeValue === '1') {
            if (attachFile === 0 && fileName === '') {
                errorString.push('The Attached file is required.');
            }
        } else {
            if (editorText === null || editorText === '') {
                errorString.push('The Content field is required.');
            }
        }

        if (errorString.length > 0) {
            $('.validation-summary-valid').find('ul:first').html('');
            showValidationSummaryErrors(errorString);
            isValid = false;
        }

        return isValid;
    }

    function initValues() {
        $attachedFileBlock = $('#allAttachFile');
        $fileList = $('#allAttachFile');
        $fileAttachContent = $('#AttachFile');
        $memoType = $('input[name=MemoType]');
        $contentBlock = $('#content');
        $memoContent = $('#Contents');
        $fileUploader = $('#fileName');
    }

    function uploadFile() {
        let files = document.getElementById('fileName').files;
        if (files === undefined) {
            return;
        }

        let fd = new FormData();
        fd.append('categoryID', $('#Category').val());
        fd.append('isOverride', $('#override')[0].checked);

        for (let i = 0; i < files.length; i++) {
            let file = files[i];

            if (file.size / 1024 > 5120) {
                alert('file is to large (' + file.size / 1024 + 'kb) to upload. maximum = 5120kb');
                return;
            }

            fd.append('files', file);
        }

        axios({
            url: resolveClientUrl('file/UploadFile'),
            method: 'POST',
            data: fd,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
            }
        }).then(response => {
            if (response.status === 200) {
                var attachFile = $fileAttachContent.val();

                if (attachFile.indexOf(resolveClientUrl(response.data)) === -1) {
                    if (attachFile) {
                        $fileAttachContent.val(attachFile + ';' + resolveClientUrl(response.data));
                    }
                    else {
                        $fileAttachContent.val(resolveClientUrl(response.data));
                    }
                }
                axios({
                    url: $form[0].action,
                    method: 'POST',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    data: $form.serialize(),
                    async: false,
                }).then(response => {
                    document.location = resolveClientUrl('Memo/Index');
                }).catch(axiosError);
            }
        }).catch(axiosError);
    };

    function initFormSubmitEvent() {
        $('#btnSubmit').click(function (e) {
            let isValid = validateForm();
            $form = $('#editForm');

            if (isValid) {
                hideValidationSummaryErrors();
                e.preventDefault();
                $memoType.val($memoTypeValue);
                if ($memoTypeValue === '1') {
                    var attachFile = $fileList.children().length;

                    if (attachFile === 0 || $fileUploader.val() !== '') {
                        uploadFile();
                    } else {
                        axios({
                            url: $form[0].action,
                            method: 'POST',
                            data: $form.serialize(),
                            headers: {
                                'X-Requested-With': 'XMLHttpRequest',
                                'Content-Type': 'application/x-www-form-urlencoded'
                            }
                        }).then(response => {
                            document.location = resolveClientUrl('Memo/Index');
                        }).catch(axiosError);
                    }
                }
                else {
                    $memoContent.val(tinymce.activeEditor.getContent());
                    axios({
                        url: $form[0].action,
                        method: 'POST',
                        data: $form.serialize(),
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest',
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }
                    }).then(response => {
                        document.location = resolveClientUrl('Memo/Index');
                    }).catch(axiosError);
                }
            }
        });
    }

    function initDeleteAttachFileEvent() {
        $('.icon-delete').on('click', function () {
            var fileName = $(this).attr('data-id');

            axios({
                url: resolveClientUrl('file/Delete'),
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                params: {
                    FileName: fileName
                }
            }).then(response => {
                if (response.status === 200) {
                    var attachFiles = $fileAttachContent.val().split(';'), attachFileRemain = '';

                    for (var i = 0; i < attachFiles.length; i++) {
                        var attachFileItem = attachFiles[i];

                        if (attachFileItem !== fileName) {
                            if (attachFileRemain === '') {
                                attachFileRemain = attachFileItem;
                            }
                            else {
                                attachFileRemain += ';' + attachFileItem;
                            }
                        }
                    }

                    $fileAttachContent.val(attachFileRemain);
                    initAttachFiles();
                    initDeleteAttachFileEvent();
                }
            }).catch(axiosError => {
                $errorBlock.html('Fail to remove attached file!');
            });
        });
    }

    $(document).ready(function () {
        initValues();
        initDatePicker();
        initAttachFiles();
        initMemoType();
        initFormSubmitEvent();
        initDeleteAttachFileEvent();
        initTextEditor('Contents');
    });
});