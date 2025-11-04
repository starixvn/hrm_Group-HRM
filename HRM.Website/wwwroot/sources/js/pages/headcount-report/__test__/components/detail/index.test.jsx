import ReactDOM from 'react-dom';

jest.mock('react-dom', () => ({
    render: jest.fn(),
}));

jest.mock('../../../components/detail/DetailContainer', () => {
    return function DetailContainer() {
        return null;
    };
});

Object.defineProperty(document, 'getElementById', {
    value: jest.fn(() => ({ tagName: 'DIV' })),
    writable: true,
});

describe('Detail Index Entry Point', () => {
    it('should render DetailContainer to DOM element', () => {
        require('../../../components/detail/index.jsx');

        expect(ReactDOM.render).toHaveBeenCalledTimes(1);
        expect(document.getElementById).toHaveBeenCalledWith(
            'headcount-report-detail'
        );

        const renderCall = ReactDOM.render.mock.calls[0];
        expect(renderCall[0]).toBeDefined();
        expect(renderCall[1]).toEqual({ tagName: 'DIV' });
    });
});

