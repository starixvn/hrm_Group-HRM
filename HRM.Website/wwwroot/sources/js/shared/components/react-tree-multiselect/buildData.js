const getChilds = (floatData, data, parentKey, isChecked = false) => {
    const totalItems = data.length;
    for (let i = 0; i < totalItems; i++) {
        const key = parentKey + '-' + i;

        floatData.push({ ...data[i], checked: isChecked, key: key, parentKey: parentKey });
        data[i] = { ...data[i], checked: isChecked, key: key };
        if (data[i].items && data[i].items.length > 0) {
            getChilds(floatData, data[i].items, key, isChecked);
        }
    }
};

export default (data, isChecked = false) => {
    let floatData = [];
    const totalItems = data.length;
    for (let i = 0; i < totalItems; i++) {
        const key = 'item-' + i;

        floatData.push({ ...data[i], checked: isChecked, key: key, parentKey: '' });
        data[i] = { ...data[i], checked: isChecked, key: key };
        if (data[i].items && data[i].items.length > 0) {
            getChilds(floatData, data[i].items, key, isChecked);
        }
    }

    return { data, floatData };
};