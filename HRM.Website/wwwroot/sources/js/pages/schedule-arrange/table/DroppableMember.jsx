import React, { PureComponent } from 'react';
import { Droppable, Draggable } from 'react-beautiful-dnd';

import Row from './Row';

export default class DroppableMember extends PureComponent {
    render() {
        const { isHost, shiftOptions, teamId, data } = this.props;

        return (
            <Droppable droppableId={`dropable${teamId}`} isDropDisabled={!isHost} type={`droppableMember-${teamId}`}>
                {
                    provided =>
                        <div ref={provided.innerRef} {...provided.droppableProps}>
                            {
                                data.map((member, eindex) =>
                                    <Draggable key={`member${member.employeeId}`} draggableId={`member${member.employeeId}`}
                                        index={eindex} isDragDisabled={!isHost}
                                    >
                                        {
                                            (memberProvided, memberSnapshot) =>
                                                <div>
                                                    <Row draggableProvided={memberProvided}
                                                        isHost={isHost} teamId={teamId} data={member}
                                                        shiftOptions={shiftOptions} isDragging={memberSnapshot.isDragging}
                                                        showRemarkPopover={this.props.showRemarkPopover}
                                                        onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                                                        onChangeShift={this.props.onChangeShift}
                                                        onChangeDefaultShift={this.props.onChangeDefaultShift}
                                                        onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                                                        onShowWeeklyDefaultShift={this.props.onShowWeeklyDefaultShift}
                                                        onUpdateRemark={this.props.onUpdateRemark}
                                                        onChangeOrder={this.props.onChangeOrder}
                                                        onSelectEmployeeOfTeam={this.props.onSelectEmployeeOfTeam}
                                                        enableSyncWorkingShift={this.props.enableSyncWorkingShift}
                                                    />
                                                    {memberProvided.placeholder}
                                                </div>
                                        }
                                    </Draggable>
                                )
                            }
                            {provided.placeholder}
                        </div>
                }
            </Droppable>
        );
    }
}