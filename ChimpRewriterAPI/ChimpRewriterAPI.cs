using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;

namespace ChimpRewriterAPI
{
    public static class ChimpRewriterAPI
    {
        public static readonly string ApiURL = "https://api.chimprewriter.com/";
        /// <summary>
        /// Application ID. Set this to a string (100 characters or less) to identify your application to the server
        /// </summary>
        public static string AppID = "";

        // Results are in JSON, so we will need a serialiser
        private static readonly JavaScriptSerializer _serialiser = new JavaScriptSerializer();

        /// <summary>
        /// Spins or rewrites an article. Standard spinning costs 1 credit per 5000 words. Advanced spinning costs 1 credit per 
        /// 500 words. A spin is “advanced” if any of the advanced parameters below are set to 1
        /// </summary>
        /// <param name="email">User’s Chimp Rewriter account email. Note that the user requires a Chimp Rewriter Pro subscription.</param>
        /// <param name="apiKey">User’s API key. Get one on the Chimp Rewriter User Management page.</param>
        /// <param name="text">Text to spin</param>
        /// <param name="quality">Synonym replacement quality: 5 – Best, 4 – Better, 3 – Good, 2 – Average, 1 – All</param>
        /// <param name="phraseQuality">Phrase replacement quality: 5 – Best, 4 – Better, 3 – Good, 2 – Average, 1 – All</param>
        /// <param name="posmatch">Required Part of Speech (POS) match for a spin: 3 – Full, 2 – Loose, 1 – Extremely Loose, 0 – None.</param>
        /// <param name="language">Two letter language code for the desired language.</param>
        /// <param name="rewrite">If true, results are returned as a rewritten article with no Spintax. Otherwise, an article with Spintax is returned. Note that with rewrite as 1, the original word will always be removed.</param>
        /// <param name="sentenceRewrite">Advanced - Set true to use artificial intelligence tools to automatically rewrite sentences</param>
        /// <param name="grammarCheck">Advanced - Set true to verify grammar on the result article for very high quality spin</param>
        /// <param name="replacePhrasesWithPhrases">Always replace phrases with equivalent phrases, regardless of quality. Results in a huge amount of spin but may not yield the best quality.</param>
        /// <param name="reorderParagraphs">Randomly reorder paragraphs</param>
        /// <param name="spinTidy">Runs a spin tidy pass over the result article. This fixes any common a/an type grammar mistakes and repeated words due to phrase spinning. Generally increases the quality of the article.</param>
        /// <param name="replaceFreq">Controls the percentage of words which are spun. 1 means every word will be eligible for spinning. 2 means every second word, 3 every third and so on.</param>
        /// <param name="excludeOriginal">Excludes the original word form the result article. Used for maximum uniqueness from the original article.</param>
        /// <param name="maxSyns">The maximum number of synonyms to be used for any one word or phrase.</param>
        /// <param name="spinWithinSpin">If true, if there is existing spin syntax in the content you send up, the API will spin any relevant content inside this syntax. If false, the API will skip over this content and only spin outside of existing syntax.</param>
        /// <param name="maxDepth">Define a maximum spin level depth in returned article. If set to 1, no nested spin will appear in the spun result. This paramater only matters if rewrite is false. Set to 0 or ignore for no limit on spin depth</param>
        /// <param name="instantUnique">Optionally runs an instant unique pass over the article. This replaces letters with characters that look like the original letter but have a different UTF8 value, passing copyscape 100% but garbling content to the search engines. It it recommended to protect keywords while using instant unique. 0 = No Instant Unique, 1 = Full Character Set, 2 = Best Character Set</param>
        /// <param name="protectedTerms">Words or phrases to protect from spin</param>
        /// <param name="tagProtect">Protects anything between any syntax you define. Separate start and end syntax with a pipe ‘|’ and separate multiple tags with a comma ‘,’. For example, you could protect anything in square brackets by setting tagprotect=[|]. You could also protect anything between “begin” and “end” by setting tagprotect=[|],begin|end</param>
        /// <returns></returns>
        public static APIResult ChimpRewrite(string email, string apiKey, string text, int? quality = null,
            int? phraseQuality = null, int? posmatch = null, string language = null, bool? rewrite = null, bool? sentenceRewrite = null,
            bool? grammarCheck = null, bool? replacePhrasesWithPhrases = null, bool? reorderParagraphs = null, bool? spinTidy = null,
            int? replaceFreq = null, bool? excludeOriginal = null, int? maxSyns = null, bool? spinWithinSpin = null, int? maxDepth = null,
            int? instantUnique = null, List<string> protectedTerms = null, string tagProtect = null)
        {
            if (string.IsNullOrEmpty(AppID)) return new APIResult(false, "AppID not set");
            if (string.IsNullOrEmpty(email)) return new APIResult(false, "No email specified");
            if (string.IsNullOrEmpty(apiKey)) return new APIResult(false, "No API Key specified");
            if (string.IsNullOrEmpty(text)) return new APIResult(false, "No text provided");

            var parameters = new NameValueCollection
                {
                    {"email", email},
                    {"apikey", apiKey},
                    {"aid", AppID},
                    {"text", text}
                };
            if (quality.HasValue) parameters.Add("quality", quality.Value.ToString(CultureInfo.InvariantCulture));
            if (phraseQuality.HasValue) parameters.Add("phrasequality", quality.Value.ToString(CultureInfo.InvariantCulture));
            if (posmatch.HasValue) parameters.Add("posmatch", posmatch.ToString());
            if (!string.IsNullOrEmpty(language)) parameters.Add("language", language);
            if (rewrite.HasValue) parameters.Add("rewrite", rewrite.Value ? "1" : "0");
            if (sentenceRewrite.HasValue) parameters.Add("sentencerewrite", sentenceRewrite.Value ? "1" : "0");
            if (grammarCheck.HasValue) parameters.Add("grammarcheck", grammarCheck.Value ? "1" : "0");
            if (replacePhrasesWithPhrases.HasValue) parameters.Add("replacephraseswithphrases", replacePhrasesWithPhrases.Value ? "1" : "0");
            if (reorderParagraphs.HasValue) parameters.Add("reorderparagraphs", reorderParagraphs.Value ? "1" : "0");
            if (spinTidy.HasValue) parameters.Add("spintidy", spinTidy.Value ? "1" : "0");
            if (replaceFreq.HasValue) parameters.Add("replacefrequency", replaceFreq.Value.ToString(CultureInfo.InvariantCulture));
            if (excludeOriginal.HasValue) parameters.Add("excludeoriginal", excludeOriginal.Value ? "1" : "0");
            if (maxSyns.HasValue) parameters.Add("maxsyns", maxSyns.Value.ToString(CultureInfo.InvariantCulture));
            if (spinWithinSpin.HasValue) parameters.Add("spinwithinspin", spinWithinSpin.Value ? "1" : "0");
            if (instantUnique.HasValue) parameters.Add("instantunique", instantUnique.Value.ToString(CultureInfo.InvariantCulture));
            if (protectedTerms != null && protectedTerms.Count > 0) parameters.Add("protectedterms", string.Join(",", protectedTerms.ToArray()));
            if (!string.IsNullOrEmpty(tagProtect)) parameters.Add("tagprotect", tagProtect);

            var jsonResult = makeRequest("ChimpRewrite", parameters);
            return _serialiser.Deserialize<APIResult>(jsonResult);
        }

        /// <summary>
        /// Generates an unspun doc from one with spintax. Optionally reorders paragraphs and removes original word.
        /// </summary>
        /// <param name="email">User’s Chimp Rewriter account email. Note that the user requires a Chimp Rewriter Pro subscription.</param>
        /// <param name="apiKey">User’s API key. Get one on the Chimp Rewriter User Management page.</param>
        /// <param name="text">Text to spin</param>
        /// <param name="dontIncludeOriginal">If true, the first first word in each set of spintax is not included in spun output. E.g. if this is found in the article: {word|syn1|syn2}, the result will only include either syn1 or syn2.</param>
        /// <param name="reorderParagraphs">If set to 1, paragraphs are randomly ordered in the result</param>
        /// <returns></returns>
        public static APIResult CreateSpin(string email, string apiKey, string text,
            bool? dontIncludeOriginal = null, bool? reorderParagraphs = null)
        {
            if (string.IsNullOrEmpty(AppID)) return new APIResult(false, "AppID not set");
            if (string.IsNullOrEmpty(email)) return new APIResult(false, "No email specified");
            if (string.IsNullOrEmpty(apiKey)) return new APIResult(false, "No API Key specified");
            if (string.IsNullOrEmpty(text)) return new APIResult(false, "No text provided");

            var parameters = new NameValueCollection();
            parameters.Add("email", email);
            parameters.Add("apikey", apiKey);
            parameters.Add("aid", AppID);
            parameters.Add("text", text);
            if (dontIncludeOriginal.HasValue) parameters.Add("dontincludeoriginal", dontIncludeOriginal.Value ? "1" : "0");
            if (reorderParagraphs.HasValue) parameters.Add("reorderparagraphs", reorderParagraphs.Value ? "1" : "0");
            var jsonResult = makeRequest("CreateSpin", parameters);
            return _serialiser.Deserialize<APIResult>(jsonResult);
        }

        /// <summary>
        /// Shows usage statistics for the API account
        /// </summary>
        /// <param name="email">User’s Chimp Rewriter account email. Note that the user requires a Chimp Rewriter Pro subscription.</param>
        /// <param name="apiKey">User’s API key. Get one on the Chimp Rewriter User Management page.</param>
        /// <returns></returns>
        public static APIStats Statistics(string email, string apiKey)
        {
            if (string.IsNullOrEmpty(email)) return new APIStats("No email specified");
            if (string.IsNullOrEmpty(apiKey)) return new APIStats("No API Key specified");

            var parameters = new NameValueCollection();
            parameters.Add("email", email);
            parameters.Add("apikey", apiKey);
            parameters.Add("aid", AppID);
            var jsonResult = makeRequest("Statistics", parameters);
            return _serialiser.Deserialize<APIStats>(jsonResult);
        }

        private static string makeRequest(string method, NameValueCollection values)
        {
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            return Encoding.UTF8.GetString(
                webClient.UploadValues(ApiURL + (ApiURL.EndsWith("/") ? "" : "/") + method, values));
        }
    }

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// API Result
    /// </summary>
    public class APIResult
    {
        /// <summary>
        /// Dictates success or failure. Possible values: “success” and “failure”
        /// </summary>
        public string status;
        /// <summary>
        /// If successful, the result of the spin. If failure, the reason for failure
        /// </summary>
        public string output;

        public APIResult()
        {
            status = "";
            output = "";
        }
        public APIResult(string status, string output)
        {
            this.status = status;
            this.output = output;
        }
        public APIResult(bool success, string output)
        {
            status = success ? "success" : "failure";
            this.output = output;
        }
    }

    /// <summary>
    /// API credit statistics
    /// </summary>
    public class APIStats
    {
        public int remainingthismonth = 0;
        public int prolimit = 0;
        public string proexpiry = "";
        public int apilimit = 0;
        public string apiexpiry = "";
        public string error = "";
        public int usedtoday = 0;
        public int usedthismonth = 0;
        public int usedever = 0;

        public APIStats()
        {
            
        }

        public APIStats(int remainingthismonth, int prolimit, string proexpiry, int apilimit, string apiexpiry, string error, int usedtoday, int usedthismonth, int usedever)
        {
            this.remainingthismonth = remainingthismonth;
            this.prolimit = prolimit;
            this.proexpiry = proexpiry;
            this.apilimit = apilimit;
            this.apiexpiry = apiexpiry;
            this.error = error;
            this.usedtoday = usedtoday;
            this.usedthismonth = usedthismonth;
            this.usedever = usedever;
        }
        public APIStats(string errorMessage)
        {
            error = errorMessage;
        }
    }

    // ReSharper restore InconsistentNaming
}
