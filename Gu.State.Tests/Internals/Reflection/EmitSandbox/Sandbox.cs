// ReSharper disable All
namespace Gu.State.Tests.Internals.Refelection.EmitSandbox
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    using NUnit.Framework;

    using static Reflection.TypeExtTypes;

    [Explicit("Sandbox")]
    public class FieldAccessorsSandbox
    {
        private ModuleBuilder moduleBuilder;

        [SetUp]
        public void SetUp()
        {
            var type = typeof(ComplexType);
            var assemblyName = new AssemblyName("Gu.State.Emit");
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            this.moduleBuilder = asmBuilder.DefineDynamicModule("FieldAccessors");
        }

        [Test]
        public void AllTheThings()
        {
            var stopwatch = Stopwatch.StartNew();
            var type = typeof(ComplexType);
            var assemblyName = new AssemblyName("Gu.State.Emit");
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = asmBuilder.DefineDynamicModule("FieldAccessors");
            var typeBuilder = moduleBuilder.DefineType($"{assemblyName.Name}.{type.Name}", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);
            var field = type.GetField(nameof(ComplexType.value), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var setMethod = typeBuilder.DefineMethod($"set_{field.Name}", MethodAttributes.Public | MethodAttributes.Static, null, new[] { field.DeclaringType, field.FieldType });
            var ilGenerator = setMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, field);
            ilGenerator.Emit(OpCodes.Ret);

            var getterMethod = typeBuilder.DefineMethod($"get_{field.Name}", MethodAttributes.Public | MethodAttributes.Static, field.FieldType, new[] { field.DeclaringType });
            ilGenerator = getterMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, field);
            ilGenerator.Emit(OpCodes.Ret);
            var accessor = typeBuilder.CreateType();
            var setter = (Action<ComplexType, int>)accessor.GetMethod($"set_{field.Name}").CreateDelegate(typeof(Action<ComplexType, int>));
            var getter = (Func<ComplexType, int>)accessor.GetMethod($"get_{field.Name}").CreateDelegate(typeof(Func<ComplexType, int>));

            var complexType = new ComplexType();
            setter(complexType, 1);
            Assert.AreEqual(1, complexType.value);
            Console.WriteLine($"AllTheThings {stopwatch.Elapsed}");
        }

        [Test]
        public void CachedAssembly()
        {
            var stopwatch = Stopwatch.StartNew();
            var type = typeof(ComplexType);
            var typeBuilder = this.moduleBuilder.DefineType($"Gu.State.Emit.{type.Name}", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);
            foreach (var field in type.GetFields(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance))
            {
                var setMethod = typeBuilder.DefineMethod($"set_{field.Name}", MethodAttributes.Public | MethodAttributes.Static, null, new[] { field.DeclaringType, field.FieldType });
                var ilGenerator = setMethod.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Stfld, field);
                ilGenerator.Emit(OpCodes.Ret);

                var getterMethod = typeBuilder.DefineMethod($"get_{field.Name}", MethodAttributes.Public | MethodAttributes.Static, field.FieldType, new[] { field.DeclaringType });
                ilGenerator = getterMethod.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, field);
                ilGenerator.Emit(OpCodes.Ret);
            }

            var accessor = typeBuilder.CreateType();
            Console.WriteLine($"CreateType {stopwatch.Elapsed}");

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var setter = (Action<ComplexType, int>)accessor.GetMethod($"set_{field.Name}")
                                                               .CreateDelegate(typeof(Action<ComplexType, int>));
                var getter = (Func<ComplexType, int>)accessor.GetMethod($"get_{field.Name}")
                                                             .CreateDelegate(typeof(Func<ComplexType, int>));
            }
            Console.WriteLine($"CreateDelegates {stopwatch.Elapsed}");
            //var complexType = new ComplexType();
            //setter(complexType, 1);
            //Assert.AreEqual(1, complexType.value);
            //Console.WriteLine($"CachedAssembly {stopwatch.Elapsed}");
        }


        [Test]
        public void CtorFieldInfo()
        {
            var type = typeof(ComplexType);
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var getterAndSetter = new GetterAndSetter<ComplexType, int>(field);
                //var complexType = new ComplexType();
                //getterAndSetter.SetValue(complexType, 1);
                //Assert.AreEqual(1, complexType.Value);
                //Assert.AreEqual(1, getterAndSetter.GetValue(complexType));
            }
        }
    }
}
