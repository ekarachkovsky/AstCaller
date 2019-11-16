using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.Services
{
    public interface IScheduledProcessorService
    {
        Task ExecuteAsync();
    }
}
