import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { Formik, Form, Field } from 'formik';
import Draggable from 'react-draggable';

import Errors from '../../../shared/components/Errors';

import { validationSchema } from '../common.functions';

const CreateEdit = ({ show, onHide, onSubmit, data, title }) => {
    const renderForm = errors => {
        return (
            <div>
                <Errors errors={errors} />
                <div className="form-horizontal clearfix">
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Job Level</label>
                        </div>
                        <div className="input-form">
                            <Field name="name" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                </div>
            </div>
        );
    };

    return (
        <Draggable handle=".modal-header" >
            <Modal keyboard={false} backdrop="static" show={show} onHide={onHide}>
                <Formik
                    validateOnBlur={false}
                    validateOnChange={false}
                    initialValues={{ name: data ? data.name : '' }}
                    validationSchema={validationSchema}
                    onSubmit={onSubmit}
                >
                    {({ errors }) => (
                        <Form>
                            <Modal.Header>
                                <Modal.Title>{title} Job Level</Modal.Title>
                            </Modal.Header>
                            <Modal.Body>
                                {renderForm(errors)}
                            </Modal.Body>
                            <Modal.Footer>
                                <Button bsStyle="primary" type="submit">Submit</Button>
                                <Button className="btn-cancel" onClick={onHide}>Cancel</Button>
                            </Modal.Footer>
                        </Form>
                    )}
                </Formik>
            </Modal>
        </Draggable>
    );
};

export default CreateEdit;