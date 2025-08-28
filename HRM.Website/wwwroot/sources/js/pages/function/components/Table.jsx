import React from 'react';

const Table = ({ rolePermission, onChangePermission }) => {
    const { isHost, roleId, functions, hierarchyFunctions } = rolePermission;

    const updatePermission = (permissions, functionId, target, checked) => {
        const findex = functions.findIndex(item => item.functionId === functionId);
        const index = permissions.findIndex(item => item.functionId === functionId);
        if (checked) {
            permissions[index] = {
                ...permissions[index],
                isRead: target === 'funcRead' || target === 'funcHost',
                isHost: target === 'funcHost'
            };
            return;
        }

        if (target === 'funcRead' || functions[findex].policy === 2) {
            permissions[index] = { ...permissions[index], isRead: false, isHost: false };
            return;
        }

        if (functions[findex].policy === null) {
            permissions[index] = { ...permissions[index], isRead: true, isHost: false };
            return;
        }
    };

    const updateParents = (permissions, parentId, target, checked) => {
        let isChecked = checked;
        const pindex = functions.findIndex(item => item.functionId === parentId);

        if (permissions.findIndex(item => item.functionId === parentId) === -1) {
            permissions.push({ ...functions[pindex], isRead: false, isHost: false });
        }

        if (!checked) {
            const hasTarget = (child, item) => {
                if (item.functionId !== child.functionId) {
                    return false
                };
                if (target === "funcRead") {
                    return item.isRead
                };
                if (target === "funcHost") {
                    return item.isHost
                };
                return false;
            };

            const childs = functions.filter(child =>
                permissions.some(item => hasTarget(child, item))
            );

            isChecked = childs.length > 0;
        }

        updatePermission(permissions, functions[pindex].functionId, target, isChecked);

        if (functions[pindex].parentId) {
            updateParents(permissions, functions[pindex].parentId, target, checked);
        }
    };

    const updateChilds = (permissions, functionId, target, checked) => {
        const childs = functions.filter(item => item.parentId === functionId);
        const totalItems = childs.length;
        for (let i = 0; i < totalItems; i++) {
            if (permissions.findIndex(item => item.functionId === childs[i].functionId) === - 1) {
                permissions.push({ ...childs[i], isRead: false, isHost: false });
            }

            updatePermission(permissions, childs[i].functionId, target, checked);
            updateChilds(permissions, childs[i].functionId, target, checked);
        }
    };

    const handleChangePermission = (func, event) => {
        if (!isHost) {
            return;
        }

        const { name, checked } = event.target;
        const { functionId, parentId } = func;

        const permissions = [...rolePermission.permissions];
        if (permissions.findIndex(item => item.functionId === functionId) === - 1) {
            permissions.push({ ...func, isRead: false, isHost: false });
        }

        updatePermission(permissions, functionId, name, checked);
        if (parentId) {
            updateParents(permissions, parentId, name, checked);
        }

        updateChilds(permissions, functionId, name, checked);
        onChangePermission({ isHost, roleId, functions, hierarchyFunctions, permissions });
    };

    const renderRow = func => {
        const permission = rolePermission.permissions.find(item => item.functionId === func.functionId);
        const trees = [];
        for (let i = 0; i < func.level; i++) {
            if (i === func.level - 1) {
                trees.push(<span key={i}>&#9492;&#9472;&#9472;</span>);
            } else {
                trees.push(<span key={i} className="invisible">&#9492;&#9472;&#9472;</span>);
            }
        }

        let rows = [
            <tr key={`row-${func.functionId}`} className={func.level === 0 ? 'root-row' : 'child-row'}>
                <td className="align-left">
                    {trees.map(item => item)}
                    <span> {func.functionName}</span>
                </td>
                <td className="text-center">
                    {
                        (func.policy === null || func.policy === 1) &&
                        <input type='checkbox' name="funcRead"
                            disabled={!isHost}
                            checked={permission !== undefined && permission.isRead}
                            onChange={event => handleChangePermission(func, event)}
                        />
                    }
                </td>
                <td className="text-center">
                    {
                        (func.policy === null || func.policy === 2) &&
                        <input type='checkbox' name="funcHost"
                            checked={permission !== undefined && permission.isHost}
                            disabled={!isHost}
                            onChange={event => handleChangePermission(func, event)}
                        />
                    }
                </td>
            </tr>
        ];

        rows = attachChildToRow(func, rows);

        return rows;
    };

    const attachChildToRow = (func, rows) => {

        if (!func.childs || func.childs.length <= 0) {
            return rows;
        }

        const totalChilds = func.childs.length;
        for (let i = 0; i < totalChilds; i++) {
            rows.push(...renderRow(func.childs[i]));
        }

        return rows;

    }

    const buildBody = () => {
        const totalItems = hierarchyFunctions.length;
        if (totalItems > 0) {
            const rows = [];
            for (let i = 0; i < totalItems; i++) {
                rows.push(...renderRow(hierarchyFunctions[i]));
            }

            return rows;
        }

        return [
            <tr key="body">
                <td colSpan="3">No information available</td>
            </tr>
        ];
    };

    return (
        <table className="table table-custom">
            <thead>
                <tr>
                    <th rowSpan="2">Function</th>
                    <th colSpan="2">Role Permission</th>
                </tr>
                <tr>
                    <th className="th-read">Read</th>
                    <th>Host</th>
                </tr>
            </thead>
            <tbody>
                {buildBody()}
            </tbody>
        </table>
    );
};

export default Table;