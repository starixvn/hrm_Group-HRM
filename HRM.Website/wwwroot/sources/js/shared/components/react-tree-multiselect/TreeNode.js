import React, { PureComponent, Fragment } from 'react';

export default class TreeNode extends PureComponent {
    handleChange = event => {
        const itemkey = this.props.data.key;
        this.props.onSelect(itemkey, event.target.checked);
    }

    render() {
        const { data, floatData } = this.props;
        const itemIndex = floatData.findIndex(item => item.key === data.key);
        const checkboxElementId = `${this.props.id}-${data.key}`;

        return (
            <Fragment>
                <div className="checkbox-wrapper">
                    <input type="checkbox" id={checkboxElementId} checked={floatData[itemIndex].checked} onChange={this.handleChange} />
                    <label htmlFor={checkboxElementId}> {data.text}</label>
                </div>
                {
                    data.items && data.items.length > 0 &&
                    <ul>
                        {
                            data.items.map(item =>
                                <li key={item.key} className="multiselect-treeview-item is-child">
                                    <TreeNode
                                        data={item}
                                        floatData={floatData}
                                        onSelect={this.props.onSelect}
                                    />
                                </li>
                            )
                        }
                    </ul>
                }
            </Fragment>
        );
    }
}