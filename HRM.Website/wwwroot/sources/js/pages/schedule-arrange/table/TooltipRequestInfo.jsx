import React from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';

const TooltipRequestInfo = ({ id, requestInfo, isLeaveHalfDay }) => {
    return (
        <OverlayTrigger trigger={['hover']} placement={'top'}
            overlay={
                <Tooltip id={id}>
                    {requestInfo.details}
                </Tooltip>
            }
        >
            {
                isLeaveHalfDay
                    ? <span className={`halfdaycode ${requestInfo.classOfLeave}`}>{requestInfo.leaveCode}</span>
                    : <i className="fa fa-info-circle" />
            }
        </OverlayTrigger>
    );
};

export default TooltipRequestInfo;