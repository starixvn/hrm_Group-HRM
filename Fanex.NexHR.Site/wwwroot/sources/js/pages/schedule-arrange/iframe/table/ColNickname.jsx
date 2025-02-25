import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';

export default class ColNickname extends PureComponent {
    render() {
        const { employeeId, nickname, remark } = this.props;

        return (
            <div className="table-cell cell-nickname">
                <div className="cell-nickname-wrapper">
                    <div className="info">
                        <div className="nickname">{nickname}</div>
                        <div className="remark-wrapper">
                            <i className={`icon-remark ${remark !== null && remark.length > 0 ? 'has-remark' : ''} fa fa-commenting-o`}
                                title="Remark"
                            />
                            <OverlayTrigger trigger={['hover']} placement={'top'}
                                overlay={<Tooltip id={`remark${employeeId}`}>{remark}</Tooltip>}
                            >
                                <span className="remark">{remark}</span>
                            </OverlayTrigger>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}