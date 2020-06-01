using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Contracts.Daos
{
    public interface IDao<TKey>
    {
        TKey Id { get; }
    }
    public class BaseDao
    {
    }
    public class BaseDao<TKey> : IDao<TKey>
    {
        public TKey Id { get; protected set; }
    }



    public class BaseAuditableDao : BaseDao
    {
        public DateTime AuditTime { get; protected set; }
        public string AuditBy { get; protected set; }
        public string EventType { get; protected set; }
    }
    public class BaseAuditableDao<TKey> : BaseAuditableDao, IDao<TKey>
    {
        public TKey Id { get; protected set; }
    }


    public class BaseTypeDao : BaseAuditableDao
    {
        public string Description { get; protected set; }
    }
    public class BaseTypeDao<TKey>: BaseTypeDao, IDao<TKey>
    {
        public TKey Id { get; protected set; }
    }
}
