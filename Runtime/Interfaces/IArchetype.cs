namespace d4160.Core
{
    public interface IArchetype
    {
        /// <summary>
        /// The unique identifier of the Archetype
        /// </summary>
        int ID { get; set; }
    }

    public interface IArchetypeName
    {
        string Name { get; set; }
    }
}
