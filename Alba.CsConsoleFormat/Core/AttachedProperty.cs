﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xaml;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class AttachedProperty
    {
        private const string PropertySuffix = "Property";

        private static readonly Dictionary<AttachableMemberIdentifier, AttachedProperty> _Properties =
            new Dictionary<AttachableMemberIdentifier, AttachedProperty>();

        internal AttachableMemberIdentifier Identifier { get; }
        internal object DefaultValueUntyped { get; }

        internal AttachedProperty(AttachableMemberIdentifier identifier, object defaultValue)
        {
            Identifier = identifier;
            DefaultValueUntyped = defaultValue;
        }

        public string Name => Identifier.MemberName;
        public Type OwnerType => Identifier.DeclaringType;

        internal static AttachedProperty Get(AttachableMemberIdentifier identifier) => _Properties[identifier];

        public static AttachedProperty<T> Register<TOwner, T>([NotNull] string name, T defaultValue = default(T))
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            var identifier = new AttachableMemberIdentifier(typeof(TOwner), name);
            var property = new AttachedProperty<T>(identifier, defaultValue);
            _Properties.Add(identifier, property);
            return property;
        }

        public static AttachedProperty<T> Register<TOwner, T>([NotNull] Expression<Func<AttachedProperty<T>>> nameExpression, T defaultValue = default(T))
        {
            if (nameExpression == null)
                throw new ArgumentNullException(nameof(nameExpression));
            string name = "";
            if (nameExpression.Body is MemberExpression memberExpr)
                name = memberExpr.Member.Name;
            if (name.EndsWith(PropertySuffix, StringComparison.Ordinal))
                name = name.Substring(0, name.Length - PropertySuffix.Length);
            return Register<TOwner, T>(name, defaultValue);
        }
    }

    public class AttachedProperty<T> : AttachedProperty
    {
        public T DefaultValue => (T)DefaultValueUntyped;

        internal AttachedProperty(AttachableMemberIdentifier identifier, T defaultValue) : base(identifier, defaultValue)
        {}
    }
}