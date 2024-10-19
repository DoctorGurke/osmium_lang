using System.Reflection;

namespace Osmium;

public abstract class Script
{
    /// <summary>
    /// Load script source code from a file on disc. 
    /// </summary>
    /// <param name="path">Absolute or relative path to file, relative to .exe/scripts.</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static string Load(string path)
    {
        var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (executableLocation is null)
            throw new Exception($"Couldn't find Executable location!");
        var scriptsPath = Path.Combine(executableLocation, "scripts");

        var fileFullPath = Path.IsPathFullyQualified(path) ? path : Path.Combine(scriptsPath, path);

        try
        {
            var fileContent = File.ReadAllText(fileFullPath);
            return fileContent;
        }
        catch
        {
            Log.Info($"script not found: {fileFullPath}");

            throw new FileNotFoundException(path);
        }
    }
}
