import React, { Component } from 'react';
import { Dropdown, MenuItem } from 'react-bootstrap';

import Autocomplete from '../../shared/components/Autocomplete';
import TreeMultiselect from '../../shared/components/react-tree-multiselect/TreeMultiselect';
import WeekPicker from '../../shared/components/WeekPicker';

export default class Search extends Component {
    handleTeamChange = selectedItems => {
        const ids = selectedItems.map(item => item.id);
        this.props.onChange('teamIds', ids.join(','));
    };

    handleEmployeeChange = value => {
        this.props.onChange('employee', value);
    };

    handleDateChange = (fromDate, toDate) => {
        this.props.onChange('fromDate', fromDate);
        this.props.onChange('toDate', toDate);
    };

    handleCategoryChange = e => {
        this.props.onChange('isLocal', e.target.value);
    };

    handleSubmit = event => {
        this.props.onSubmit(event.target.name === 'currentWeek');
    };
    render() {
        return (
            <div className="filter">
                <div className="form-horizontal clearfix">
                    <div className="form-group form-group--l">
                        <div className="label-form">
                            <label className="control-label">Week</label>
                        </div>
                        <div className="calendar-form">
                            <WeekPicker
                                fromDate={this.props.filter.fromDate}
                                toDate={this.props.filter.toDate}
                                onChange={(fromDate, toDate) => {
                                    this.props.onChange('fromDate', fromDate);
                                    this.props.onChange('toDate', toDate);
                                }}
                            />
                        </div>
                    </div>
                    <div className="form-group form-group--m">
                        <div className="label-form">
                            <label className="control-label">Team</label>
                        </div>
                        <div className="select-form">
                            <TreeMultiselect
                                data={this.props.teamOptions}
                                isSelectedAll
                                onChange={this.handleTeamChange}
                            />
                        </div>
                    </div>

                    <div className="form-group form-group--m">
                        <div className="label-form">
                            <label className="control-label">Category</label>
                        </div>
                        <div className="select-form">
                            <select
                                className="form-control"
                                value={this.props.filter.isLocal}
                                onChange={this.handleCategoryChange}>
                                <option value>All</option>
                                <option value="0">Expat</option>
                                <option value="1">Local</option>
                            </select>
                        </div>
                    </div>

                    <div className="form-group form-group--m">
                        <div className="label-form">
                            <label className="control-label">Employee</label>
                        </div>
                        <div className="select-form">
                            <Autocomplete
                                className="form-control"
                                sources={this.props.employeeOptions}
                                onInput={this.props.onSearchEmployees}
                                onChange={this.handleEmployeeChange}
                            />
                        </div>
                    </div>
                </div>
                <div className="form-horizontal clearfix">
                    <div className="button-group">
                        <button
                            className="btn btn-primary"
                            onClick={this.handleSubmit}>
                            Submit
                        </button>
                        <button
                            className="btn btn-primary"
                            name="currentWeek"
                            onClick={this.handleSubmit}>
                            Current Week
                        </button>
                        {this.props.showExportExcel && (
                            <Dropdown
                                id={`dropdownExportExcel`}
                                componentClass="div">
                                <Dropdown.Toggle
                                    useAnchor
                                    className="btn btn-excel">
                                    Excel
                                </Dropdown.Toggle>
                                <Dropdown.Menu>
                                    <MenuItem
                                        onClick={this.props.onExportExcel}>
                                        Upload
                                    </MenuItem>
                                    <MenuItem
                                        onClick={
                                            this.props.onExportExcelDriverPickup
                                        }>
                                        Driver Pickup
                                    </MenuItem>
                                </Dropdown.Menu>
                            </Dropdown>
                        )}
                    </div>
                </div>
            </div>
        );
    }
}

