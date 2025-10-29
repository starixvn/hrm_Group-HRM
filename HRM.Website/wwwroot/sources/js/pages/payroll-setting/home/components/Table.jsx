import React from 'react';
import Row from './Row';

const Table = ({ data, saveEditData, onDelete, types }) => {
    const colSpan = window.hasPermission === 'True' ? 7 : 5;

    const columns = [
        { label: '#', key: null },
        { label: 'Type', key: 'type' },
        { label: 'Items', key: 'items' },
        { label: 'Amount', key: 'amount' },
    ];

    if (window.hasPermission === 'True') {
        columns.push({ label: 'Edit', key: null });
        columns.push({ label: 'Delete', key: null });
    }

    return (
        <table className='table table-custom'>
            <thead>
                <tr>
                    {columns.map(({ label, key }) => 
                        <th
                            key={key}
                        >
                            <div className='cell-control-wrapper'>
                                {label}
                            </div>
                        </th>
                    )}
                </tr>
            </thead>
            <tbody>
                {data && data.length > 0 ? (
                    data.map((item, index) => (
                        <Row
                            key={item.payrollItemSettingId}
                            rowIndex={index + 1}
                            item={item}
                            saveEditData={saveEditData}
                            onDelete={onDelete}
                            types={types}
                        />
                    ))
                ) : (
                    <tr>
                        <td colSpan={colSpan}>No information available</td>
                    </tr>
                )}
            </tbody>
        </table>
    );
};

export default Table;
