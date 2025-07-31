import React, { PureComponent, Fragment } from 'react';
import { DragDropContext } from 'react-beautiful-dnd';

import THead from './THead';
import DroppableTeam from './DroppableTeam';

export default class Table extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            isFixedHeader: false,
        };

        this.contentScrollElement =
            document.getElementsByClassName('content_scroll')[0];
    }

    componentDidMount() {
        this.contentScrollElement.addEventListener('scroll', this.handleScroll);
    }

    componentWillUnmount() {
        this.contentScrollElement.removeEventListener(
            'scroll',
            this.handleScroll
        );
    }

    handleScroll = (event) => {
        const scrollTop = event.srcElement.scrollTop;
        if (scrollTop > 70) {
            if (!this.state.isFixedHeader) {
                this.setState({ isFixedHeader: true });
            }
        } else {
            if (this.state.isFixedHeader) {
                this.setState({ isFixedHeader: false });
            }
        }
    };

    render() {
        const {
            isHost,
            fromDate,
            toDate,
            shiftOptions,
            data,
            enableSyncWorkingShift,
        } = this.props;

        return (
            <Fragment>
                <div
                    className={`table-wrapper ${
                        isHost && enableSyncWorkingShift ? 'has-syncdata' : ''
                    }`}>
                    <div
                        className={`table-header ${
                            this.state.isFixedHeader ? 'fixed' : ''
                        }`}>
                        <THead
                            fromDate={fromDate}
                            toDate={toDate}
                            isHost={isHost}
                            onSave={this.props.onSave}
                            onChangeSelectWeekDays={
                                this.props.onChangeSelectWeekDays
                            }
                            onChangeOffNoShift={this.props.onChangeOffNoShift}
                            onChangeOffdayOT={this.props.onChangeOffdayOT}
                            onChangeWorkday={this.props.onChangeWorkday}
                            onShiftMultiple={this.props.onShiftMultiple}
                            onFixedOffDay={this.props.onFixedOffDay}
                            onPrevious={this.props.onPrevious}
                            onNext={this.props.onNext}
                            onSyncData={this.props.onSyncData}
                            enableSyncWorkingShift={enableSyncWorkingShift}
                        />
                    </div>
                    <div className="table-body">
                        {data.length === 0 && (
                            <div className="nodata">
                                No information available
                            </div>
                        )}
                        {data.length > 0 && (
                            <DragDropContext onDragEnd={this.props.onDragEnd}>
                                <DroppableTeam
                                    isHost={isHost}
                                    data={data}
                                    shiftOptions={shiftOptions}
                                    enableSyncWorkingShift={
                                        enableSyncWorkingShift
                                    }
                                    showRemarkPopover={
                                        this.props.showRemarkPopover
                                    }
                                    onChangeSelectWeekDay={
                                        this.props.onChangeSelectWeekDay
                                    }
                                    onChangeShift={this.props.onChangeShift}
                                    onChangeDefaultShift={
                                        this.props.onChangeDefaultShift
                                    }
                                    onChangePublics={this.props.onChangePublics}
                                    onChangeShowRemarkPopover={
                                        this.props.onChangeShowRemarkPopover
                                    }
                                    onShowWeeklyDefaultShift={
                                        this.props.onShowWeeklyDefaultShift
                                    }
                                    onUpdateRemark={this.props.onUpdateRemark}
                                    onChangeTeamOrder={
                                        this.props.onChangeTeamOrder
                                    }
                                    onChangeOrder={this.props.onChangeOrder}
                                    onSelectEmployeeOfTeam={
                                        this.props.onSelectEmployeeOfTeam
                                    }
                                />
                            </DragDropContext>
                        )}
                    </div>
                </div>
            </Fragment>
        );
    }
}
