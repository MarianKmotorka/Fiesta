using System;
using System.Collections.Generic;
using MediatR;

namespace Fiesta.Domain.Common
{
    public abstract class Entity<TId> : BaseEntity, IEquatable<Entity<TId>>
        where TId : IEquatable<TId>
    {

        protected Entity()
        {
        }

        protected Entity(TId id)
        {
            Id = id;
        }

        public virtual TId Id { get; protected set; }

        public override bool Equals(object obj)
        {
            return obj is Entity<TId> other && Equals(other);
        }

        public override int GetHashCode()
        {
            if (Id is null || EqualityComparer<TId>.Default.Equals(Id, default))
                return base.GetHashCode();
            else
                return Id.GetHashCode();
        }

        public bool Equals(Entity<TId> other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id != null && !EqualityComparer<TId>.Default.Equals(Id, default) && EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            if (Equals(left, null))
                return (Equals(right, null));
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !(left == right);
        }
    }

    public abstract class BaseEntity
    {
        private List<INotification> _notifications;

        protected void AddDomainEvent(INotification notification)
        {
            if (_notifications == null)
                _notifications = new List<INotification>();

            _notifications.Add(notification);
        }

        public IReadOnlyList<INotification> ConsumeEvents()
        {
            var result = (IReadOnlyList<INotification>)_notifications ?? Array.Empty<INotification>();
            _notifications = null;
            return result;
        }
    }
}
