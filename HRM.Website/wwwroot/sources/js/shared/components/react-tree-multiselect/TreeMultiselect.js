import React from 'react';
import PropTypes from 'prop-types';

import buildData from './buildData';
import TreeNode from './TreeNode';

const updateParent = (floatData, parentKey, isChecked) => {
    const index = floatData.findIndex(item => item.key === parentKey),
        siblings = floatData.filter(item => item.parentKey === parentKey),
        selected = siblings.filter(item => item.checked);
    floatData[index] = { ...floatData[index], checked: siblings.length === selected.length };

    if (floatData[index].parentKey !== null && floatData[index].parentKey !== '') {
        updateParent(floatData, floatData[index].parentKey, isChecked);
    }
};

const updateChild = (floatData, itemkey, isChecked) => {
    const totalItems = floatData.length;
    for (let i = 0; i < totalItems; i++) {
        if (floatData[i].parentKey === itemkey) {
            floatData[i] = { ...floatData[i], checked: isChecked };
            updateChild(floatData, floatData[i].key, isChecked);
        }
    }
};

const getSelectedText = floatData => {
    const items = floatData.filter(item => item.isLeaf),
        selected = items.filter(item => item.checked);
    let text = '';

    if (selected.length === 0) {
        text = 'Select options';
    } else if (items.length === selected.length) {
        text = 'All selected';
    } else {
        text = `${selected.length} selected`;
    }

    return text;
};

const getSelectedItems = floatData => {
    return floatData.filter(item => item.isLeaf && item.checked);
};

let id = 1;

export default class TreeMultiselect extends React.Component {
    static propTypes = {
        data: PropTypes.arrayOf(PropTypes.object).isRequired,
        isSelectedAll: PropTypes.bool,
        onChange: PropTypes.func,
        hasSearchBox: PropTypes.bool,
        leafOnly: PropTypes.bool,
    }

    static defaultProps = {
        data: [],
        isSelectedAll: false,
        hasSearchBox: false,
        leafOnly: false,
        onChange: function () { }
    }

    static getDerivedStateFromProps(props, state) {
        if (state.original === props.data) {
            return null;
        }
        return { ...buildData(props.data || [], props.isSelectedAll), original: props.data };
    }

    constructor(props) {
        super(props);
        this.id = `multiselect-treeview-${props.id || id++}`;
        this.state = {
            selectedText: 'Select options',
            opened: false,
            searchText: ''
        };
    }

    componentDidMount() {
        document.addEventListener('click', this.handleDocumentClick);
    }

    componentWillUnmount() {
        document.removeEventListener('click', this.handleDocumentClick);
    }

    handleDocumentClick = event => {
        const clickOutside = this.wrapper && !this.wrapper.contains(event.target);
        if (clickOutside && this.state.opened) {
            this.setState({ opened: false });
        }
    }

    onToggle = () => {
        this.setState(prevState => { return { opened: !prevState.opened }; });
    }

    handleSelectAll = event => {
        let floatData = [...this.state.floatData];
        const totalItems = floatData.length;
        for (let i = 0; i < totalItems; i++) {
            floatData[i] = { ...floatData[i], checked: event.target.checked };
        }

        this.props.onChange && this.props.onChange(getSelectedItems(floatData));
        this.setState({ floatData });
    }

    handleSelect = (itemkey, isChecked) => {
        let floatData = [...this.state.floatData];
        const index = floatData.findIndex(item => item.key === itemkey);
        floatData[index] = { ...floatData[index], checked: isChecked };

        updateChild(floatData, itemkey, isChecked);
        if (floatData[index].parentKey !== null && floatData[index].parentKey !== '') {
            updateParent(floatData, floatData[index].parentKey, isChecked);
        }

        this.props.onChange && this.props.onChange(getSelectedItems(floatData));
        this.setState({ floatData });
    }

    renderNodes = (data, floatData, searchText) => {
        const filter = (node) => {
            if (!searchText) return true;
            if (node.text.toLowerCase().includes(searchText.toLowerCase())) return true;
            if (node.items && node.items.some(filter)) return true;
            return false;
        };

        const renderRecursive = (nodes) => (
            nodes.filter(filter).map(item => (
                <ul key={item.key}>
                    <li className="multiselect-treeview-item">
                        <TreeNode id={this.id}
                            data={item}
                            floatData={floatData}
                            onSelect={this.handleSelect}
                            leafOnly={this.props.leafOnly}
                        />
                    </li>
                </ul>
            ))
        );

        return renderRecursive(data);
    };

    render() {
        const { floatData, data } = this.state;
        const selected = floatData.filter(item => item.checked);
        const selectedText = getSelectedText(floatData);
        const checkboxAllElementId = `${this.id}-selectAll`;

        return (
            <div id={this.id} className="multiselect-treeview" ref={element => this.wrapper = element}>
                <div className="multiselect-treeview-dropdown-button" onClick={this.onToggle}>
                    <span className="selected-text">{selectedText}</span>
                    <span className="up-down-icon"><span className={this.state.opened ? 'up' : 'down'} /></span>
                </div>
                <div className={`multiselect-treeview-dropdown-list ${this.state.opened ? '' : 'hidden'}`}>
                    {this.props.hasSearchBox && (
                        <div className="multiselect-treeview-search">
                            <input
                                type="text"
                                placeholder="Search..."
                                className="form-control"
                                value={this.state.searchText}
                                onChange={e => this.setState({ searchText: e.target.value })}
                            />
                        </div>
                    )}
                    <div className="multiselect-treeview-select-all">
                        <div className="checkbox-wrapper">
                            <input type="checkbox" id={checkboxAllElementId}
                                checked={floatData.length === selected.length}
                                onChange={this.handleSelectAll}
                            />
                            <label htmlFor={checkboxAllElementId}> Select all</label>
                        </div>
                    </div>
                    <div className="multiselect-treeview-items">
                        {this.renderNodes(data, floatData, this.state.searchText)}
                    </div>
                </div>
            </div>
        );
    }
}