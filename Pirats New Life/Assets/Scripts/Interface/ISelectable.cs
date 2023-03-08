
public interface ISelectable
{
    void OnSelect();
    void OnDeselect();
    void OnEnable()
    {
        Events.Cursor.onDeselect += OnDeselect;
    }

    void OnDisable()
    {
        Events.Cursor.onDeselect -= OnDeselect;
    }
}