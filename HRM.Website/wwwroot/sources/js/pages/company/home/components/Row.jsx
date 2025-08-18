import React from 'react';
import { resolveClientUrl } from '../../../../shared/common.functions';

const Row = ({ rowIndex, item }) => {
    const handleEdit = () => {
        window.location = resolveClientUrl(`company/edit?Id=${item.companyId}`);
    };

    const isBooleanString = (val) => val && ['true', 'false'].includes(val.toLowerCase());

    const isTrue = (val) => val && val.toLowerCase() === 'true';

    return (
        <tr id={`row_${item.companyId}`}>
            <td>{rowIndex}</td>
            <td className='text-left'>{item.name}</td>
            <td>
                {isBooleanString(item.hasBIO) && (
                    <input
                        type='checkbox'
                        className='readonly-checkbox'
                        name={`hasBIO-${item.companyId}`}
                        checked={isTrue(item.hasBIO)}
                        readOnly
                    />
                )}
            </td>
            <td>
                {isBooleanString(item.enableLetstalk) && (
                    <input
                        type='checkbox'
                        className='readonly-checkbox'
                        checked={isTrue(item.enableLetstalk)}
                        readOnly
                    />
                )}
            </td>
            <td>{item.currency}</td>
            <td>
                {item.whitelistIPList.length > 0 && (
                    <div className='whitelist-ip-list'>
                        {item.whitelistIPList.map((ip, idx) => (
                            <span className='text-left whitelist-item' key={idx}>{ip}</span>
                        ))}
                    </div>
                )}
            </td>
            <td>
                {item.urlList.length > 0 && (
                    <div className='whitelist-ip-list'>
                        {item.urlList.map((url, idx) => (
                            <span className='text-left url-item' key={idx}><a href={url} target='_blank' rel='noopener noreferrer'>{url}</a></span>
                        ))}
                    </div>
                )}
            </td>
            <td>
                {item.subCompanyList && item.subCompanyList.length > 0 ? (
                    item.subCompanyList.map(subCompany => subCompany.name).join(', ')
                ) : ''}
            </td>
            <td>
                <a
                    onClick={handleEdit}
                    className='fa fa-pencil fa-lg icon-edit'
                />
            </td>
        </tr>
    );
};

export default Row;