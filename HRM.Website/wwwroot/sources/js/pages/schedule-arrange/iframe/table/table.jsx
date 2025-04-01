import React, { PureComponent, Fragment } from 'react';

import THead from './THead';
import Row from './Row';

export default class Table extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            isFixedHeader: false
        };

        this.contentScrollElement = document.getElementsByClassName('content_scroll')[0];
    }

    componentDidMount() {
        this.contentScrollElement.addEventListener('scroll', this.handleScroll);
    }

    componentWillUnmount() {
        this.contentScrollElement.removeEventListener('scroll', this.handleScroll);
    }

    handleScroll = event => {
        const scrollTop = event.srcElement.scrollTop;
        if (scrollTop > 100) {
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
        const { fromDate, toDate, data } = this.props;

        return (
            <div className="table-wrapper">
                <div className={`table-header ${this.state.isFixedHeader ? 'fixed' : ''}`}>
                    <THead fromDate={fromDate} toDate={toDate}
                        onPrevious={this.props.onPrevious}
                        onNext={this.props.onNext}
                    />
                </div>
                <div className="table-body">
                    {
                        data.length === 0 &&
                        <div className="nodata">No information available</div>
                    }
                    {
                        data.length > 0 &&
                        data.map(team =>
                            <Fragment key={team.teamId}>
                                <div className="row-team"><div className="team-name">{team.name}</div></div>
                                {
                                    team.members.map((member, index) => <Row key={`row${index}`} data={member} />)
                                }
                            </Fragment>
                        )
                    }
                </div>
            </div>
        );
    }
}