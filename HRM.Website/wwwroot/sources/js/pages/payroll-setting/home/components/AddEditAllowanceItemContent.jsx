import React from "react";

const AddEditAllowanceItemContent = ({ editData, setEditData, types }) => {
  if (editData.allowanceItemSettingId !== null) {
    if (types == null) {
      types = [];
      types.push({ id: editData.payrollItemSettingId, name: editData.type });
    }
  }
  return (
    <div className="add-edit-allowance-item-content">
      <div className="content-line">
        <div>Type</div>
        <div>
          <select
            className="form-control"
            value={editData.payrollItemSettingId}
            onChange={(e) =>
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
              types.map((item) => (
                <option key={item.id} value={item.id}>
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
            onChange={(e) =>
              setEditData({ ...editData, itemName: e.target.value })
            }
          />
        </div>
      </div>
      <div className="content-line">
        <div>Is Percent</div>
        <div>
          <input
            type="checkbox"
            checked={editData.isPercent}
            onChange={(e) =>
              setEditData({ ...editData, isPercent: e.target.checked })
            }
          />
        </div>
      </div>
      <div className="content-line">
        <div>
          {editData.isPercent ? "Percent" : "Amount"}
          <span style={{ color: "red", marginLeft: "5px" }}>*</span>
        </div>
        <div style={{ position: "relative" }}>
          <input
            type="number"
            value={editData.amount}
            min="0"
            className="form-control no-spinner"
            onChange={(e) =>
              setEditData({ ...editData, amount: e.target.value })
            }
            style={{ paddingRight: "25px" }}
          />
          {editData.isPercent && (
            <span
              style={{
                position: "absolute",
                right: "10px",
                top: "50%",
                transform: "translateY(-50%)",
                color: "#888",
              }}
            >
              %
            </span>
          )}
        </div>
      </div>
      <div className="content-line">
        <div>Is Active</div>
        <div>
          <input
            type="checkbox"
            checked={editData.isActive}
            onChange={(e) =>
              setEditData({ ...editData, isActive: e.target.checked })
            }
          />
        </div>
      </div>
    </div>
  );
};

export default AddEditAllowanceItemContent;
