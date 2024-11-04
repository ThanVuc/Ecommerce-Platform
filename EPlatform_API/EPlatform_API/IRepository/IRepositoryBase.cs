using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EPlatform_API.IRepository
{
    public interface IRepositoryBase<TEnitity> where TEnitity : class
    {
        TEnitity? GetById(int id);
        Task<TEnitity?> GetByIdAsync(int id);
        IQueryable<TEnitity> GetAllDataSet();
        IEnumerable<TEnitity>? GetAll(
            Expression<Func<TEnitity,bool>>? filter,
            Func<IQueryable<TEnitity>,IOrderedQueryable<TEnitity>>? orderBy,
            string[]? includeProperties
        );
        Task<IEnumerable<TEnitity>?> GetAllAsync(
            Expression<Func<TEnitity,bool>>? filter,
            Func<IQueryable<TEnitity>,IOrderedQueryable<TEnitity>>? orderBy,
            string[]? includeProperties
        );
        TEnitity? Find(Func<TEnitity,bool> predicate);
        Task<TEnitity?> FindAsync(Expression<Func<TEnitity,bool>> predicate);

        TEnitity? Add(TEnitity e);
        Task<TEnitity?> AddAsync(TEnitity e);
        IEnumerable<TEnitity>? AddRange(IEnumerable<TEnitity> es);
        Task<IEnumerable<TEnitity>?> AddRangeAsync(IEnumerable<TEnitity> es);
        bool Update(TEnitity e);
        bool Delete(TEnitity e);
        bool DeleteRange(IEnumerable<TEnitity> es);
    }
}