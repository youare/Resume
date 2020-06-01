using Domain.SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Contracts.Daos
{
    public class ContentType:BaseTypeDao<string>
    {
        public ContentType(string id, string description, DateTime auditTime, string auditBy, string eventType)
        {
            Id = id;
            Description = description;
            AuditTime = auditTime;
            AuditBy = auditBy;
            EventType = eventType;
        }
        private ContentType() { }
        public ContentTypeValueObject ToValueObject()
        {
            return new ContentTypeValueObject(Id, Description, AuditTime, AuditBy, EventType);
        }
        public static ContentType MapFrom(ContentTypeValueObject item)
        {
            return new ContentType(item.Id, item.Description, item.AuditTime, item.AuditBy, item.EventType);
        }
    }
}
