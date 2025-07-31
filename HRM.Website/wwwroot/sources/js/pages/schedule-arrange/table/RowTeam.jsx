import React, { PureComponent } from 'react';

import DroppableMember from './DroppableMember';
import DropdownOrder from '../../../shared/components/DropdownOrder';

export default class RowTeam extends PureComponent {
    handleMoveUp = () => {
        const { teamId } = this.props.data;
        this.props.onChangeTeamOrder('up', teamId);
    };

    handleMoveDown = () => {
        const { teamId } = this.props.data;
        this.props.onChangeTeamOrder('down', teamId);
    };

    handleMoveTop = () => {
        const { teamId } = this.props.data;
        this.props.onChangeTeamOrder('top', teamId);
    };

    handleMoveBottom = () => {
        const { teamId } = this.props.data;
        this.props.onChangeTeamOrder('bottom', teamId);
    };

    handleChangePublic = (event) => {
        const { teamId } = this.props.data;
        this.props.onChangePublics(teamId, event.target.checked);
    };

    handleSelectEmployeeOfTeam = (event) => {
        const { teamId } = this.props.data;
        this.props.onSelectEmployeeOfTeam(teamId, null, event.target.checked);
    };

    render() {
        const {
            isHost,
            draggableProvided,
            isDragging,
            shiftOptions,
            enableSyncWorkingShift,
        } = this.props;
        const { teamId, name, isPublic, isSelected, members } = this.props.data;

        return (
            <div
                className={`${isDragging ? 'dragging' : ''}`}
                ref={draggableProvided.innerRef}
                {...draggableProvided.draggableProps}
                {...draggableProvided.dragHandleProps}>
                <div className="row-team">
                    <div className="team-name">
                        <div>{name}</div>
                        {isHost && (
                            <DropdownOrder
                                id={`dropdownTeam${teamId}`}
                                onMoveUp={this.handleMoveUp}
                                onMoveDown={this.handleMoveDown}
                                onMoveTop={this.handleMoveTop}
                                onMoveBottom={this.handleMoveBottom}
                            />
                        )}
                    </div>
                    <div className="right-container">
                        {isHost && enableSyncWorkingShift && (
                            <div className="cell-syncdata">
                                <div className="checkbox">
                                    <input
                                        type="checkbox"
                                        className={`checkbox-custom`}
                                        id={`ckb-syncdata-team${teamId}`}
                                        checked={isSelected}
                                        onChange={
                                            this.handleSelectEmployeeOfTeam
                                        }
                                    />
                                    <label
                                        htmlFor={`ckb-syncdata-team${teamId}`}
                                    />
                                </div>
                            </div>
                        )}
                        <div className="cell-public">
                            <div className="checkbox">
                                <input
                                    type="checkbox"
                                    className={`checkbox-custom`}
                                    id={`ckb-public-team${teamId}`}
                                    checked={isPublic}
                                    disabled={!isHost}
                                    onChange={this.handleChangePublic}
                                />
                                <label htmlFor={`ckb-public-team${teamId}`}>
                                    {' '}
                                    Publish
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
                <DroppableMember
                    isHost={isHost}
                    teamId={teamId}
                    data={members}
                    shiftOptions={shiftOptions}
                    showRemarkPopover={this.props.showRemarkPopover}
                    onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                    onChangeShift={this.props.onChangeShift}
                    onChangeDefaultShift={this.props.onChangeDefaultShift}
                    onChangeShowRemarkPopover={
                        this.props.onChangeShowRemarkPopover
                    }
                    onShowWeeklyDefaultShift={
                        this.props.onShowWeeklyDefaultShift
                    }
                    onUpdateRemark={this.props.onUpdateRemark}
                    onChangeOrder={this.props.onChangeOrder}
                    onSelectEmployeeOfTeam={this.props.onSelectEmployeeOfTeam}
                    enableSyncWorkingShift={this.props.enableSyncWorkingShift}
                />
            </div>
        );
    }
}
