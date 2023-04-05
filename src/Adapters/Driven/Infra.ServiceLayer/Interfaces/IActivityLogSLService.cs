using System;
using Domain.Entities;

namespace Infra.ServiceLayer.Interfaces
{
	public interface IActivityLogSLService
	{
        Task CreateActivityLogAsync(ActivityLog activityLog, int tryLogin = 0);
    }
}

