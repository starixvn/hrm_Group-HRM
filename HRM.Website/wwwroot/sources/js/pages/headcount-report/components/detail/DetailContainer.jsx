import React, { useEffect, useState } from 'react';
import { blockUI, resolveClientUrl, unblockUI } from '../../../../shared/common.functions';
import axios from 'axios';
import moment from 'moment';
import { orderBy } from 'lodash';

const DetailContainer = () => {
    const [data, setData] = useState([]);
    const [originalData, setOriginalData] = useState([]);
    const [sortColumn, setSortColumn] = useState(null);
    const [sortDirection, setSortDirection] = useState('asc');

    useEffect(() => {
        const urlParams = new URLSearchParams(window.location.search);
        const empIds = urlParams.get('empIds') || '';

        if (empIds) {
            fetchDetailData(empIds);
        }
    }, []);

    const fetchDetailData = async empIds => {
        blockUI();
        const response = await axios.get(
            resolveClientUrl(`HeadcountReport/GetDetail?empIds=${encodeURIComponent(empIds)}`),
        );
        const fetchedData = response.data || [];
        setOriginalData(fetchedData);
        setData(fetchedData);

        unblockUI();
    };

    const formatDate = date => {
        if (!date) {
            return '';
        }

        const jsDate = new Date(date);
        return moment(jsDate).format('DD MMM, YYYY');
    };

    const handleSort = column => {
        const newDirection = sortColumn === column && sortDirection === 'asc' ? 'desc' : 'asc';
        setSortColumn(column);
        setSortDirection(newDirection);
        const sortedData = orderBy(originalData, [item => {
                const value = item[column];
                return value ? String(value).toLowerCase() : '';
            }],
            [newDirection]
        );
        setData(sortedData);
    };

    const getSortIcon = column => {
        if (sortColumn !== column) {
            return 'fa fa-sort';
        }
        return sortDirection === 'asc'  
            ? 'fa fa-sort-asc'
            : 'fa fa-sort-desc';   
    }

    const columns = [
        { key: null, label: '#', sortable: false },
        { key: 'ctrIdFormatted', label: 'CtrId', sortable: true },
        { key: 'empId', label: 'EmpId', sortable: true },
        { key: 'code', label: 'Code', sortable: true },
        { key: 'nickname', label: 'Nickname', sortable: true },
        { key: 'bu', label: 'BU', sortable: true },
        { key: 'teamName', label: 'TeamName', sortable: true },
        { key: 'departmentName', label: 'DepartmentName', sortable: true },
        { key: 'location', label: 'Location', sortable: true },
        { key: 'joinedDate', label: 'JoinedDate', sortable: true },
        { key: 'resignedDate', label: 'ResignedDate', sortable: true },
        { key: 'confirmedDate', label: 'ConfirmedDate', sortable: true },
        { key: 'isLocal', label: 'IsLocal', sortable: true },
        { key: 'isSpecialUser', label: 'IsSpecialUser', sortable: true },
        { key: 'companyDate', label: 'CompanyDate', sortable: true },
        { key: 'company', label: 'Company', sortable: true },
        { key: 'subCompany', label: 'SubCompany', sortable: true },
    ];

    const getYesNo = val => val ? 'Yes' : 'No';

    return (
        <div>
            <h2 className="page-title">Headcount Report Detail</h2>
            <div className="table-data">
                <table className="table table-custom" id="data">
                    <thead>
                        <tr>
                            {columns.map((col, idx) => (
                                <th 
                                    key={idx}
                                    onClick={() => col.sortable && handleSort(col.key)}
                                    className={col.sortable ? 'col-sorter' : ''}
                                >
                                    <div className="cell-control-wrapper">
                                        {col.label} {col.sortable && (
                                                <div className={getSortIcon(col.key)}></div>
                                            )}
                                    </div>
                                </th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {data && data.length > 0 && (
                            data.map((item, index) => (
                                <tr key={index} className={item.isDeleted || item.enabled === false ? 'deleted-user-row' : ''}>
                                    <td>{index + 1}</td>
                                    <td>{item.ctrIdFormatted}</td>
                                    <td>{item.empId}</td>
                                    <td>{item.code}</td>
                                    <td>{item.nickname}</td>
                                    <td>{item.bu}</td>
                                    <td>{item.teamName}</td>
                                    <td>{item.departmentName}</td>
                                    <td>{item.location}</td>
                                    <td>{formatDate(item.joinedDate)}</td>
                                    <td>{formatDate(item.resignedDate)}</td>
                                    <td>{formatDate(item.confirmedDate)}</td>
                                    <td>{getYesNo(item.isLocal)}</td>
                                    <td>{getYesNo(item.isSpecialUser)}</td>
                                    <td>{formatDate(item.companyDate)}</td>
                                    <td>{item.company}</td>
                                    <td>{item.subCompany}</td>                                    
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default DetailContainer;

