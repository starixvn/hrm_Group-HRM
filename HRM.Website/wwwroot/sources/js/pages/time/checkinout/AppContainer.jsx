import React, { Component } from 'react';
import { Button, Modal } from 'react-bootstrap';
import axios from 'axios';

import { resolveClientUrl } from '../../../shared/common.functions';

export default class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            time: new Date(document.getElementById('currentDateTime').value),
            records: [],
            fields: {
                username: '',
                password: ''
            },
            modalDialog: {
                show: false,
                inOutType: 0,
                isSuccessfull: false,
                errMsg: ''
            }
        };
    }

    componentDidMount() {
        this.getCheckInOutRecord();
        this.ticker = setInterval(this.tick, 1000);
        this.intervalId = setInterval(() => this.setState({ time: new Date() }), 50);
    }

    componentWillUnmount() {
        clearInterval(this.ticker);
        clearInterval(this.intervalId);
    }

    tick = () => {
        const time = this.state.time;
        this.setState({ time: new Date(time.getTime() + 1000) });
    };

    getCheckInOutRecord = () => {
        axios({
            url: resolveClientUrl('Time/GetCheckInOutOnlineRecord'),
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            this.setState({
                records: response.data
            });
        });
    };

    validate = () => {
        const fields = this.state.fields;
        let isValid = true;
        if (!fields['username'] || !fields['password']) {
            isValid = false;
        }

        return isValid;
    };

    handleCheckInOut = inOutType => {
        const { fields } = this.state;
        const isValid = this.validate();
        if (isValid) {
            axios.post(
                resolveClientUrl('Time/CheckInOut'),
                {
                    username: fields['username'],
                    password: btoa(unescape(encodeURIComponent(fields['password']))),
                    inOutType
                },
                {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                }
            ).then(response => {
                const { data } = response;
                this.setState({
                    modalDialog: {
                        show: true,
                        inOutType,
                        isSuccessfull: data.isSuccessfull,
                        errMsg: data.errorMessage
                    }
                });

                if (data.isSuccessfull) {
                    this.getCheckInOutRecord();
                }
            });
        }
    };

    handleCheckin = event => {
        event.preventDefault();
        this.handleCheckInOut(0);
    };

    handleCheckout = event => {
        event.preventDefault();
        this.handleCheckInOut(1);
    };

    handleChange = event => {
        const fields = { ...this.state.fields };
        fields[event.target.name] = event.target.value;
        this.setState({ fields });
    };

    handleCloseModalDialog = () => {
        const { modalDialog, fields } = this.state;

        this.setState({
            fields: {
                username: modalDialog.isSuccessfull ? '' : fields.username,
                password: ''
            },
            modalDialog: {
                show: false,
                inOutType: 0,
                isSuccessfull: false,
                errMsg: ''
            }
        });
    };

    render() {
        const { modalDialog, fields, records, time } = this.state;

        const seconds = time.getSeconds() + time.getMilliseconds() / 1000;
        const minutes = time.getMinutes() + seconds / 60;
        const hours = (time.getHours() % 12) + minutes / 60;
        const secondsDeg = (seconds / 60) * 360;
        const minutesDeg = (minutes / 60) * 360;
        const hoursDeg = (hours / 12) * 360;
        return (
            <div className='app-container'>
                <div className='checkinout-flex'>
                    <div className='checkinout-form'>
                        <form>
                            <div className='checkinout-logo'>
                                <div className="clock">
                                    <div className="hand hour-hand" style={{ transform: `rotate(${hoursDeg}deg)` }} />
                                    <div className="hand minute-hand" style={{ transform: `rotate(${minutesDeg}deg)` }} />
                                    <div className="hand second-hand" style={{ transform: `rotate(${secondsDeg}deg)` }} />
                                    <div className="center-dot" />
                                    <div className="center-dot-orange" />
                                    <div className="hour-markers">
                                        {[...Array(12)].map((_, i) => {
                                            const rotation = i * 30;
                                            return (
                                                <div
                                                    key={i}
                                                    className="hour-marker"
                                                    style={{
                                                        '--rotation': `${rotation}deg`,
                                                    }}
                                                >
                                                    <span style={{ transform: `rotate(-${rotation}deg)` }}>
                                                        {i === 0 ? '12' : i}
                                                    </span>
                                                </div>
                                            );
                                        })}
                                    </div>
                                    <div className="minute-markers">
                                        {[...Array(60)].map((_, i) =>
                                            i % 5 !== 0 ? (
                                                <div
                                                    key={i}
                                                    className="minute-marker"
                                                    style={{
                                                        '--rotation': `${i * 6}deg`,
                                                    }}
                                                />
                                            ) : null
                                        )}
                                    </div>
                                </div>
                            </div>
                            <p className='checkinout-datetime'>
                                {time.toDateString()} {time.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: true })}
                            </p>
                            <div className='material-textfield'>
                                <input type='text' className='custom-input' value={fields.username}
                                    id='username' name='username' placeholder='' autoComplete='off'
                                    onChange={this.handleChange}
                                />
                                <label>Username</label>
                            </div>
                            <div className='material-textfield'>
                                <input type='password' className='custom-input' value={fields.password}
                                    id='password' name='password' placeholder='' autoComplete='off'
                                    onChange={this.handleChange}
                                />
                                <label>Password</label>
                            </div>
                            <div className='checkinout-btn'>
                                <input type='button' className='btn btn-primary' value='Check in' onClick={this.handleCheckin} />
                                <input type='button' className='btn btn-primary' value='Check out' onClick={this.handleCheckout} />
                            </div>
                        </form>
                    </div>
                    <div className='checkinout-records'>
                        <table className='table-time'>
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Emp ID</th>
                                    <th>Full Name</th>
                                    <th>Transaction Time</th>
                                    <th>In/Out</th>
                                </tr>
                            </thead>
                            <tbody>
                                {
                                    records.length === 0 &&
                                    <tr><td colSpan='5'>No data</td></tr>
                                }
                                {
                                    records.map((record, index) => {
                                        return (
                                            <tr key={record.recordTimeId}>
                                                <td>{index + 1}</td>
                                                <td>{record.code}</td>
                                                <td className='align-left'>{record.fullName}</td>
                                                <td>{record.recordTime}</td>
                                                <td>
                                                    <span className={`${record.inOutName === "In" ? "row-checkin" : "row-checkout"} row-checkinout`}>
                                                        {record.inOutName}
                                                    </span>
                                                </td>
                                            </tr>
                                        );
                                    })
                                }
                            </tbody>
                        </table>
                    </div>
                </div>
                <Modal keyboard={false} backdrop='static' show={modalDialog.show}>
                    <Modal.Body>
                        <i className={`fa ${modalDialog.isSuccessfull ? 'fa-check-circle' : 'fa-exclamation-circle'}`} />
                        <p>{modalDialog.inOutType === 0 ? 'Check in' : 'Check out'} {modalDialog.isSuccessfull ? 'successfully!' : 'unsuccessfully!'}</p>
                        {
                            modalDialog.errMsg &&
                            <p>{modalDialog.errMsg}</p>
                        }
                    </Modal.Body>
                    <Modal.Footer>
                        <Button bsStyle='primary' type='button' onClick={this.handleCloseModalDialog}>Ok</Button>
                    </Modal.Footer>
                </Modal>
            </div>
        );
    }
}