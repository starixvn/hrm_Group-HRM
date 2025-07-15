import React from "react";
import toastr from "toastr";
import { toastOptions } from "../../../../shared/common.functions";

const isValidIP = (ip) => {
    const ipv4WithCIDR = /^(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}(\/([0-9]|[12][0-9]|3[0-2]))?$/;
    const ipv6WithCIDR = /^(([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4})(\/([0-9]|[1-9][0-9]|1[01][0-9]|12[0-8]))?$/;
    return ipv4WithCIDR.test(ip) || ipv6WithCIDR.test(ip);
};

const TagInput = ({ tags, onChange }) => {
    const handleAdd = (value) => {
        if (!value) return;

        if (!isValidIP(value)) {
            toastr.warning("Invalid IP address format", "", toastOptions());
            return;
        }

        if (tags.some(tag => tag.text === value)) {
            toastr.info(`"${value}" is already in the list`, "", toastOptions());
            return;
        }

        const newTag = { id: Date.now().toString(), text: value };
        onChange([...tags, newTag]);
    };

    const handleRemove = (index) => {
        const newTags = tags.filter((_, i) => i !== index);
        onChange(newTags);
    };

    return (
        <div className="tag-input-container">
            <div className="tag-list">
                {tags.map((tag, index) => (
                    <span key={tag.id} className="tag-item">
                        {tag.text}
                        <button
                            type="button"
                            onClick={() => handleRemove(index)}
                            className="tag-remove-btn"
                            aria-label="Remove IP address"
                        >
                            <i className="fa fa-times" />
                        </button>
                    </span>
                ))}
            </div>
            <input
                type="text"
                className="tag-input-field"
                placeholder="Enter IP address and press Enter"
                onKeyPress={(e) => {
                    if (e.key === "Enter") {
                        e.preventDefault();
                        handleAdd(e.target.value.trim());
                        e.target.value = "";
                    }
                }}
            />
        </div>
    );
};

export default TagInput;
