import React, { useState } from 'react';

const CurrencyRoundingTable = ({ data, onEdit }) => {
    const hasPermission = window.hasPermission === 'True';
    const [editingIndex, setEditingIndex] = useState(null);
    const [editingValue, setEditingValue] = useState('');

    const filteredData = data || [];
    const colSpan = 2;

    const handleEditStart = (item, index) => {
        if (!hasPermission) {
            return;
        }
        setEditingIndex(index);
        setEditingValue(item.rounding !== undefined ? item.rounding.toString() : '');
    };

    const handleEditCancel = () => {
        setEditingIndex(null);
        setEditingValue('');
    };

    const handleEditSave = (item, _index) => {
        if (onEdit) {
            const updatedItem = {
                ...item,
                rounding: editingValue === '' ? undefined : parseInt(editingValue, 10)
            };
            onEdit(updatedItem);
        }
        setEditingIndex(null);
        setEditingValue('');
    };

    const handleKeyDown = (e, item, index) => {
        if (e.key === 'Enter') {
            handleEditSave(item, index);
        } else if (e.key === 'Escape') {
            handleEditCancel();
        }
    };

    const renderRow = (item, index) => {
        const rounding = item.rounding !== undefined ? item.rounding : '';
        const isEditing = editingIndex === index;
        const originalValue = item.rounding !== undefined ? item.rounding.toString() : '';
        const currentValue = editingValue.trim();
        const hasChanged = currentValue !== originalValue;
        const isValidValue = currentValue === '' ? originalValue === '' : !isNaN(parseInt(currentValue, 10));
        const isSaveDisabled = !hasChanged || !isValidValue;

        return (
            <tr key={index}>
                <td className="currency-cell"><strong>{item.currency}</strong></td>
                <td
                    className={`rounding-cell ${hasPermission ? 'editable-cell' : ''}`}
                    onClick={() => hasPermission && !isEditing && handleEditStart(item, index)}
                >
                    {isEditing ? (
                        <div className="rounding-edit-container">
                            <input
                                type="number"
                                className="rounding-input"
                                value={editingValue}
                                onChange={e => setEditingValue(e.target.value)}
                                onKeyDown={e => handleKeyDown(e, item, index)}
                                autoFocus
                            />
                            <button
                                className="btn btn-sm btn-primary rounding-save-btn"
                                onClick={() => handleEditSave(item, index)}
                                disabled={isSaveDisabled}
                                title={isSaveDisabled ? "No changes to save" : "Save"}
                            >
                                <i className="fa fa-floppy-o"></i>
                            </button>
                        </div>
                    ) : (
                        <span className="rounding-value">{rounding}</span>
                    )}
                </td>
            </tr>
        );
    };

    return (
        <div className="currency-rounding-table-section">
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th className="currency-header">Currency</th>
                        <th className="rounding-header">Rounding</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredData.length > 0 ? (
                        filteredData.map(renderRow)
                    ) : (
                        <tr>
                            <td colSpan={colSpan} className="empty-state-cell">
                                No data available
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default CurrencyRoundingTable;

