import React, { PureComponent } from "react";

export default class Row extends PureComponent {
    handleRemoveEmployee = () => {
        const { ownerId } = this.props.data;
        this.props.onRemoveEmployee(ownerId);
    };

    render() {
        const { data } = this.props;
        return (
            <tr>
                <td className="align-left cell-name">
                    <div className="wrapper">
                        <div>{`${data.nickname}`}</div>
                    </div>
                </td>
                <td>
                    {data.canRemove && (
                        <button
                            type="button"
                            className="fa fa-times fa-lg icon-delete btn-link"
                            onClick={this.handleRemoveEmployee}
                        />
                    )}
                </td>
            </tr>
        );
    }
}
