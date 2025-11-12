import React from 'react';

const KPICard = ({ label, value, icon, dark }) => {
    const getIconClass = () => {
        const iconMap = {
            'users': 'fa fa-users',
            'user-plus': 'fa fa-user-plus',
            'user-times': 'fa fa-user-times',
            'clock-o': 'fa fa-clock-o',
            'building': 'fa fa-building',
            'briefcase': 'fa fa-briefcase',
            'birthday-cake': 'fa fa-birthday-cake',
            'globe': 'fa fa-globe'
        };
        return iconMap[icon] || null;
    };

    const cardClass = dark ? 'kpi-dark-card' : 'kpi-card';
    const valueClass = dark ? 'kpi-dark-value' : 'kpi-value';
    const labelClass = dark ? 'kpi-dark-label' : 'kpi-label';

    return (
        <div className={cardClass}>
            {icon && !dark && <i className={getIconClass()}></i>}
            <div className={dark ? '' : 'kpi-content'}>
                <div className={labelClass}>{label}</div>
                <div className={valueClass}>{value.toLocaleString()}</div>
            </div>
        </div>
    );
};

export default KPICard;

