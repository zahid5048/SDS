namespace ChemicalSDS.Models;

/// <summary>
/// Parses/serializes occupational exposure limits: one ingredient per row, multi-line limits.
/// Storage: rows separated by <c>\n---\n</c>; each row is <c>ingredient|limits</c> (limits may contain newlines).
/// Legacy single-line-per-limit text is still parsed (continuation lines append to the previous ingredient).
/// </summary>
public static class OelLimitsFormatter
{
    public const string RowSeparator = "\n---\n";

    public record OelRow(string Ingredient, string Limits);

    public static IReadOnlyList<OelRow> Parse(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return Array.Empty<OelRow>();

        if (data.Contains(RowSeparator, StringComparison.Ordinal))
        {
            return data.Split(RowSeparator, StringSplitOptions.None)
                .Select(ParseBlock)
                .Where(r => !string.IsNullOrWhiteSpace(r.Ingredient) || !string.IsNullOrWhiteSpace(r.Limits))
                .ToList();
        }

        var rows = new List<OelRow>();
        string? ingredient = null;
        var limits = new System.Text.StringBuilder();

        foreach (var rawLine in data.Split('\n'))
        {
            var line = rawLine.TrimEnd();
            if (string.IsNullOrWhiteSpace(line))
            {
                if (ingredient != null && limits.Length > 0)
                    limits.Append('\n');
                continue;
            }

            var pipeIdx = line.IndexOf('|');
            if (pipeIdx > 0 && LooksLikeIngredientRow(line, pipeIdx))
            {
                Flush();
                ingredient = line[..pipeIdx].Trim();
                var after = line[(pipeIdx + 1)..].Trim();
                if (!string.IsNullOrEmpty(after))
                {
                    if (limits.Length > 0)
                        limits.Append('\n');
                    limits.Append(after);
                }
            }
            else if (ingredient != null)
            {
                if (limits.Length > 0)
                    limits.Append('\n');
                limits.Append(line.Trim());
            }
            else if (pipeIdx > 0)
            {
                rows.Add(new OelRow(line[..pipeIdx].Trim(), line[(pipeIdx + 1)..].Trim()));
            }
        }

        Flush();
        return rows;

        void Flush()
        {
            if (ingredient == null)
                return;
            rows.Add(new OelRow(ingredient, limits.ToString()));
            ingredient = null;
            limits.Clear();
        }
    }

    public static string Serialize(IEnumerable<OelRow> rows)
    {
        var list = rows
            .Where(r => !string.IsNullOrWhiteSpace(r.Ingredient) || !string.IsNullOrWhiteSpace(r.Limits))
            .Select(r => new OelRow(r.Ingredient.Trim(), r.Limits.Trim()))
            .ToList();

        if (list.Count == 0)
            return string.Empty;

        return string.Join(RowSeparator, list.Select(r =>
            string.IsNullOrEmpty(r.Limits) ? $"{r.Ingredient}|" : $"{r.Ingredient}|{r.Limits}"));
    }

    private static OelRow ParseBlock(string block)
    {
        var lines = block.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var idx = line.IndexOf('|');
            if (idx <= 0)
                continue;

            var ing = line[..idx].Trim();
            var lim = new System.Text.StringBuilder(line[(idx + 1)..].Trim());
            for (var j = i + 1; j < lines.Length; j++)
            {
                if (lim.Length > 0)
                    lim.Append('\n');
                lim.Append(lines[j].TrimEnd());
            }

            return new OelRow(ing, lim.ToString());
        }

        return new OelRow(string.Empty, block.Trim());
    }

    private static bool LooksLikeIngredientRow(string line, int pipeIdx)
    {
        var left = line[..pipeIdx].Trim();
        var right = line[(pipeIdx + 1)..].Trim();
        if (left.Length > 120)
            return false;
        if (ContainsLimitKeyword(left) && !ContainsLimitKeyword(right))
            return false;
        return !ContainsLimitKeyword(left) || left.Length < 50;
    }

    private static bool ContainsLimitKeyword(string s) =>
        s.Contains("ppm", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("mg/m", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("TWA", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("STEL", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("OSHA", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("ACGIH", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("NIOSH", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("PEL", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("REL", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("TLV", StringComparison.OrdinalIgnoreCase);
}
