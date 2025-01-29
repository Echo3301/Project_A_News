namespace Assignment_A2_04.Models;

public class NewsCacheKey
{
    readonly NewsCategory _category;
    readonly string _timewindow;

    public string FileName => fname("Cache-" + Key + ".xml");
    public string Key => _category.ToString() + _timewindow;
    public bool CacheExist => File.Exists(FileName);

    public NewsCacheKey (NewsCategory category, DateTime dt)
    {
        _category = category;
        _timewindow = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
    }
    static string fname(string name)
    {
        var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        documentPath = Path.Combine(documentPath, "ADOP", "ProjectB");
        if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
        return Path.Combine(documentPath, name);
    }
}