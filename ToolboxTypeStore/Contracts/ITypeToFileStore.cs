// ReSharper disable CheckNamespace
#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxTypeStore;



public interface ITypeToFileStore<out T> : IDisposable

    where T : class, ITypeToFileValue, new()

{

    /// <summary>

    /// The instance of the type held in the store.

    /// It´s loaded on creation of the store, and saved on dispose

    /// </summary>

    T Instance { get; }

    

    /// <summary>

    /// Gets the timestamp, the instance was created

    /// </summary>

    DateTimeOffset TimestampCreated { get; }



    /// <summary>

    /// Gets the timestamp, the instance was last saved

    /// </summary>

    DateTimeOffset? TimestampSaved { get; }



    /// <summary>

    /// Resets the instance to a new default instance, and saves it right away

    /// </summary>

    /// <returns>the new instance</returns>

    T CommitReset();



    /// <summary>

    /// Saves the current state of the Instance to file

    /// </summary>

    void SaveChanges();

}


