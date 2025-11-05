import React, { useEffect, useState } from 'react';
import Filter from './Filter';
import HeadcountTableByUnit from './HeadcountTableByUnit';
import HeadcountTableByCompany from './HeadcountTableByCompany';
import HeadcountTableByDetails from './HeadcountTableByDetails';
import HeadcountTableSummary from './HeadcountTableSummary';
import { blockUI, resolveClientUrl, unblockUI } from '../../../shared/common.functions';
import axios from 'axios';

const AppContainer = () => {
    const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());
    const [activeTab, setActiveTab] = useState(0);
    const [reportData, setReportData] = useState({
        reportByUnit: [],
        reportByCompany: [],
        reportByDetails: [],
        reportByUnitCountry: [],
        reportByUnitCountryLocal: [],
        reportByUnitCountryExpat: []
    });

    const tabs = [
        { id: 0, label: 'Headcount by BU', data: reportData.reportByUnit },
        { id: 1, label: 'Headcount by Company', data: reportData.reportByCompany },
        { id: 2, label: 'Headcount Details', data: reportData.reportByDetails },
        { id: 3, label: 'Headcount by BU by Country', data: reportData.reportByUnitCountry },
        { id: 4, label: 'Headcount by BU by Country - Local', data: reportData.reportByUnitCountryLocal },
        { id: 5, label: 'Headcount by BU by Country - Expat', data: reportData.reportByUnitCountryExpat }
    ];

    const fetchReportData = async year => {
        blockUI();
        const response = await axios.get(
            resolveClientUrl(`HeadcountReport/GetData?year=${year}`),
        );
        const data = await response.data;
        unblockUI();
        setReportData({
            reportByUnit: data.reportByUnit || [],
            reportByCompany: data.reportByCompany || [],
            reportByDetails: data.reportByDetails || [],
            reportByUnitCountry: data.reportByUnitCountry || [],
            reportByUnitCountryLocal: data.reportByUnitCountryLocal || [],
            reportByUnitCountryExpat: data.reportByUnitCountryExpat || []
        });
    };

    useEffect(() => {
        fetchReportData(selectedYear);
    }, []);

    const handleYearChange = year => {
        setSelectedYear(year);
    };

    const handleSearch = () => {
        fetchReportData(selectedYear);
    };

    return (
        <div>
            <Filter 
                selectedYear={selectedYear} 
                onYearChange={handleYearChange}
                onSearch={handleSearch}
            />
            
            <ul className="nav nav-tabs">
                {tabs.map(tab => (
                    <li
                        key={tab.id}
                        className={activeTab === tab.id ? 'active' : ''}
                        onClick={() => setActiveTab(tab.id)}
                    >
                        <span>{tab.label}</span>
                    </li>
                ))}
            </ul>

            {activeTab === 0 && <HeadcountTableByUnit data={tabs[0].data} />}
            {activeTab === 1 && <HeadcountTableByCompany data={tabs[1].data} />}
            {activeTab === 2 && <HeadcountTableByDetails data={tabs[2].data} />}
            {activeTab >= 3 && activeTab <= 5 && (
                <HeadcountTableSummary data={tabs[activeTab].data} />
            )}
        </div>
    );
};

export default AppContainer;