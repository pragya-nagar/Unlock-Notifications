using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using OKRNotification.EF;

namespace OKRNotification.Service.Contracts
{
    public interface IServicesAggregator
    {
        IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        IOperationStatus OperationStatus { get; set; }
        IConfiguration Configuration { get; set; }
        IHostingEnvironment HostingEnvironment { get; set; }

        IMapper Mapper { get; set; }
    }
}
