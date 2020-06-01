using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.SharedKernel.ValueObjects
{
    public class ContentTypeValueObject: ValueObject
    {
        public string Id { get; }
        public string Description { get; }
        public DateTime AuditTime { get; }
        public string AuditBy { get; }
        public string EventType { get; }

        public ContentTypeValueObject(string id, string description, DateTime auditTime, string auditBy, string eventType)
        {
            Id = id;
            Description = description;
            AuditTime = auditTime;
            AuditBy = auditBy;
            EventType = eventType;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return Description;
            yield return AuditTime;
            yield return AuditBy;
            yield return EventType;
        }
    }
}
