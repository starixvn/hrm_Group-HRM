import React from 'react';
import { Modal } from 'react-bootstrap';
import { Formik, Form, Field } from 'formik';
import Draggable from 'react-draggable';

import Errors from '../../../../shared/components/Errors';

import { validationSchema } from '../common.functions';

const CreateEdit = ({ show, data, title, onHide, onSubmit }) => {
    const teamName = data ? data.teamName : '';
    const teamId = data ? data.id : '';

    const renderForm = ({ errors }) => {
        return (
            <Form className="form-horizontal">
                <Modal.Header closeButton>
                    <Modal.Title id="contained-modal-title-vcenter">
                        {title} external group
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Errors errors={errors} />
                    <div className="form-group" style={{ marginBottom: '0' }}>
                        <div className="label-form">Group Name</div>
                        <div className="input-form">
                            <Field name="TeamName" className="form-control">
                                {({
                                    field
                                }) => (
                                    <input
                                        autoComplete='off'
                                        className="form-control"
                                        type="text"
                                        {...field}
                                    />
                                )}
                            </Field>
                        </div>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <button type="submit" className="btn btn-primary">Save</button>
                    <a className="btn btn-default" onClick={onHide}>Cancel</a>
                </Modal.Footer>
            </Form>
        );
    };

    return (
        <Draggable handle=".modal-header" >
            <Modal
                show={show}
                keyboard={false}
                backdrop="static"
                onHide={onHide}
            >
                <Formik
                    validateOnBlur={false}
                    validateOnChange={false}
                    enableReinitialize={true}
                    initialValues={{
                        TeamName: teamName,
                        TeamId: teamId
                    }}
                    validationSchema={validationSchema }
                    onSubmit={onSubmit}
                >
                    {renderForm}
                </Formik>
            </Modal>
        </Draggable>
    );
}

export default CreateEdit;