import React, { PureComponent } from 'react';

import THead from './THead';
import Row from './Row';

export default class Table extends PureComponent {
    render() {
        const { data, employeeOptions, roleId } = this.props;

        return (
            <table className="table table-custom">
                <THead roleId={roleId} employeeOptions={employeeOptions}
                    onSearchEmployees={this.props.onSearchEmployees}
                    onAddEmployee={this.props.onAddEmployee}
                />
                <tbody>
                    {
                        data.length === 0 &&
                        <tr className="nodata"><td colSpan={9}>No information available</td></tr>
                    }
                    {
                        data.length > 0 &&
                        data.map((manager, index) =>
                            <Row key={`row${index}`} data={manager}
                                onRemoveEmployee={this.props.onRemoveEmployee}
                                onChangePermissionOnRequest={this.props.onChangePermissionOnRequest}
                                onChangeOrder={this.props.onChangeOrder}
                            />
                        )
                    }
                </tbody>
            </table>
        );
    }
}