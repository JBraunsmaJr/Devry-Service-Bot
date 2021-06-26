﻿using Domain.Common.Models;

namespace Domain.Events
{
    public abstract class BaseEvent<TDomainObject> : DomainEvent
    {
        public TDomainObject Result { get; }

        public BaseEvent(TDomainObject result)
        {
            Result = result;
        }
    }
}