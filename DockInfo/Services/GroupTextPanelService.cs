using System;
using System.Text;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class GroupTextPanelService : IContainerItem
    {
        private readonly Program _program;
        private readonly ConnectionHistoryRepository _connectionHistoryRepository;
        
        private readonly MyIni _ini = new MyIni();

        public GroupTextPanelService(Program program)
        {
            _program = program;
            _connectionHistoryRepository = _program.Container.GetItem<ConnectionHistoryRepository>();
        }

        public void RefreshPanels(DockGroup group)
        {
            foreach (var textPanel in group.TextPanels)
            {
                try
                {
                    var section = group.IsConnected ? DataStructure.ConnectedSection : DataStructure.DisconnectedSection;
                    if (_ini.TryParse(textPanel.CustomData))
                    {
                        textPanel.FontSize = _ini.Get(section, DataStructure.FontSizeField)
                            .ToSingle();

                        textPanel.TextPadding = _ini.Get(section,
                                DataStructure.TextPaddingField)
                            .ToSingle();

                        var alignStr = _ini.Get(section, DataStructure.TextAlignField)
                            .ToString();
                        TextAlignment align;
                        if (Enum.TryParse(alignStr, out align))
                        {
                            textPanel.Alignment = align;
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to parse text panel custom data");
                    }
                }
                catch (Exception e)
                {
                    _program.Echo($"Failed to read ${textPanel.CustomName} custom data: {e.Message}");
                }
                
                if (group.IsConnected)
                {
                    var sb = new StringBuilder();
                    var otherGrid = group.Connector.OtherConnector.CubeGrid;
                    var shipName = otherGrid.CustomName;
                    var faction = group.Connector.OtherConnector.GetOwnerFactionTag();

                    sb.AppendLine($"[{shipName}]");
                    sb.AppendLine($"Faction: {faction}");
                    
                    var history = _connectionHistoryRepository.GetLastHistory(otherGrid.EntityId);
                    if (history.HasValue)
                    {
                        sb.AppendLine($"Connected since: {history.Value.Timestamp}");
                    }

                    textPanel.WritePublicTitle(group.Name);
                    textPanel.WriteText(sb.ToString());
                }
                else
                {
                    textPanel.WritePublicTitle(group.Name);
                    textPanel.WriteText(group.Name);
                }
            }
        }
    }
}