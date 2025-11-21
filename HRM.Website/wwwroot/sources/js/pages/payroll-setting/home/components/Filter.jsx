import React, { useState } from "react";
import SweetModalDialog from "../../../../shared/components/SweetModalDialog";
import AddEditAllowanceItemContent from "./AddEditAllowanceItemContent";

const Filter = ({ saveEditData, types, currencies }) => {
  const initialData = {
    payrollItemSettingId: null,
    allowanceItemSettingId: null,
    type: "",
    itemName: "",
    amount: "",
    currencyId: null,
    isPercent: false,
    isActive: true,
  };

  const [open, setOpen] = useState(false);
  const [data, setData] = useState(initialData);
  const addAllowanceItem = async () => {
    const result = await saveEditData(data);
    if (result) {
      setOpen(false);
    }
  };

  const handleOpen = () => {
    const defaultId = types && types.length > 0 ? types[0].id : null;
    const defaultCurrencyId = currencies && currencies.length > 0 ? currencies[0].id : null;
    setData({
      ...initialData,
      payrollItemSettingId: defaultId,
      currencyId: defaultCurrencyId,
    });
    setOpen(true);
  };

  return (
    <div className="filter">
      <div className="form-horizontal clearfix">
        <div className="form-group form-group--m" style={{ width: "unset" }}>
          <button
            type="button"
            className="btn btn-request"
            onClick={() => handleOpen()}
          >
            <span className="fa fa-plus" style={{ marginRight: "5px" }} />
            New Allowance Item
          </button>
        </div>
      </div>

      <SweetModalDialog
        show={open}
        contents={
          <AddEditAllowanceItemContent
            editData={data}
            setEditData={setData}
            types={types}
            currencies={currencies}
          />
        }
        onSubmit={() => addAllowanceItem()}
        onHide={() => setOpen(false)}
        title="Add Allowance Item"
        submitText="Save"
      />
    </div>
  );
};

export default Filter;
