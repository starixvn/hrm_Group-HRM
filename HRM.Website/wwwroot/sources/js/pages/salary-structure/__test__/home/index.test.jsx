import ReactDOM from 'react-dom';

jest.mock('react-dom', () => ({
    render: jest.fn(),
}));

jest.mock('../../home/components/AppContainer', () => {
    return function AppContainer() {
        return null;
    };
});

Object.defineProperty(document, 'getElementById', {
    value: jest.fn(() => ({ tagName: 'DIV' })),
    writable: true,
});

describe('Index Entry Point', () => {
    it('should render AppContainer to DOM element', () => {
        require('../../home/index.jsx');

        expect(ReactDOM.render).toHaveBeenCalledTimes(1);
        expect(document.getElementById).toHaveBeenCalledWith(
            'salary-structure'
        );

        const renderCall = ReactDOM.render.mock.calls[0];
        expect(renderCall[0]).toBeDefined();
        expect(renderCall[1]).toEqual({ tagName: 'DIV' });
    });
});
