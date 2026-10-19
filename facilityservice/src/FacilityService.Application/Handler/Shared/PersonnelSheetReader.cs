using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace FacilityService.Application.Handler.Shared;

/// <summary>
/// خواندن فایل بارگذاری گروهی پرسنل (US-E2): CSV (UTF-8) و XLSX (اولین شیت) بدون کتابخانه، XLS قدیمی با ExcelDataReader.
/// </summary>
public static class PersonnelSheetReader
{
    public static readonly string[] Header = ["نام و نام خانوادگی", "کد ملی", "موبایل", "کد پرسنلی", "تاریخ استخدام", "اولویت تخصیص"];

    /// <returns>ردیف‌های داده (بدون سرتیتر)، هر ردیف با شمارهٔ ردیف فایل</returns>
    public static IReadOnlyList<(int Row, string[] Cells)> Read(FileUpload file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var rows = ext switch
        {
            ".csv" => ReadCsv(file.Content),
            ".xlsx" => ReadXlsx(file.Content),
            ".xls" => ReadXls(file.Content),
            _ => throw new FacilityDomainException("فرمت فایل باید xlsx، xls یا csv باشد."),
        };
        return rows.Select((cells, i) => (Row: i + 1, Cells: cells))
            .Skip(1)
            .Where(r => r.Cells.Any(c => !string.IsNullOrWhiteSpace(c)))
            .ToList();
    }

    public static byte[] TemplateCsv() =>
        Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(
            string.Join(",", Header) + "\nعلی محمدی,0012345678,09121234567,101,1403/02/15,1\n")).ToArray();

    private static List<string[]> ReadCsv(byte[] content)
    {
        var text = Encoding.UTF8.GetString(content).TrimStart('﻿');
        var rows = new List<string[]>();
        foreach (var line in text.Split('\n'))
        {
            var l = line.TrimEnd('\r');
            rows.Add(SplitCsvLine(l));
        }
        return rows;
    }

    private static string[] SplitCsvLine(string line)
    {
        var cells = new List<string>();
        var sb = new StringBuilder();
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (quoted)
            {
                if (ch == '"' && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                else if (ch == '"') quoted = false;
                else sb.Append(ch);
            }
            else if (ch == '"') quoted = true;
            else if (ch == ',') { cells.Add(sb.ToString().Trim()); sb.Clear(); }
            else sb.Append(ch);
        }
        cells.Add(sb.ToString().Trim());
        return cells.ToArray();
    }

    private static readonly XNamespace S = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    private static List<string[]> ReadXlsx(byte[] content)
    {
        try
        {
            using var zip = new ZipArchive(new MemoryStream(content), ZipArchiveMode.Read);
            var shared = zip.GetEntry("xl/sharedStrings.xml") is { } se
                ? XDocument.Load(se.Open()).Root!.Elements(S + "si").Select(si => string.Concat(si.Descendants(S + "t").Select(t => t.Value))).ToList()
                : [];
            var sheet = zip.Entries.Where(e => e.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase))
                            .OrderBy(e => e.FullName.Length).ThenBy(e => e.FullName).FirstOrDefault()
                        ?? throw new FacilityDomainException("فایل اکسل شیت ندارد.");
            var rows = new List<string[]>();
            foreach (var row in XDocument.Load(sheet.Open()).Descendants(S + "row"))
            {
                var cells = new string[Header.Length];
                foreach (var c in row.Elements(S + "c"))
                {
                    var col = ColumnIndex((string?)c.Attribute("r"));
                    if (col < 0 || col >= cells.Length) continue;
                    var type = (string?)c.Attribute("t");
                    var v = c.Element(S + "v")?.Value;
                    cells[col] = type switch
                    {
                        "s" when int.TryParse(v, out var idx) && idx < shared.Count => shared[idx],
                        "inlineStr" => string.Concat(c.Descendants(S + "t").Select(t => t.Value)),
                        _ => v ?? "",
                    };
                }
                rows.Add(cells.Select(x => (x ?? "").Trim()).ToArray());
            }
            return rows;
        }
        catch (InvalidDataException)
        {
            throw new FacilityDomainException("فایل اکسل معتبر نیست.");
        }
    }

    private static List<string[]> ReadXls(byte[] content)
    {
        // فایل‌های BIFF قدیمی کدپیج ویندوز دارند (فارسی: 1256)
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        try
        {
            using var reader = ExcelDataReader.ExcelReaderFactory.CreateBinaryReader(new MemoryStream(content));
            var rows = new List<string[]>();
            while (reader.Read())
            {
                var cells = new string[Header.Length];
                for (var i = 0; i < cells.Length && i < reader.FieldCount; i++)
                    cells[i] = reader.GetValue(i) switch
                    {
                        null => "",
                        DateTime d => d.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                        double n => n.ToString("0.##########", System.Globalization.CultureInfo.InvariantCulture),
                        var v => (Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture) ?? "").Trim(),
                    };
                rows.Add(cells.Select(x => x ?? "").ToArray());
            }
            return rows;
        }
        catch (Exception ex) when (ex is ExcelDataReader.Exceptions.ExcelReaderException or InvalidDataException or NotSupportedException)
        {
            throw new FacilityDomainException("فایل اکسل (xls) معتبر نیست.");
        }
    }

    private static int ColumnIndex(string? reference)
    {
        if (string.IsNullOrEmpty(reference)) return -1;
        var index = 0;
        foreach (var ch in reference.TakeWhile(char.IsLetter)) index = index * 26 + (char.ToUpperInvariant(ch) - 'A' + 1);
        return index - 1;
    }

    /// <summary>ارقام فارسی/عربی ← لاتین.</summary>
    public static string Latin(string? s) => new((s ?? "").Trim().Select(c => c switch
    {
        >= '۰' and <= '۹' => (char)('0' + (c - '۰')),
        >= '٠' and <= '٩' => (char)('0' + (c - '٠')),
        _ => c,
    }).ToArray());

    /// <summary>تاریخ استخدام: شمسی «1403/02/15»، میلادی «2024-05-04» یا عدد تاریخ اکسل.</summary>
    public static DateOnly? ParseDate(string? text)
    {
        var s = Latin(text);
        var parts = s.Split('/', '-');
        if (parts.Length == 3 && int.TryParse(parts[0], out var y) && int.TryParse(parts[1], out var m) && int.TryParse(parts[2], out var d))
        {
            try
            {
                if (y is >= 1300 and < 1500) return Jalali.ToDate(y, m, d);
                if (y is >= 1900 and < 2200) return new DateOnly(y, m, d);
            }
            catch (ArgumentOutOfRangeException) { return null; }
        }
        if (double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var serial) && serial is > 1000 and < 100000)
            return DateOnly.FromDateTime(DateTime.FromOADate(serial));
        return null;
    }
}
