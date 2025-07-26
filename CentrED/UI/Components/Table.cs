using Hexa.NET.ImGui;

namespace CentrED.UI.Components;

public abstract class Table<TContext> : IUIComponent
{
    protected TContext Context { get; }
    private string _id;
    private int _columnCount;

    public Table(TContext parent, string id, int columnCount = 1)
    {
        Context = parent;
        _id = id;
        _columnCount = columnCount;
    }

    public void Draw()
    {
        ImGui.PushID(_id);

        if (ImGui.BeginTable(_id, _columnCount))
        {
            DrawContents();
        }
        ImGui.EndTable();

        ImGui.PopID();
    }

    protected abstract void DrawContents();
}
