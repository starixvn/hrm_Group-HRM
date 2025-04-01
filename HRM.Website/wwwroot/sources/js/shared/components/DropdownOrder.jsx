import React, { PureComponent } from 'react';
import { Dropdown, MenuItem } from 'react-bootstrap';

export default class DropdownOrder extends PureComponent {
    render() {
        return (
            <Dropdown id={this.props.id} componentClass="div" className="customize-order">
                <Dropdown.Toggle noCaret useAnchor>
                    <i className="icon icon-sortable" title="Customize order" />
                </Dropdown.Toggle>
                <Dropdown.Menu>
                    <MenuItem onClick={this.props.onMoveUp}>Up</MenuItem>
                    <MenuItem onClick={this.props.onMoveDown}>Down</MenuItem>
                    <MenuItem onClick={this.props.onMoveTop}>Top</MenuItem>
                    <MenuItem onClick={this.props.onMoveBottom}>Bottom</MenuItem>
                </Dropdown.Menu>
            </Dropdown>
        );
    }
}