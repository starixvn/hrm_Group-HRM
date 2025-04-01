import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';

export default class Autocomplete extends Component {
    static propTypes = {
        sources: PropTypes.arrayOf(PropTypes.object),
        className: PropTypes.string,
        isDisabled: PropTypes.bool,
        onInput: PropTypes.func,
        onChange: PropTypes.func
    };

    static defaultProps = {
        sources: [],
        className: '',
        isDisabled: false,
        onInput: function () {
        },
        onChange: function () {
        }
    };

    componentDidMount() {
        this.$el = $(this.el);
        this.updateAutocomplete();
    }

    shouldComponentUpdate(nextProps) {
        return this.props.sources !== nextProps.sources
            || this.props.className !== nextProps.className
            || this.props.isDisabled !== nextProps.isDisabled;
    }

    componentDidUpdate() {
        this.updateAutocomplete();
    }

    componentWillUnmount() {
        this.$el.autocomplete('destroy');
    }

    updateAutocomplete = () => {
        const sources = this.props.sources;
        this.$el.autocomplete({
            source: sources,
            minLength: 2,
            select: (event, ui) => this.handleChange(event, ui),
            change: (event, ui) => this.handleChange(event, ui)
        });
    };

    handleInput = event => {
        this.props.onInput(event.target.value);
    };

    handleChange = (event, ui) => {
        let value = 0,
            label = '';
        if (ui && ui.item) {
            label = ui.item.value;
            value = ui.item.id;
        } else {
            if (this.$el.value === '') {
                value = '';
            }
        }
        this.props.onChange({ value, label });
    };

    render() {
        return (
            <Fragment>
                {
                   <input
                        ref={el => this.el = el}
                        type="text"
                        className={this.props.className}
                        disabled={this.props.isDisabled}
                        onInput={this.handleInput}
                    />
                }
            </Fragment>
        );
    }
}