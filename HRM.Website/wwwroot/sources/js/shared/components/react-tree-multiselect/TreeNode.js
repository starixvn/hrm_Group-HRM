import React, { PureComponent, Fragment } from 'react';

export default class TreeNode extends PureComponent {
    handleChange = event => {
        const itemkey = this.props.data.key;
        this.props.onSelect(itemkey, event.target.checked);
    }

    render() {
        const { data, floatData, singleSelect, leafOnly } = this.props;
        const itemIndex = floatData.findIndex(item => item.key === data.key);
        const checkboxElementId = `${this.props.id}-${data.key}`;
        const isChecked = floatData[itemIndex].checked;

        return (
            <Fragment>
                <div className="checkbox-wrapper">
                    {(data.isLeaf || !leafOnly) && (
                        <input 
                            type="checkbox" 
                            id={checkboxElementId} 
                            checked={isChecked} 
                            onChange={this.handleChange} 
                        />
                    )}
                    <label htmlFor={checkboxElementId}> {data.text}</label>
                </div>
                {
                    data.items && data.items.length > 0 &&
                    <ul>
                        {
                            data.items.map(item =>
                                <li key={item.key} className="multiselect-treeview-item is-child">
                                    <TreeNode
                                        id={this.props.id}
                                        data={item}
                                        floatData={floatData}
                                        onSelect={this.props.onSelect}
                                        leafOnly={leafOnly}
                                        singleSelect={singleSelect}
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