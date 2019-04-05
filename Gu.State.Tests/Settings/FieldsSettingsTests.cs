﻿// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable SA1401 // Fields must be private
namespace Gu.State.Tests.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using NUnit.Framework;

    public class FieldsSettingsTests
    {
        [Test]
        public void Ignores()
        {
            var type = typeof(ComplexType);
            var nameField = type.GetField(nameof(ComplexType.Name), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var valueField = type.GetField(nameof(ComplexType.Value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var settings = new FieldsSettings(new[] { nameField }, null, null, null, null, ReferenceHandling.Throw, Constants.DefaultFieldBindingFlags);
            Assert.AreEqual(true, settings.IsIgnoringField(nameField));
            Assert.AreEqual(false, settings.IsIgnoringField(valueField));
        }

        [Test]
        public void IgnoresWithBuilder()
        {
            var type = typeof(ComplexType);
            var settings = FieldsSettings.Build()
                                         .AddIgnoredField<ComplexType>(nameof(ComplexType.Name))
                                         .CreateSettings();
            var nameField = type.GetField(nameof(ComplexType.Name), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Assert.AreEqual(true, settings.IsIgnoringField(nameField));
            var valueField = type.GetField(nameof(ComplexType.Value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Assert.AreEqual(false, settings.IsIgnoringField(valueField));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(ImmutableList<int>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(ImmutableArray<int>))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ObservableCollection<int>))]
        [TestCase(typeof(Dictionary<int, int>))]
        [TestCase(typeof(ImmutableDictionary<int, int>))]
        [TestCase(typeof(HashSet<int>))]
        [TestCase(typeof(ImmutableHashSet<int>))]
        public void IgnoresCollectionFields(Type type)
        {
            var settings = FieldsSettings.GetOrCreate();
            var fieldInfos = type.GetFields(Constants.DefaultFieldBindingFlags);
            if (type != typeof(int[]))
            {
                CollectionAssert.IsNotEmpty(fieldInfos);
            }

            foreach (var fieldInfo in fieldInfos)
            {
                Assert.AreEqual(true, settings.IsIgnoringField(fieldInfo));
            }
        }

        [Test]
        public void IgnoresIteratorFields()
        {
            var enumaberbles = new object[]
            {
                Enumerable.Repeat(1, 0),
                new[] { 1 }.Where(x => x > 1),
                new[] { 1 }.Select(x => x),
            };
            foreach (var enumerable in enumaberbles)
            {
                var type = enumerable.GetType();
                var settings = FieldsSettings.GetOrCreate();
                var fieldInfos = type.GetFields(Constants.DefaultFieldBindingFlags);
                CollectionAssert.IsNotEmpty(fieldInfos);
                foreach (var fieldInfo in fieldInfos)
                {
                    Assert.AreEqual(true, settings.IsIgnoringField(fieldInfo));
                }
            }
        }

        [Test]
        public void IgnoresNull()
        {
            var settings = FieldsSettings.GetOrCreate();
            Assert.AreEqual(true, settings.IsIgnoringField(null));
        }

        [TestCase(BindingFlags.Public, ReferenceHandling.Throw)]
        [TestCase(BindingFlags.Public, ReferenceHandling.Structural)]
        [TestCase(BindingFlags.Public | BindingFlags.NonPublic, ReferenceHandling.Structural)]
        public void Cache(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            Assert.AreSame(FieldsSettings.GetOrCreate(referenceHandling, bindingFlags), FieldsSettings.GetOrCreate(referenceHandling, bindingFlags));
        }

        public class ComplexType
        {
            public string Name;

            public int Value;
        }
    }
}
