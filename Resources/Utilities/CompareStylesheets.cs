using System.Text.RegularExpressions;

HashSet<string> ReadSelectors(string file){
    return new HashSet<string>(
        File.ReadAllLines(file)
            .Where(line => line.Contains('{'))
            .Select(line => line.Substring(0, line.IndexOf('{')).Trim())
            .SelectMany(lines => lines.Split(new char[]{ ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
    );
}

HashSet<string> ExtractClasses(HashSet<string> selectors){
    return new HashSet<string>(
        selectors.SelectMany(selector => Regex.Matches(selector, @"\.[a-zA-Z0-9_-]+").Cast<Match>().Select(match => match.Value))
    );
}

void PrintAll(IEnumerable<string> data){
    foreach(string line in data){
        Print(line);
    }
}

void PrintMissing(HashSet<string> all, HashSet<string> subset){
    PrintAll(subset.Where(ele => !all.Contains(ele)));
}
