import React, { PureComponent } from 'react';

import DropdownOrder from '../../../../shared/components/DropdownOrder';
import { moveUp, moveDown, moveTop, moveBottom } from '../../supervisor/common.functions';

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
                        <div>{`${data.nickname} (${data.fullName})`}</div>
                        <DropdownOrder id={`dropdown${data.ownerId}`}
                            onMoveUp={() => moveUp(this)}
                            onMoveDown={() => moveDown(this)}
                            onMoveTop={() => moveTop(this)}
                            onMoveBottom={() => moveBottom(this)}
                        />
                    </div>
                </td>
                <td>
                    <a className="fa fa-times fa-lg icon-delete" href="javascript:;"
                        onClick={this.handleRemoveEmployee}
                    />
                </td>
            </tr>
        );
    }
}