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

        private static readonly string[] CommonTlds = new[]
        {
            "com", "net", "org", "info", "biz", "co", "io", "me", "tv", "cc",
            "tk", "ml", "ga", "cf", "ly", "sh", "eu", "us", "uk", "de", 
            "fr", "it", "es", "nl", "pl", "ru", "cn", "jp", "in", "br",
            "au", "ca", "mx", "ar", "cl", "co", "pe", "ve", "za", "ng",
            "ke", "ma", "eg", "il", "tr", "sa", "ae", "pk", "bd", "lk",
            "com.au", "net.au", "org.au", "edu.au", "gov.au", "asn.au",
            "co.uk", "org.uk", "me.uk", "ltd.uk", "plc.uk", "net.uk",
            "co.nz", "net.nz", "org.nz", "ac.nz", "school.nz", "govt.nz",
            "com.br", "net.br", "org.br", "gov.br", "edu.br", "mil.br",
            "co.za", "net.za", "org.za", "gov.za", "edu.za", "ac.za",
            "co.in", "net.in", "org.in", "gov.in", "edu.in", "ac.in",
            "com.cn", "net.cn", "org.cn", "gov.cn", "edu.cn", "ac.cn",
            "co.jp", "ne.jp", "or.jp", "go.jp", "ac.jp", "ad.jp",
            "com.mx", "net.mx", "org.mx", "gob.mx", "edu.mx", "mil.mx",
            "com.sg", "net.sg", "org.sg", "gov.sg", "edu.sg", "per.sg",
            "com.my", "net.my", "org.my", "gov.my", "edu.my", "mil.my",
            "com.hk", "net.hk", "org.hk", "gov.hk", "edu.hk", "idv.hk",
            "com.tw", "net.tw", "org.tw", "gov.tw", "edu.tw", "idv.tw",
            "co.kr", "ne.kr", "or.kr", "go.kr", "ac.kr", "mil.kr",
            "com.tr", "net.tr", "org.tr", "gov.tr", "edu.tr", "mil.tr",
            "co.il", "net.il", "org.il", "gov.il", "ac.il", "idf.il"
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

        public static List<string> GenerateTypoSquatDomainsWithTldVariations(string domainName)
        {
            var parts = domainName.Split('.');
            var domain = parts.First();
            
            var variations = new List<string>();
            
            // Generate domain variations (without TLD)
            var domainVariations = new List<string> { domain };
            
            // Homoglyph variations
            for (int i = 0; i < 29; i++)
            {
                if (ReplacementGlyphs.ContainsKey(i))
                {
                    var (original, replacement) = ReplacementGlyphs[i];
                    var newDomain = domain.Replace(original, replacement);
                    
                    domainVariations.Add(newDomain);
                    domainVariations.Add(newDomain + "s");
                    domainVariations.Add(newDomain + "a");
                    domainVariations.Add(newDomain + "t");
                    domainVariations.Add(newDomain + "en");
                }
            }
            
            // Bit squatting and omission
            for (int i = 0; i < domain.Length; i++)
            {
                // Character omission
                if (i + 1 < domain.Length)
                {
                    var omitted = domain.Substring(0, i) + domain.Substring(i + 2);
                    domainVariations.Add(omitted);
                }
                
                // Character transposition
                if (i + 2 < domain.Length)
                {
                    var transposed = domain.Substring(0, i) + 
                                   domain[i + 1] + 
                                   domain[i] + 
                                   domain.Substring(i + 2);
                    domainVariations.Add(transposed);
                }
            }
            
            // Plurals
            domainVariations.Add(domain + "s");
            domainVariations.Add(domain + "a");
            domainVariations.Add(domain + "en");
            domainVariations.Add(domain + "t");
            
            // Combine all domain variations with all TLDs
            foreach (var domainVar in domainVariations.Distinct())
            {
                foreach (var tld in CommonTlds)
                {
                    variations.Add($"{domainVar}.{tld}");
                }
            }
            
            // Remove original domain and duplicates
            var combinedDomains = variations
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

    public class O365TypoSquatService
    {
        private readonly O365TypoSquatChecker _checker;

        public O365TypoSquatService()
        {
            _checker = new O365TypoSquatChecker();
        }

        public async Task<O365TypoSquatResult> TestDomainAsync(string domain)
        {
            return await _checker.CheckO365TypoSquat(domain);
        }

        public async Task<List<O365TypoSquatResult>> TestDomainsAsync(IEnumerable<string> domains)
        {
            var tasks = domains.Select(domain => _checker.CheckO365TypoSquat(domain));
            return (await Task.WhenAll(tasks)).ToList();
        }

        public List<string> GenerateTypoSquatVariations(string domainName)
        {
            return TypoSquatGenerator.GenerateTypoSquatDomains(domainName);
        }

        public List<string> GenerateTypoSquatVariationsWithTlds(string domainName)
        {
            return TypoSquatGenerator.GenerateTypoSquatDomainsWithTldVariations(domainName);
        }

        public async Task<List<O365TypoSquatResult>> GenerateAndTestTypoSquatsAsync(string domainName)
        {
            var variations = GenerateTypoSquatVariations(domainName);
            return await TestDomainsAsync(variations);
        }

        public async Task<List<O365TypoSquatResult>> GenerateAndTestTypoSquatsWithTldsAsync(string domainName)
        {
            var variations = GenerateTypoSquatVariationsWithTlds(domainName);
            return await TestDomainsAsync(variations);
        }
    }
}