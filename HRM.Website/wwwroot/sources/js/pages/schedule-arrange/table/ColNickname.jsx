import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';

import DropdownOrder from '../../../shared/components/DropdownOrder';

export default class ColNickname extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            remark: props.remark !== null ? props.remark : '',
            showPopover: false
        };
    }

    handleShowPopover = () => {
        const { isHost, showRemarkPopover } = this.props;

        if (isHost && !this.state.showPopover && !showRemarkPopover) {
            this.setState({ showPopover: true });
            this.props.onChangeShowRemarkPopover(true);
        }
    };

    handleHidePopover = () => {
        this.setState({ showPopover: false });
        this.props.onChangeShowRemarkPopover(false);
    };

    handleShowWeeklyDefaultShiftModal = () => {
        const { teamId, employeeId } = this.props;
        this.props.onShowWeeklyDefaultShift(teamId, employeeId);
    };

    handleChangeRemark = event => {
        this.setState({ remark: event.target.value });
    };

    handleUpdateRemark = () => {
        const remark = this.state.remark.trim();
        const { teamId, employeeId } = this.props;

        this.handleHidePopover();
        this.props.onChangeShowRemarkPopover(false);
        this.props.onUpdateRemark(teamId, employeeId, remark);
    };

    handleMoveUp = () => {
        const { teamId, employeeId } = this.props;
        this.props.onChangeOrder('up', teamId, employeeId);
    };

    handleMoveDown = () => {
        const { teamId, employeeId } = this.props;
        this.props.onChangeOrder('down', teamId, employeeId);
    };

    handleMoveTop = () => {
        const { teamId, employeeId } = this.props;
        this.props.onChangeOrder('top', teamId, employeeId);
    };

    handleMoveBottom = () => {
        const { teamId, employeeId } = this.props;
        this.props.onChangeOrder('bottom', teamId, employeeId);
    };

    render() {
        const { isHost, employeeId, nickname, remark } = this.props;

        return (
            <div className="table-cell cell-nickname">
                <div className="cell-nickname-wrapper">
                    <div className="info">
                        <div className="nickname">{nickname}</div>
                        <div className="remark-wrapper">
                            <i className={`icon-remark ${remark !== null && remark.length > 0 ? 'has-remark' : ''} fa fa-commenting-o`}
                                title="Remark"
                                onClick={this.handleShowPopover}
                            />
                            <OverlayTrigger trigger={['hover']} placement={'top'}
                                overlay={<Tooltip id={`remark${employeeId}`}>{remark}</Tooltip>}
                            >
                                <span className="remark">{remark}</span>
                            </OverlayTrigger>
                            {
                                this.state.showPopover &&
                                <div className="remark-popover">
                                    <div className="input-field">
                                        <label>Remark</label>
                                        <textarea maxLength="200" autoFocus value={this.state.remark} onChange={this.handleChangeRemark} />
                                    </div>
                                    <div className="btn-control">
                                        <a className="btn btn-primary" onClick={this.handleUpdateRemark}>Save</a>
                                        <a className="btn btn-default" onClick={this.handleHidePopover}>Cancel</a>
                                    </div>
                                </div>
                            }
                        </div>
                    </div>
                    <div className="action">
                        <i className="icon icon-default-shift" title="Weekly default shift"
                            onClick={this.handleShowWeeklyDefaultShiftModal}
                        />
                        {
                            isHost &&
                            <DropdownOrder id={`dropdown${employeeId}`}
                                onMoveUp={this.handleMoveUp}
                                onMoveDown={this.handleMoveDown}
                                onMoveTop={this.handleMoveTop}
                                onMoveBottom={this.handleMoveBottom}
                            />
                        }
                    </div>
                </div>
            </div>
        );
    }
}