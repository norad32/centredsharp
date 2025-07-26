using Hexa.NET.ImGui;

namespace CentrED.UI.Components;

public abstract class PopupModal<TContext> : IUIComponent
{
    protected TContext Context;
    private string _id;
    private ImGuiWindowFlags _windowFlags;

    public PopupModal(TContext context, string id, ImGuiWindowFlags windowFlags = ImGuiWindowFlags.None)
    {
        Context = context;
        _id = id;
        _windowFlags = windowFlags;
    }

    public void Draw()
    {
        ImGui.PushID(_id);

        if (ImGui.BeginPopupModal(_id, _windowFlags))
        {
            DrawContents();
            ImGui.EndPopup();
        }

        ImGui.PopID();
    }

    public void Open()
    {
        ImGui.PushID(_id);
        ImGui.OpenPopup(_id);
        ImGui.PopID();
    }

    protected abstract void DrawContents();
}
