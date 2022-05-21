using System.IO;
using System.Reflection;


namespace aemarcoCommons.Toolbox.SerializationTools
{

    //https://josef.codes/using-embedded-files-in-dotnet-core/
    //https://github.com/joseftw/jos.embeddedresources


    public interface IEmbeddedResourceQuery
    {
        /// <summary>
        /// read the bytes from a embedded file
        /// </summary>
        /// <typeparam name="T">a type in the assembly with the resource file</typeparam>
        /// <param name="resource">path of resource in . notation (Resources.MyFile.png)</param>
        /// <returns>stream with bytes</returns>
        Stream Read<T>(string resource);
        /// <summary>
        /// read the bytes from a embedded file
        /// </summary>
        /// <param name="assembly">assembly containing the resource file</param>
        /// <param name="resource">path of resource in . notation (Resources.MyFile.png)</param>
        /// <returns>stream with bytes</returns>
        Stream Read(Assembly assembly, string resource);
        /// <summary>
        /// read the bytes from a embedded file
        /// </summary>
        /// <param name="assemblyName">assembly name containing the resource file</param>
        /// <param name="resource">path of resource in . notation (Resources.MyFile.png)</param>
        /// <returns>stream with bytes</returns>
        Stream Read(string assemblyName, string resource);
    }


    public class EmbeddedResourceQuery : IEmbeddedResourceQuery
    {
        public Stream Read<T>(string resource)
        {
            var assembly = typeof(T).Assembly;
            return ReadInternal(assembly, resource);
        }

        public Stream Read(Assembly assembly, string resource)
        {
            return ReadInternal(assembly, resource);
        }

        public Stream Read(string assemblyName, string resource)
        {
            var assembly = Assembly.Load(assemblyName);
            return ReadInternal(assembly, resource);
        }

        private static Stream ReadInternal(Assembly assembly, string resource)
        {
            return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resource}");
        }

    }
}
