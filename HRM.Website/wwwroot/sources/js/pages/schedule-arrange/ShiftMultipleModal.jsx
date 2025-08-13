import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Button, Form, Modal } from 'react-bootstrap';

import ShiftAutocomplete from './ShiftAutocomplete';

export default class ShiftMultipleModal extends Component {
    static propTypes = {
        show: PropTypes.bool,
        onHide: PropTypes.func,
        onSubmit: PropTypes.func
    };

    static defaultProps = {
        show: false,
        onHide: function () {
            // This is intentional
        },
        onSubmit: function () {
            // This is intentional
        }
    };

    constructor(props) {
        super(props);

        this.state = {
            shift: null,
            isOffDay: false,
            isValid: true
        };
    }

    handleSubmit = event => {
        event.preventDefault();

        const { shift, isOffDay } = this.state;

        if (shift === null) {
            this.setState({ isValid: false });
        } else {
            this.props.onSubmit(shift, isOffDay);
        }
    };

    handleChangeShift = (_, shift) => {
        const isValid = shift !== null;

        this.setState({ shift, isValid });
    };

    handleChangeOffDay = event => {
        this.setState({ isOffDay: event.target.checked });
    };

    render() {
        const { shift, isOffDay, isValid } = this.state;

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.show} onHide={this.props.onHide}
                dialogClassName="modal-shift-multiple modal-dialog-centered"
            >
                <Modal.Header closeButton>
                    <Modal.Title>Shift Multiple</Modal.Title>
                </Modal.Header>
                <Form id="shift-multiple-form" horizontal onSubmit={this.handleSubmit}>
                    <Modal.Body>
                        <div className="form-group">
                            <div className="label-form">
                                <label className="control-label">Shift</label>
                            </div>
                            <div className="input-form">
                                <ShiftAutocomplete className={`form-control ${isValid ? '' : 'input-validation-error'}`}
                                    shift={shift} shiftOptions={this.props.shiftOptions}
                                    onChange={this.handleChangeShift}
                                />
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="checkbox">
                                <input type="checkbox" className={`checkbox-custom`} id={`ckb-isOffDay`}
                                    checked={isOffDay} onChange={this.handleChangeOffDay}
                                />
                                <label htmlFor={`ckb-isOffDay`}> Is Off Day</label>
                            </div>
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button type="submit" bsStyle="primary">Ok</Button>
                        <Button onClick={this.props.onHide} className="btn-cancel">Cancel</Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}