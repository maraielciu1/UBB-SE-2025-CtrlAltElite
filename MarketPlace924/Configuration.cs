using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MarketPlace924;

public class Configuration
{
    public static Configuration Instance = new();

    private Dictionary<string, string> _configuration = new();

    private Configuration()
    {
        var lines = File.ReadAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "MarketPlaceConfig.ini"));
        var sectionPattern = @"^\[(?<section>\w+)\]$";
        var propertyPattern = @"^(?<propertyName>\w+)=(?<propertyValue>.*)$";

        var currentSection = string.Empty;
        foreach (var line in lines)
        {
            var match = Regex.Match(line, sectionPattern);
            if (match.Success)
            {
                currentSection = match.Groups["section"].Value;
            }

            match = Regex.Match(line, propertyPattern);
            if (match.Success)
            {
                _configuration.Add(currentSection + ":" + match.Groups["propertyName"].Value,
                    match.Groups["propertyValue"].Value);
            }
        }
    }

    public Dictionary<string, string> Properties => _configuration;
}