import React, { Component } from 'react';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../shared/common.functions';

export default class Login extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fields: {},
            errors: {},
            errMsg: '',
            isShowErr: false
        };
    }

    validate = () => {
        const fields = this.state.fields;
        const errors = {};
        let isValid = true;
        if (!fields['username']) {
            isValid = false;
            errors['username'] = 'The Username field is required.';
        }

        if (!fields['password']) {
            isValid = false;
            errors['password'] = 'The Password field is required.';
        }

        this.setState({ errors: errors, isShowErr: !isValid });
        return isValid;
    };

    handleLogin = event => {
        event.preventDefault();
        const { fields } = this.state;
        const isValid = this.validate();
        if (isValid) {
            axios.post(
                resolveClientUrl('Auth/Login'),
                {
                    username: fields['username'],
                    password: btoa(unescape(encodeURIComponent(fields['password'])))
                },
                {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                }
            ).then(response => {
                const { data } = response;
                if (data.isSuccessfull) {
                    this.setState({
                        errMsg: '',
                        isShowErr: false
                    });
                    window.location.href = ('/');
                } else {
                    this.setState({
                        errMsg: data.errorMessage,
                        isShowErr: true
                    });
                }
            }).catch(axiosError);
        }
    }

    handleChange = event => {
        const fields = this.state.fields;
        fields[event.target.name] = event.target.value;
        this.setState({ fields });
    };

    render() {
        const { isShowErr, errors, errMsg } = this.state;
        return (
            <form onSubmit={this.handleLogin} id="formContainer">
                {
                    isShowErr && <div className="validation-summary-errors">
                        <ul>
                            {errors["username"] && <li>{errors["username"]}</li>}
                            {errors["password"] && <li>{errors["password"]}</li>}
                            {errMsg && <li>{errMsg}</li>}

                        </ul>
                    </div>
                }
                <div className="textbox-outer">
                    <div className="username-icon" />
                    <div className="username-field">
                        <input type="text" placeholder="Username" name="username" onChange={this.handleChange} />
                    </div>
                </div>
                <div className="textbox-outer">
                    <div className="password-icon" />
                    <div className="password-field">
                        <input type="password" placeholder="Password" name="password" onChange={this.handleChange} />
                    </div>
                </div>
                <div className="signin-button">
                    <input type="submit" name="submit" className="button" value="LOGIN" />
                </div>
            </form>
        );
    }
}