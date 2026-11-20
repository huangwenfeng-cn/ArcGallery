using System.Globalization;
using Avalonia.Controls;

namespace TKCode.Arc.Gallery.Demos;

public partial class DataGridRemoteEditingDemo : UserControl
{
    public DataGridRemoteEditingDemo()
    {
        InitializeComponent();
        var source = new RecordsSource();
        RecordsGrid.RowKeySelector = row => ((DataGridRow)row).Key;
        RecordsGrid.DataSource = source;
        RejectWrites.IsCheckedChanged += (_, _) => source.RejectWrites = RejectWrites.IsChecked == true;
        CancelSave.Click += (_, _) => RecordsGrid.CancelEdit();
    }

    // 阅读顺序：GetRowsAsync 计算当前查询，UpdateCellAsync 保存并返回独立快照。
    // 此处用三个内存记录模拟服务端，不引入 HTTP、数据库、定时器或全局服务。
    // 实际应用应在写入接口内校验权限与版本，并保证后续读取能读到已经确认保存的记录。
    private sealed class RecordsSource : IDataGridEditableDataSource
    {
        private readonly Dictionary<object, DataGridRow> _rows = new()
        {
            ["r1"] = new("r1") { { "name", "Alder" }, { "amount", 120m }, { "active", true } },
            ["r2"] = new("r2") { { "name", "Basalt" }, { "amount", 80m }, { "active", false } },
            ["r3"] = new("r3") { { "name", "Cobalt" }, { "amount", 250m }, { "active", true } }
        };

        public bool RejectWrites { get; set; }

        public Task<DataGridRowsResult> GetRowsAsync(DataGridRowsRequest request, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            IEnumerable<DataGridRow> rows = _rows.Values;
            foreach (var (column, filter) in request.Filters)
            {
                if (filter.Values is { } values)
                {
                    rows = rows.Where(row => values.Contains(TextOf(row[column])));
                }
                else if (column == "amount" &&
                         DataGridFilterExpression.TryParseNumber(filter.Expression, CultureInfo.InvariantCulture, out var number) &&
                         number is not null)
                {
                    rows = rows.Where(row => row[column] is decimal amount && number(amount));
                }
                else if (DataGridFilterExpression.CreateText(filter.Expression, CultureInfo.InvariantCulture) is { } text)
                {
                    rows = rows.Where(row => text(TextOf(row[column])));
                }
            }
            if (request.Search.Length > 0)
            {
                rows = rows.Where(row => $"{row["name"]} {row["amount"]} {row["active"]}"
                    .Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }
            var ordered = DataGridSorting.Sort(rows, request.Sort, (row, key) => row[key], culture: CultureInfo.InvariantCulture).ToArray();
            var page = ordered.Skip(request.Start).Take(request.End - request.Start).Cast<object>().ToArray();
            var total = DataGridAggregates.Compute(ordered.Select(row => row["amount"]), DataGridAggregate.Sum);
            return Task.FromResult(new DataGridRowsResult(page, ordered.Length, new Dictionary<string, decimal?> { ["amount"] = total }));
        }

        private static string TextOf(object? value) => value is bool flag ? flag ? "true" : "false" : value?.ToString() ?? string.Empty;

        public async Task<object> UpdateCellAsync(DataGridCellChange change, CancellationToken cancellation)
        {
            // 延迟用来观察 pending 和取消，不自动重试。拒绝时原记录保持不变，编辑器保留草稿。
            await Task.Delay(500, cancellation);
            if (RejectWrites)
            {
                throw new InvalidOperationException("The demonstration service rejected the save.");
            }
            var current = _rows[change.RowKey];
            if (!Equals(current[change.ColumnKey], change.OldValue))
            {
                throw new InvalidOperationException("The record changed on the server.");
            }
            // 服务端规范化：去除姓名两端空格、金额保留两位小数。控件的成功事件采用这些权威值。
            var value = change.NewValue switch
            {
                string text => text.Trim(),
                decimal amount => Math.Round(amount, 2),
                _ => change.NewValue
            };
            var updated = new DataGridRow(current.Key)
            {
                { "name", current["name"] }, { "amount", current["amount"] }, { "active", current["active"] }
            };
            updated[change.ColumnKey] = value;
            _rows[change.RowKey] = updated;
            return updated;
        }
    }
}
