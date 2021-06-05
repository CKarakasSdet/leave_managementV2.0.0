using System;
using System.Collections.Generic;

namespace leave_management.Contracts
{
    public interface IRepositoryBase<T> where T : class // I can pass any type in this interface  
    {

        ICollection<T> FindAll();

        T FindById(int id);

        bool IsPresent(int id);

        bool Create(T entity);

        bool Update(T entity);

        bool Delete(T entity);

        bool Save(); 


    }
}
