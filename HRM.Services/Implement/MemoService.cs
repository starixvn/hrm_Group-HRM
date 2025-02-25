namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Memo;
    using HRM.Models;

    public class MemoService : IMemoService
    {
        private readonly IBaseService baseService;

        public MemoService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<Memo> GetAll(int year, int category)
        {
            var criteria = new GetAllMemoCriteria
            {
                Year = year,
                Category = category
            };
            return baseService.GetData<Memo>(criteria).ToList();
        }

        public Memo GetById(int id)
        {
            var criteria = new GetMemoByIdCriteria
            {
                Id = id
            };
            var memo = baseService.GetDataEntity<Memo>(criteria);
            if (memo != null && memo.MemoType == 1)
            {
                memo.AttachFile = memo.Contents;
            }

            return memo ?? new Memo();
        }

        public CommandResult Create(Memo model)
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
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Edit(Memo model)
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
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteMemoCriteria { Id = id };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}