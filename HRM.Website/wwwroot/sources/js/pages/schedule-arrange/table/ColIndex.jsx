import React, { PureComponent } from 'react';
import { getTotalOffDays } from '../common.functions';

export default class ColIndex extends PureComponent {
    render() {
        const { data } = this.props;
        const totalOffDaysDefault = data.offDaysbyWeek + data.totalOffDayReplacement,
            totalOffDays = getTotalOffDays(data);

        return (
            <div className="table-cell cell-index">
                {totalOffDaysDefault < totalOffDays ? <i className="fa fa-exclamation-triangle" /> : ''}
            </div>
        );
    }
}