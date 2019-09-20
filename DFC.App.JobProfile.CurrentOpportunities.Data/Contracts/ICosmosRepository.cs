using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICosmosRepository<T>
        where T : IDataModel
    {
        Task<T> GetAsync(Expression<Func<T, bool>> where);

        Task<IEnumerable<T>> GetAllAsync();
    }
}
