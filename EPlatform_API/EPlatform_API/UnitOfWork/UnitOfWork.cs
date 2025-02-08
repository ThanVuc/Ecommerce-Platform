using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using EPlatform_API.Repository;
using EPlatform_API.Services;

namespace EPlatform_API.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly AppDbContext _context;
        private ShopRepository _shopRepo;
        private ProductRepository _productRepo;
        private readonly ILoggingService _loggingService;
        private readonly IConfiguration _configuration;
        public ShopRepository ShopRepo {
            get{
                if (_shopRepo == null){
                    _shopRepo = new ShopRepository(_context, _configuration, _loggingService);
                }

                return _shopRepo;
            }
        }

        public ProductRepository ProductRepo{
            get{
                if (_productRepo == null){
                    _productRepo = new ProductRepository(_context,_configuration,_loggingService);
                }
                return _productRepo;
            }
        }

        public UnitOfWork(
            AppDbContext context,
            IConfiguration configuration,
            ILoggingService loggingService
        )
        {
            _context = context;
            _configuration = configuration;
            _loggingService = loggingService;
        }

        private bool disposed = false;

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

        public async Task BeginTransaction()
        {
            // if (_context.Database.cu)
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollBackTransaction()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}