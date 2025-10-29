import React from "react";

const AddEditAllowanceItemContent = ({ editData, setEditData, types }) => {
  return (
    <div className="add-edit-allowance-item-content">
      <div className="content-line">
        <div>Type</div>
        <div>
          <select
            className="form-control"
            value={editData.payrollItemSettingId}
            onChange={e =>
              setEditData({ ...editData, payrollItemSettingId: e.target.value })
            }
            disabled={editData.allowanceItemSettingId !== null}
            style={{
              display: "inline-block",
              width: "100%",
              fontSize: "13px",
              fontWeight: "bold",
              borderRadius: "4px",
              boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
              cursor: "pointer",
              transition: "all 0.2s ease",
            }}
          >
            {types &&
              types.map(item => (
                <option key={`allowance-item-setting-${item.id}`} value={item.id}>
                  {item.name}
                </option>
              ))}
          </select>
        </div>
      </div>
      <div className="content-line">
        <div>
          Item Name
          <span style={{ color: "red", marginLeft: "5px" }}>*</span>
        </div>
        <div>
          <input
            type="text"
            value={editData.itemName}
            className="form-control"
            onChange={e =>
              setEditData({ ...editData, itemName: e.target.value })
            }
          />
        </div>
      </div>
      <div className="content-line">
        <div>
          Amount
          <span style={{ color: "red", marginLeft: "5px" }}>*</span>
        </div>
        <div>
          <input
            type="number"
            value={editData.amount}
            min="0"
            className="form-control no-spinner"
            onChange={e =>
              setEditData({ ...editData, amount: e.target.value })
            }
            style={{ paddingRight: "25px" }}
          />
        </div>
      </div>
    </div>
  );
};

export default AddEditAllowanceItemContent;
