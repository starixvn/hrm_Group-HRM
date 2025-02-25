import React, { PureComponent } from 'react';
import PropTypes from 'prop-types';
import Autocomplete from 'react-autocomplete';

export default class ShiftAutocomplete extends PureComponent {
    static propTypes = {
        shift: PropTypes.object,
        shiftOptions: PropTypes.arrayOf(PropTypes.object),
        name: PropTypes.string,
        className: PropTypes.string,
        placeholder: PropTypes.string,
        isDisabled: PropTypes.bool,
        onChange: PropTypes.func
    };

    static defaultProps = {
        shift: null,
        shiftOptions: [],
        name: '',
        className: '',
        placeholder: '',
        isDisabled: false,
        onChange: function () {
            // This is intentional
        }
    };

    constructor(props) {
        super(props);

        this.state = {
            value: props.shift === null ? '' : props.shift.code,
            options: [],
            isPasted: false
        };
    }

    componentDidUpdate(prevProps) {
        if (this.props.shift !== prevProps.shift) {
            this.setState({ value: this.props.shift === null ? '' : this.props.shift.code });
        }
    }

    handleChange = event => {
        const value = event.target.value;
        const isPasted = this.state.isPasted;
        const shiftOptions = this.props.shiftOptions;
        let options = [];

        if (value.trim().length > 0) {
            if (isPasted) {
                const trimvalue = value.trim().toLowerCase().replace(/ /g, '');
                const index = this.props.shiftOptions.findIndex(item => item.code.toLowerCase().replace(/ /g, '') === trimvalue);
                if (index >= 0) {
                    this.props.onChange(this.props.name, this.props.shiftOptions[index]);
                }
            } else {
                options = shiftOptions.filter(item => item.code.toLowerCase().replace(/ /g, '')
                    .startsWith(value.trim().toLowerCase().replace(/ /g, '')));
            }
        }

        this.setState({
            value,
            options,
            isPasted: false
        });
    };

    handleSelect = (value, item) => {
        this.setState({ value });

        this.props.onChange(this.props.name, item);
    };

    handleInputFocus = event => {
        event.target.select();
        this.setState({ options: [] });
    };

    handleInputPaste = () => {
        this.setState({ isPasted: true });
    };

    handleInputBlur = event => {
        if (this.props.isDisabled) {
            return;
        }

        const value = event.target.value.trim().toLowerCase().replace(/ /g, '');
        if (value.length > 0) {
            const index = this.props.shiftOptions.findIndex(item => item.code.toLowerCase().replace(/ /g, '') === value);
            if (index >= 0) {
                this.props.onChange(this.props.name, this.props.shiftOptions[index]);
            } else {
                this.setState(
                    { value: '' },
                    () => this.props.onChange(this.props.name, null)
                );
            }
        } else {
            this.props.onChange(this.props.name, null);
        }
    };

    render() {
        const { name, className, isDisabled, placeholder } = this.props;

        return (
            <Autocomplete
                items={this.state.options}
                shouldItemRender={(item, value) => item.code.toLowerCase().replace(/ /g, '').startsWith(value.trim().toLowerCase().replace(/ /g, ''))}
                getItemValue={item => item.code}
                wrapperProps={{ className: 'react-autocomplete-wrapper' }}
                wrapperStyle={{ display: 'block' }}
                renderInput={props => <input type="text" {...props} />}
                renderMenu={items => <div className="items" children={items} />}
                renderItem={(item, isHighlighted) => <div className={`item ${isHighlighted ? 'highlighted' : ''}`} key={item.id}>{item.code}</div>}
                inputProps={{
                    name: name,
                    className: className,
                    placeholder: placeholder,
                    disabled: isDisabled,
                    onFocus: this.handleInputFocus,
                    onPaste: this.handleInputPaste,
                    onBlur: this.handleInputBlur
                }}
                value={this.state.value}
                onChange={this.handleChange}
                onSelect={this.handleSelect}
            />
        );
    }
}