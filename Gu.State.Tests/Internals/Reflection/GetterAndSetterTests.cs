﻿namespace Gu.State.Tests.Internals.Reflection
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using NUnit.Framework;

    using static TypeExtTypes;

    public class GetterAndSetterTests
    {
        [Test]
        public void CreateFromPropertyInfo()
        {
            var propertyInfo = typeof(ComplexType).GetProperty(nameof(ComplexType.Value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var getterAndSetter = (GetterAndSetter<ComplexType, int>)GetterAndSetter.GetOrCreate(propertyInfo);
            var complexType = new ComplexType();
            getterAndSetter.SetValue(complexType, 1);
            Assert.AreEqual(1, complexType.Value);
            Assert.AreEqual(1, getterAndSetter.GetValue(complexType));
        }

        [Test]
        public void CreateFromFieldInfo()
        {
            var fieldInfo = typeof(ComplexType).GetField(nameof(ComplexType.value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var getterAndSetter = (GetterAndSetter<ComplexType, int>)GetterAndSetter.GetOrCreate(fieldInfo);
            var complexType = new ComplexType();
            getterAndSetter.SetValue(complexType, 1);
            Assert.AreEqual(1, complexType.Value);
            Assert.AreEqual(1, getterAndSetter.GetValue(complexType));
        }

        [Test]
        public void CtorFieldInfo()
        {
            var fieldInfo = typeof(ComplexType).GetField(nameof(ComplexType.value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var getterAndSetter = new GetterAndSetter<ComplexType, int>(fieldInfo);
            var complexType = new ComplexType();
            getterAndSetter.SetValue(complexType, 1);
            Assert.AreEqual(1, complexType.Value);
            Assert.AreEqual(1, getterAndSetter.GetValue(complexType));
        }

        [Test]
        public void SetUsingExpressionSandbox()
        {
            var fieldInfo = typeof(ComplexType).GetField(nameof(ComplexType.value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var source = Expression.Parameter(typeof(ComplexType));
            var fieldExp = Expression.Field(source, fieldInfo);
            var value = Expression.Parameter(typeof(int));
            var assign = Expression.Assign(fieldExp, value);
            var setter = Expression.Lambda<Action<ComplexType, int>>(assign, source, value).Compile();
            var complexType = new ComplexType();
            setter(complexType, 1);
            Assert.AreEqual(1, complexType.Value);
        }
    }
}
