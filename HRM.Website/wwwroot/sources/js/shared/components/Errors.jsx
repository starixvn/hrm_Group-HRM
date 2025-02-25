import React from 'react';

const Errors = ({ errors }) => {
    if (!errors || Object.keys(errors).length === 0) {
        return null;
    }

    return (
        <div className="validation-summary-errors">
            <ul>
                {Object.values(errors).map((error, index) => (
                    <li key={index}>{error}</li>
                ))}
            </ul>
        </div>
    );
};

export default Errors;