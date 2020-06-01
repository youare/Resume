using Domain.SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Contracts.Daos
{
    public class SectionType : BaseTypeDao<string>
    {
        public SectionType(string id, string description, DateTime auditTime, string auditBy, string eventType)
        {
            Id = id;
            Description = description;
            AuditTime = auditTime;
            AuditBy = auditBy;
            EventType = eventType;
        }
        private SectionType() { }
        public SectionTypeValueObject ToValueObject()
        {
            return new SectionTypeValueObject(Id, Description, AuditTime, AuditBy, EventType);
        }
        public static SectionType MapFrom(SectionTypeValueObject item)
        {
            return new SectionType(item.Id, item.Description, item.AuditTime, item.AuditBy, item.EventType);
        }
    }
}
