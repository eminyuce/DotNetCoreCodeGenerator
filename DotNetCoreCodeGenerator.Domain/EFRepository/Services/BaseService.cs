using DotNetCoreCodeGenerator.Domain.EFRepository.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DotNetCoreCodeGenerator.Domain.EFRepository.Services.IServices
{
    public abstract class BaseService<T> : IBaseService<T> where T : class, IEntity<int>
    {
        private IBaseRepository<T> baseRepository { get; set; }

        public BaseService(IBaseRepository<T> baseRepository)
        {
            this.baseRepository = baseRepository;

        }

        public virtual List<T> LoadEntites(Expression<Func<T, bool>> whereLambda)
        {
            return baseRepository.FindBy(whereLambda).ToList();
        }

        public virtual List<T> GetAll()
        {
            return baseRepository.GetAll().ToList();
        }

        public virtual T GetSingle(int id)
        {
            return baseRepository.GetSingle(id);
        }

        public virtual T SaveOrEditEntity(T entity)
        {
            var tmp = baseRepository.SaveOrEdit(entity);
            return entity;
        }


        public virtual bool DeleteEntity(T entity)
        {
            var result = this.baseRepository.DeleteItem(entity);
            return result == 1;
        }
      

    }
}