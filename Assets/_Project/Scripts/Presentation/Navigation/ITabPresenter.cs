namespace App.Presentation.Navigation
{
    public interface ITabPresenter
    {
        TabId Id { get; }
        void Open();
        void Close();
    }
}
