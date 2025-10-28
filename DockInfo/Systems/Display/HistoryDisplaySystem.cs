using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    public class HistoryDisplaySystem : IAsyncSystem
    {
        private readonly Program _program;
        private readonly ConnectionHistoryRepository _connectionHistoryRepository;
        private readonly DisplayRepository _displayRepository;

        public HistoryDisplaySystem(Program program)
        {
            _program = program;
            _connectionHistoryRepository = _program.Container.GetItem<ConnectionHistoryRepository>();
            _displayRepository = _program.Container.GetItem<DisplayRepository>();
        }

        public IEnumerator<bool> Run()
        {
            foreach (var display in _displayRepository.History)
            {
                var amountPerPage = display.Panel.MaxLines() - 2;
                var maxPage = _connectionHistoryRepository.MaxPage(amountPerPage);
                var currentPage = _displayRepository.HistoryPage;
                if (currentPage >= maxPage)
                {
                    currentPage = maxPage - 1;
                }

                var content = _connectionHistoryRepository.GetPage(
                    amountPerPage,
                    currentPage
                );

                var sb = new StringBuilder();
                sb.AppendLine(display.Panel.FormatTitle("[Connection History]"));

                foreach (var line in content)
                {
                    switch (line.HistoryType)
                    {
                        case HistoryType.Landing:
                            sb.AppendLine(
                                $"[{line.Timestamp:MM/dd HH:mm:ss}][{line.DockGroupName}<-Docked] {line.GridName}");
                            break;
                        case HistoryType.Takeoff:
                            sb.AppendLine(
                                $"[{line.Timestamp:MM/dd HH:mm:ss}][{line.DockGroupName}->Undocked] {line.GridName}");
                            break;
                        default:
                            throw new Exception($"Unknown history type {line.HistoryType}");
                    }
                }

                sb.AppendLine(display.Panel.FormatTitle($"[page {currentPage + 1}/{maxPage}]"));

                display.Panel.WriteText(sb.ToString());

                yield return true;
            }

            yield return false;
        }
    }
}