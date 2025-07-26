using Hexa.NET.ImGui;

namespace CentrED.UI.Components;

public abstract class PopupContext<TContext> : IUIComponent
{
    protected TContext Context;

    public PopupContext(TContext context)
    {
        Context = context;
    }

    public void Draw()
    {
        if (ImGui.BeginPopupContextItem())
        {
            DrawContents();
            ImGui.EndPopup();
        }
    }

    protected abstract void DrawContents();
}
