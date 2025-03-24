import React, { PureComponent } from 'react';
import { Dropdown, MenuItem } from 'react-bootstrap';

export default class ColorDescription extends PureComponent {
    render() {
        return (
            <Dropdown id={`dropdownColorDescription`} componentClass="span" className="dropdown-color">
                <Dropdown.Toggle noCaret className="btn">
                    <img src="../sources/styles/images/pantone.svg" width="17" />
                </Dropdown.Toggle>
                <Dropdown.Menu>
                    <MenuItem><span className="holiday" />Public Holiday</MenuItem>
                    <MenuItem><span className="offday" />Off Day</MenuItem>
                    <MenuItem><span className="offdayreplacement" />Off Replacement</MenuItem>
                    <MenuItem><span className="keepinviewday" />Compensation Leave</MenuItem>
                    <MenuItem><span className="changeshift" />Change Shift</MenuItem>
                    <MenuItem><span className="overtime" />Overtime</MenuItem>
                    <MenuItem><span className="annualleave" />Annual Leave</MenuItem>
                    <MenuItem><span className="unpaidleave" />Unpaid Leave</MenuItem>
                    <MenuItem><span className="businesstrip" />Business Trip</MenuItem>
                    <MenuItem><span className="weddingleave" />Wedding Leave</MenuItem>
                    <MenuItem><span className="leavehalfday2" />2 Leaves Half Day</MenuItem>
                    <MenuItem><span className="leaveday" />Others Leave</MenuItem>
                </Dropdown.Menu>
            </Dropdown>
        );
    }
}