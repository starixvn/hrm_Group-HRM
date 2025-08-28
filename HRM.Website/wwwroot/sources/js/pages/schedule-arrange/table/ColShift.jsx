import React, { PureComponent } from 'react';

import { OffDayShiftId } from '../../../shared/constants';
import { getColumnShiftInfo } from '../common.functions';
import ShiftAutocomplete from '../ShiftAutocomplete';
import TooltipRequestInfo from './TooltipRequestInfo';

export default class ColShift extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            isShowCheckbox: props.isChecked
        };
    }

    componentDidUpdate(prevProps) {
        if (this.props.isChecked !== prevProps.isChecked) {
            this.setState({ isShowCheckbox: this.props.isChecked });
        }
    }

    handleChangeSelectWeekDay = event => {
        this.props.onChangeSelectWeekDay(this.props.scheduleAttrName, event.target.checked);
    };

    handleChangeShift = (scheduleAttrName, shift) => {
        this.props.onChangeShift(scheduleAttrName, shift);
    };

    handleMouseOver = () => {
        if (!this.state.isShowCheckbox) {
            this.setState({ isShowCheckbox: true });
        }
    };

    handleMouseLeave = () => {
        if (this.state.isShowCheckbox && !this.props.isChecked) {
            this.setState({ isShowCheckbox: false });
        }
    };

    isSkipOffDay = data => data.isLeaveHalfDay || data.isChangeShift || (data.isKeepInView && data.isOffDay);

    render() {
        const { employeeId, data, isHost, scheduleAttrName, defaultShift, shiftOptions, isUsedWeeklyDefaultShift } = this.props;
        const shiftInfo = getColumnShiftInfo(data, isHost, defaultShift, isUsedWeeklyDefaultShift);

        let shift;
        if (data.shift === null) {
            shift = { code: shiftInfo.value, name: shiftInfo.value };
        } else if (data.shift.code !== shiftInfo.value) {
            shift = { ...data.shift, code: shiftInfo.value, name: shiftInfo.value };
        } else {
            shift = data.shift;
        }

        return (
            <div className="table-cell cell-shift" onMouseOver={this.handleMouseOver} onMouseLeave={this.handleMouseLeave}>
                <div className={`cell-shift-wrapper ${data.isLeaveHalfDay ? 'leavehalfday' : ''}`}>
                    {
                        shiftInfo.details !== '' &&
                        <TooltipRequestInfo id={`moreInfo-${scheduleAttrName}${employeeId}`}
                            requestInfo={shiftInfo}
                            isLeaveHalfDay={data.isLeaveHalfDay}
                        />
                    }
                    <ShiftAutocomplete
                        shift={shift}
                        name={scheduleAttrName}
                        className={shiftInfo.class}
                        placeholder={shiftInfo.placeholder}
                        isDisabled={shiftInfo.isDisabled}
                        shiftOptions={this.isSkipOffDay(data) ? shiftOptions.filter(item => item.id !== OffDayShiftId) : shiftOptions}
                        onChange={this.handleChangeShift}
                    />
                    <div className={`checkbox ${shiftInfo.isShowCheckBox && this.state.isShowCheckbox ? '' : 'hidden'}`}>
                        <input type="checkbox" className={`checkbox-custom ${scheduleAttrName}`} checked={this.props.isChecked}
                            id={`ckb-${scheduleAttrName}${employeeId}`} onChange={this.handleChangeSelectWeekDay}
                        />
                        <label htmlFor={`ckb-${scheduleAttrName}${employeeId}`} />
                    </div>
                </div>
            </div>
        );
    }
}