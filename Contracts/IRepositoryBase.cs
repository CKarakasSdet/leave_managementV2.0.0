using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
    public interface IRepositoryBase<T> where T : class // I can pass any type in this interface  
    {

        Task<ICollection<T>> FindAll();

        Task<T> FindById(int id);

        Task<bool> IsPresent(int id);

        Task<bool> Create(T entity);

        Task<bool> Update(T entity);

        Task<bool> Delete(T entity);

        Task<bool> Save(); 


    }


    
}
