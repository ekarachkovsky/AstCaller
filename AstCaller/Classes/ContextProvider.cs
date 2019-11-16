using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AstCaller.Classes
{
    public class ContextProvider : IContextProvider
    {
        private IServiceProvider _serviceProvider;

        public ContextProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MainContext GetContext()
        {
            return new MainContext(_serviceProvider.GetService(typeof(DbContextOptions<MainContext>)) as DbContextOptions<MainContext>);
        }
    }
}
