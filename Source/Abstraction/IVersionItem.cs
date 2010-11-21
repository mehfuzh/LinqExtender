namespace LinqExtender.Abstraction
{
    interface IVersionItem
    {
        void Commit();
        void Revert();
        object Item { get; }
    }
}