﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TinyCsvParser.Reflection
{
    public static class ReflectionUtils
    {
        public static PropertyInfo GetProperty<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            var member = GetMemberExpression(expression).Member;
            var property = member as PropertyInfo;
            if (property == null)
            {
                throw new InvalidOperationException($"Member with Name '{member.Name}' is not a property.");
            }
            return property;
        }

        private static MemberExpression GetMemberExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", "expression");
            }

            return memberExpression;
        }

        public static bool IsEnum(Type type)
        {
#if NETSTANDARD1_3
            return typeof(Enum).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
            
#else 
            return typeof(Enum).IsAssignableFrom(type);
#endif
        }

        public static Action<TEntity, TProperty> CreateSetter<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            PropertyInfo propertyInfo = ReflectionUtils.GetProperty(property);

            ParameterExpression instance = Expression.Parameter(typeof(TEntity), "instance");
            ParameterExpression parameter = Expression.Parameter(typeof(TProperty), "param");

#if NETSTANDARD1_3
            return Expression.Lambda<Action<TEntity, TProperty>>(
                Expression.Call(instance, propertyInfo.SetMethod, parameter),
                new ParameterExpression[] { instance, parameter }).Compile();
#else
            return Expression.Lambda<Action<TEntity, TProperty>>(
                Expression.Call(instance, propertyInfo.GetSetMethod(), parameter),
                new ParameterExpression[] { instance, parameter }).Compile();
#endif
        }

        public static string GetPropertyNameFromExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            var member = GetMemberExpression(expression).Member;

            return member.Name;
        }
    }
}
