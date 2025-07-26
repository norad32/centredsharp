using System.Numerics;
using Hexa.NET.ImGui;

namespace CentrED.UI.Components;

public abstract class ListPanel<TContext, TItem> : Panel<TContext>
{
    public ListPanel(TContext context, string id, ImGuiChildFlags childFlags = ImGuiChildFlags.None, ImGuiWindowFlags windowFlags = ImGuiWindowFlags.None, Vector2 size = default)
        : base(context, id, childFlags, windowFlags, size)
    {
    }

    protected abstract IReadOnlyList<TItem> Items { get; }
    protected abstract float RowHeight { get; }
    protected abstract float TotalRowHeight { get; }

    protected abstract void DrawItemRow(TItem item, int index);
    protected abstract bool IsSelected(TItem item);
    protected abstract void OnItemSelected(TItem item);
    protected virtual void DrawRowExtras(TItem item) { }
    protected virtual void HandleScroll(ImGuiListClipperPtr clipper) { }

    protected override void DrawContents()
    {
        if (!ImGui.BeginTable(_id, 2))
            return;

        ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("0x0000").X);

        var clipper = ImGui.ImGuiListClipper();
        clipper.Begin(Items.Count, TotalRowHeight);

        while (clipper.Step())
        {
            for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
            {
                var item = Items[i];
                var y = ImGui.GetCursorPosY();

                DrawItemRow(item, i);

                ImGui.SetCursorPosY(y);

                if (ImGui.Selectable(
                    $"##item{i}",
                    IsSelected(item),
                    ImGuiSelectableFlags.SpanAllColumns,
                    new Vector2(ImGui.GetContentRegionAvail().X, RowHeight)))
                {
                    OnItemSelected(item);
                }

                DrawRowExtras(item);
            }
        }

        clipper.End();

        HandleScroll(clipper);

        ImGui.EndTable();
    }

    protected int GetItemIndex(TItem lookup)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (EqualityComparer<TItem>.Default.Equals(Items[i], lookup))
            {
                return i;
            }
        }
        return -1;
    }
}