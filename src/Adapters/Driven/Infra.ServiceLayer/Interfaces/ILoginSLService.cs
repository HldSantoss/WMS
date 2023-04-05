using System;
namespace Infra.ServiceLayer.Interfaces
{
	public interface ILoginSLService
	{
        Task<string> TokenAsync();
        Task LoginAsync();
    }
}

