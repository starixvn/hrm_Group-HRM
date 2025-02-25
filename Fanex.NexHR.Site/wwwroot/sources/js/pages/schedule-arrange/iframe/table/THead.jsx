import React, { PureComponent, Fragment } from 'react';
import moment from 'moment';

import ColorDescription from '../../table/ColorDescription';

export default class THead extends PureComponent {
    dayOfWeeks = [0, 1, 2, 3, 4, 5, 6];

    render() {
        const { fromDate, toDate } = this.props;

        return (
            <Fragment>
                <div className="header-control">
                    <div />
                    <div>
                        <button className="btn prev-week fa fa-chevron-left" onClick={this.props.onPrevious} />
                        <button className="btn next-week fa fa-chevron-right" onClick={this.props.onNext} />
                        <span className="label-week">{`${fromDate} - ${toDate}`}</span>
                        <ColorDescription />
                    </div>
                </div>
                <div className="header">
                    <div className="table-cell cell-index" />
                    <div className="table-cell cell-nickname">
                        Nickname
                    </div>
                    {
                        this.dayOfWeeks.map(item => {
                            return (
                                <div className="table-cell cell-shift" key={`dayOfWeeks${item}`}>
                                    {moment(fromDate).add(item, 'days').format('D MMM, ddd')}
                                </div>
                            );
                        })
                    }
                </div>
            </Fragment>
        );
    }
}