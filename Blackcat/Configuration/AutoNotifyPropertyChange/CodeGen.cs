using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Blackcat.Configuration.AutoNotifyPropertyChange
{
    internal static class CodeGen
    {
        private static readonly AssemblyBuilder AssemblyBuilder;
        private static readonly ModuleBuilder ModuleBuilder;

        static CodeGen()
        {
            AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Blackcat.Utils.AutoNotifyPropertyChange"), AssemblyBuilderAccess.Run);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule("Module");
        }

        public static Type CreateType(string name, Action<TypeBuilder> cb)
        {
            lock (AssemblyBuilder)
            {
                var builder = ModuleBuilder.DefineType(Guid.NewGuid().ToString().Replace("-", "") + "." + name);
                cb(builder);
                return builder.CreateType();
            }
        }
    }
}