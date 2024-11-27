using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.IRepository;
using EPlatform_API.Models;

namespace EPlatform_API.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int Save();
        Task<int> SaveAsync();
    }
}