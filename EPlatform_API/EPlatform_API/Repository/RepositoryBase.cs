using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Services;
using Microsoft.EntityFrameworkCore;

namespace EPlatform_API.Repository
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        protected IBlobServices _blobServices;
        private AppDbContext context;

        public RepositoryBase(
            AppDbContext context,
            IConfiguration configuration
        )
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual TEntity? Add(TEntity e)
        {
            if (e == null){
                return null;
            }

            try{
                _dbSet.Add(e);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
            return e;
        }

        public virtual async Task<TEntity?> AddAsync(TEntity e)
        {
            if (e == null){
                return null;
            }
            try{
                await _dbSet.AddAsync(e);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
            return e;
        }

        public virtual IEnumerable<TEntity>? AddRange(IEnumerable<TEntity> es)
        {
            if (es == null){
                return null;
            }
            try{
                _dbSet.AddRange(es);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
            return es;
        }

        public virtual async Task<IEnumerable<TEntity>?> AddRangeAsync(IEnumerable<TEntity> es)
        {
            if (es == null){
                return null;
            }
            try{
                await _dbSet.AddRangeAsync(es);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
            return es;
        }

        public virtual bool Delete(TEntity e)
        {
            if (e == null){
                return false;
            }
            try{
                _dbSet.Remove(e);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public virtual bool DeleteRange(IEnumerable<TEntity> es)
        {
            if (es == null){
                return false;
            }
            try{
                _dbSet.RemoveRange(es);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public virtual TEntity? Find(Func<TEntity, bool> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public virtual async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual IEnumerable<TEntity>? GetAll(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string[]? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null){
                query = query.Where(filter);
            }

            if (includeProperties != null){
                foreach (var prop in includeProperties){
                    query.Include(prop);
                }
            }

            if (orderBy != null){
                query = orderBy(query);
            }

            return query.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>?> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string[]? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null){
                query = query.Where(filter);
            }

            if (includeProperties != null){
                foreach (var prop in includeProperties){
                    query.Include(prop);
                }
            }

            if (orderBy != null){
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public virtual IQueryable<TEntity> GetAllDataSet()
        {
            return _dbSet.AsQueryable<TEntity>();
        }

        public virtual TEntity? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual bool Update(TEntity e)
        {
            if (e == null){
                return false;
            }
            try{
                _dbSet.Update(e);
            } catch (Exception ex){
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
    }
}