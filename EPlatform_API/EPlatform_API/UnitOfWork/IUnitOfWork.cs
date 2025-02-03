using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.IRepository;
using EPlatform_API.Models;
using EPlatform_API.Repository;

namespace EPlatform_API.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ShopRepository ShopRepo{ get; }
        ProductRepository ProductRepo {get;}

        int Save();
        Task<int> SaveAsync();

        Task BeginTransaction();
        Task CommitTransaction();
        Task RollBackTransaction();

    }
}