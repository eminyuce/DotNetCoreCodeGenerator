using System;
using System.Collections.Generic;
using System.Text;

namespace EFGenericRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IEntity<TId> where TId : IComparable
    {

        /// <summary>
        /// Id of the Entity
        /// </summary>
        TId Id { get; set; }
    }
}
