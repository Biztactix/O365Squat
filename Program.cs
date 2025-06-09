using DnsClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O365TypoSquat
{
    public class TypoSquatGenerator
    {
        private static readonly Dictionary<int, (string original, string replacement)> ReplacementGlyphs = new()
        {
            { 0, ("0", "b") },
            { 1, ("1", "b") },
            { 2, ("2", "c") },
            { 3, ("3", "d") },
            { 4, ("4", "d") },
            { 5, ("5", "d") },
            { 6, ("6", "e") },
            { 7, ("7", "g") },
            { 8, ("8", "h") },
            { 9, ("9", "i") },
            { 10, ("i", "l") },
            { 11, ("k", "lk") },
            { 12, ("k", "ik") },
            { 13, ("k", "lc") },
            { 14, ("l", "1") },
            { 15, ("l", "i") },
            { 16, ("m", "n") },
            { 17, ("m", "nn") },
            { 18, ("m", "rn") },
            { 19, ("m", "rr") },
            { 20, ("n", "r") },
            { 21, ("n", "m") },
            { 22, ("o", "0") },
            { 23, ("o", "q") },
            { 24, ("q", "g") },
            { 25, ("u", "v") },
            { 26, ("v", "u") },
            { 27, ("w", "vv") },
            { 28, ("w", "uu") },
            { 29, ("z", "s") },
            { 30, ("n", "r") },
            { 31, ("r", "n") }
        };

        public static List<string> GenerateTypoSquatDomains(string domainName)
        {
            var parts = domainName.Split('.');
            var tld = parts.Last();
            var domain = parts.First();
            
            var variations = new List<string>();
            
            // Homoglyph variations
            for (int i = 0; i < 29; i++)
            {
                if (ReplacementGlyphs.ContainsKey(i))
                {
                    var (original, replacement) = ReplacementGlyphs[i];
                    var newDomain = domain.Replace(original, replacement);
                    
                    variations.Add(newDomain);
                    variations.Add(newDomain + "s");
                    variations.Add(newDomain + "a");
                    variations.Add(newDomain + "t");
                    variations.Add(newDomain + "en");
                }
            }
            
            // Bit squatting and omission
            for (int i = 0; i < domain.Length; i++)
            {
                // Character omission
                if (i + 1 < domain.Length)
                {
                    var omitted = domain.Substring(0, i) + domain.Substring(i + 2);
                    variations.Add(omitted);
                }
                
                // Character transposition
                if (i + 2 < domain.Length)
                {
                    var transposed = domain.Substring(0, i) + 
                                   domain[i + 1] + 
                                   domain[i] + 
                                   domain.Substring(i + 2);
                    variations.Add(transposed);
                }
            }
            
            // Plurals
            variations.Add(domain + "s");
            variations.Add(domain + "a");
            variations.Add(domain + "en");
            variations.Add(domain + "t");
            
            // Add TLD and remove duplicates
            var combinedDomains = variations
                .Select(v => $"{v}.{tld}")
                .Distinct()
                .Where(d => d != domainName)
                .OrderBy(d => d)
                .ToList();
            
            return combinedDomains;
        }
    }

    public class O365TypoSquatResult
    {
        public string Domain { get; set; } = string.Empty;
        public bool OnMicrosoftTest { get; set; }
        public bool SharePointTest { get; set; }
        public bool DomainTest { get; set; }
    }

    public class O365TypoSquatChecker
    {
        private readonly LookupClient _dnsClient;

        public O365TypoSquatChecker()
        {
            _dnsClient = new LookupClient();
        }

        public async Task<O365TypoSquatResult> CheckO365TypoSquat(string typoSquattedDomain)
        {
            var domainWithoutTld = typoSquattedDomain.Split('.').First();
            
            var onMicrosoftTask = CheckDnsResolution($"{domainWithoutTld}.onmicrosoft.com");
            var sharePointTask = CheckDnsResolution($"{domainWithoutTld}.sharepoint.com");
            var domainTask = CheckDnsResolution(typoSquattedDomain);
            
            await Task.WhenAll(onMicrosoftTask, sharePointTask, domainTask);
            
            return new O365TypoSquatResult
            {
                Domain = typoSquattedDomain,
                OnMicrosoftTest = onMicrosoftTask.Result,
                SharePointTest = sharePointTask.Result,
                DomainTest = domainTask.Result
            };
        }

        private async Task<bool> CheckDnsResolution(string domain)
        {
            try
            {
                var result = await _dnsClient.QueryAsync(domain, QueryType.A);
                return result.Answers.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var domains = TypoSquatGenerator.GenerateTypoSquatDomains("Google.com");
            var checker = new O365TypoSquatChecker();
            
            Console.WriteLine($"Generated {domains.Count} typosquat variations for Google.com");
            Console.WriteLine("Testing for O365 availability...\n");
            
            var tasks = domains.Select(async domain =>
            {
                var result = await checker.CheckO365TypoSquat(domain);
                
                if (result.OnMicrosoftTest || result.SharePointTest || result.DomainTest)
                {
                    Console.WriteLine($"Domain: {result.Domain}");
                    Console.WriteLine($"  OnMicrosoft: {result.OnMicrosoftTest}");
                    Console.WriteLine($"  SharePoint: {result.SharePointTest}");
                    Console.WriteLine($"  Domain: {result.DomainTest}");
                    Console.WriteLine();
                }
                
                return result;
            });
            
            await Task.WhenAll(tasks);
            Console.WriteLine("Scan complete.");
        }
    }
}