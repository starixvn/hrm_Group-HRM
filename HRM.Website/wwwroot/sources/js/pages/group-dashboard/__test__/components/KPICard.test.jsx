import { render } from '@testing-library/react';
import React from 'react';
import KPICard from '../../components/KPICard';

describe('KPICard Component', () => {
    it('should render with label and value', () => {
        const { getByText } = render(
            <KPICard label="Test Label" value={1234} icon="users" />
        );

        expect(getByText('Test Label')).toBeInTheDocument();
        expect(getByText('1,234')).toBeInTheDocument();
    });

    it('should render with dark theme when dark prop is true', () => {
        const { container } = render(
            <KPICard label="Test" value={100} icon="users" dark={true} />
        );

        expect(container.querySelector('.kpi-dark-card')).toBeInTheDocument();
        expect(container.querySelector('.kpi-dark-value')).toBeInTheDocument();
        expect(container.querySelector('.kpi-dark-label')).toBeInTheDocument();
    });

    it('should render without icon when dark is true', () => {
        const { container } = render(
            <KPICard label="Test" value={100} icon="users" dark={true} />
        );

        expect(container.querySelector('i')).not.toBeInTheDocument();
    });

    it('should format large numbers with locale string', () => {
        const { getByText } = render(
            <KPICard label="Test" value={1234567} icon="users" />
        );

        expect(getByText('1,234,567')).toBeInTheDocument();
    });

    it('should handle missing icon gracefully', () => {
        const { container } = render(
            <KPICard label="Test" value={100} />
        );

        expect(container.querySelector('.kpi-card')).toBeInTheDocument();
    });
});

