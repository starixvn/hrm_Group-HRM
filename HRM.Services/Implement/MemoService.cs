namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Memo;
    using HRM.Models;

    public class MemoService(IBaseService baseService) : IMemoService
    {
        public async Task<IEnumerable<Memo>> GetAllAsync(int year, int category)
        {
            var criteria = new GetAllMemoCriteria
            {
                Year = year,
                Category = category
            };
            return await baseService.GetDataAsync<Memo>(criteria);
        }

        public async Task<Memo> GetByIdAsync(int id)
        {
            var criteria = new GetMemoByIdCriteria
            {
                Id = id
            };
            var memo = await baseService.GetDataEntityAsync<Memo>(criteria);
            if (memo != null && memo.MemoType == 1)
            {
                memo.AttachFile = memo.Contents;
            }

            return memo ?? new Memo();
        }

        public async Task<CommandResult> CreateAsync(Memo model)
        {
            var criteria = new CreateMemoCriteria
            {
                RefNo = model.RefNo,
                Contents = model.Contents,
                Subject = model.Subject,
                SentTo = model.SentTo,
                SentFrom = model.SentFrom,
                SentDate = model.SentDate,
                ForInfo = model.ForInfo,
                TotalPages = model.TotalPages,
                Category = model.Category,
                MemoType = model.MemoType,
                SendMail = model.SendMail
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> EditAsync(Memo model)
        {
            var criteria = new EditMemoCriteria
            {
                Id = model.Id,
                RefNo = model.RefNo,
                Contents = model.Contents,
                Subject = model.Subject,
                SentTo = model.SentTo,
                SentFrom = model.SentFrom,
                ForInfo = model.ForInfo,
                TotalPages = model.TotalPages,
                SentDate = model.SentDate,
                Category = model.Category,
                MemoType = model.MemoType
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> DeleteAsync(int id)
        {
            var criteria = new DeleteMemoCriteria { Id = id };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}