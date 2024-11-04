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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(
            AppDbContext context
        )
        {
            _context = context;
            UserRepo = new RepositoryBase<Users>(_context);
            RoleRepo = new RepositoryBase<Roles>(_context);
            GroupRepo = new RepositoryBase<Group>(_context);
        }

        private bool disposed = false;

        public IRepositoryBase<Users> UserRepo {get;}

        public IRepositoryBase<Roles> RoleRepo {get;}
        public IRepositoryBase<Group> GroupRepo {get;}

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}