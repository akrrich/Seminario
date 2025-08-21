public interface IBookableUI 
{
    int IndexPanel { get; } // La idea es que esta variable represente el orden en el cual se va a mostrar cada panel en el libro

    void OpenPanel();

    void ClosePanel();
}
