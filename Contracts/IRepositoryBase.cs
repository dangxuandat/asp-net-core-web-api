using System;
using System.Linq;
using System.Linq.Expressions;
using Entities;

namespace Contracts
{
    //Repository Pattern
    public interface IRepositoryBase<T>
    {

        IQueryable<T> FindAll(bool trackChanges);

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
            bool trackChanges);

        void Create(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}