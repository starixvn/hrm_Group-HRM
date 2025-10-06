import React, { PureComponent } from "react";
import { PageType } from "../../../../shared/constants";

export default class Row extends PureComponent {
    handleRemoveEmployee = () => {
        const { ownerId } = this.props.data;
        this.props.onRemoveEmployee(ownerId);
    };

    render() {
        const { data, pageType } = this.props;
        const isTeam = pageType === PageType.Team;
        const isGroup = pageType === PageType.GroupTeam;
        
        return (
            <tr>
                <td className="align-left cell-name">
                    <div className="wrapper">
                        <div>{`${data.nickname}`}</div>
                    </div>
                </td>
                <td>
                    {(isGroup || (isTeam && data.canRemove)) && (
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
