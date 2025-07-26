using System.Numerics;
using Hexa.NET.ImGui;

namespace CentrED.UI.Components;

public abstract class Panel<TContext> : IUIComponent
{
    protected TContext Context;
    protected string _id;
    private ImGuiChildFlags _childFlags;
    private ImGuiWindowFlags _windowFlags;
    private Vector2 _size;

    protected Panel(TContext parent, string id, ImGuiChildFlags childFlags = ImGuiChildFlags.None, ImGuiWindowFlags windowFlags = ImGuiWindowFlags.None, Vector2 size = new Vector2())
    {
        Context = parent;
        _id = id;
        _childFlags = childFlags;
        _windowFlags = windowFlags;
        _size = size;
    }

    public void Draw()
    {
        ImGui.PushID(_id);

        if (ImGui.BeginChild(_id, _size, _childFlags, _windowFlags))
        {
            DrawContents();
        }
        ImGui.EndChild();

        ImGui.PopID();
    }

    protected abstract void DrawContents();
}
