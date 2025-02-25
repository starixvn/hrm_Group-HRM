namespace Fanex.NexHR.Core.Repositories
{
    using System;
    using System.Collections;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models;

    public class BaseRepository : IBaseRepository
    {
        public CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                const string ErrCode = "ErrCode";
                const string ErrMsg = "ErrMsg";

                db.SetParameterValue(ErrCode, 0);
                db.SetParameterValue(ErrMsg, string.Empty);

                db.ExecuteNonQuery(criteria);
                Hashtable hash = db.GetOutputParams();

                return GetCommandResult(hash);
            }
        }

        private CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
    }
}