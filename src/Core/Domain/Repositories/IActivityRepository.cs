using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IActivityRepository
    {
        Task SaveActivity(Activity activity);
        Task<IEnumerable<DataProdutivity>> GetActivityAsync(string userName, string docEntry);
        Task<IEnumerable<DataProdutivity>> GetAllActivities(int bplId, DateTime date, string action); 
    }
}
