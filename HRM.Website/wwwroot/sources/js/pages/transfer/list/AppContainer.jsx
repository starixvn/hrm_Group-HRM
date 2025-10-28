import React, { useState, useEffect, Fragment } from "react";
import axios from "axios";
import { resolveClientUrl, axiosError } from "../../../shared/common.functions";
import Table from "./components/Table";

const AppContainer = () => {
  const [data, setData] = useState([]);

  const getTransferList = () => {
    axios
      .get(resolveClientUrl("EmployeeTransfer/GetEmployeeTransferList"), {
        headers: { "X-Requested-With": "XMLHttpRequest" },
      })
      .then((response) => {
        setData(response.data);
      })
      .catch(axiosError);
  };

  useEffect(() => {
    getTransferList();
  }, []);
  return (
    <Fragment>
      {window.hasPermission === "True" && <Header />}
      <Table data={data} />
    </Fragment>
  );
};

const Header = () => {
  const handleCreate = () => {
    window.location.href = resolveClientUrl("EmployeeTransfer/Transfer");
  };
  return (
    <form class="filter">
      <div class="form-horizontal clearfix">
        <button type="button" class="btn btn-request" onClick={handleCreate}>
          Transfer Employee
        </button>
      </div>
    </form>
  );
};

export default AppContainer;
