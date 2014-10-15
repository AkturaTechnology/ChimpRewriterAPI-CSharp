using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ChimpRewriterAPISample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Short identifier for your app, so we  know where the requests are coming from
            // This may be used for special deals, if your app attracts lots of
            // users to Chimp Rewriter
            ChimpRewriterAPI.ChimpRewriterAPI.AppID = "YOUR_APP_NAME_HERE";
            string email = "test@example.com";
            string apikey = "YOUR_API_KEY";
            string text;

            // Load in an article
            using (var fs = new FileStream("testarticle2.txt", FileMode.Open, FileAccess.Read))
            {
                var sr = new StreamReader(fs);
                text = sr.ReadToEnd();
                sr.Close();
            }

            // Optional Protected Terms
            var protectedTerms = new List<string>();
            protectedTerms.Add("You spin me right round baby, right round");
            
            //*******************
            // Chimp Rewrite! (Simple)
            //*******************

            var chimpResult = ChimpRewriterAPI.ChimpRewriterAPI.ChimpRewrite(email, apikey, text);
            if (chimpResult.status != "success")
            {
                Console.WriteLine("Chimp Rewrite Failed :(");
                return;
            }

            //*******************
            // Chimp Rewrite! (Complete)
            //*******************

            chimpResult = ChimpRewriterAPI.ChimpRewriterAPI.ChimpRewrite(email, apikey, text,
                                        4,                  // quality 1-5 (null for default=4)
                                        4,                  // phrase quality 1-5 (null for default=4)
                                        3,                  // pos match 0-4 (null for default=3)
                                        "en",               // language (null or empty for English ("en")
                                        false,              // rewrite - spin article before returning (no spintax in result)
                                        true,              // sentence rewrite - intelligently reorder sentence (advanced option)
                                        true,              // grammar check - intelligently check results (advanced option)
                                        false,              // replace phrases with phrases
                                        false,              // reorder paragraphs
                                        true,               // spin tidy (cleans up spin results and checks for improper use of langauge)
                                        1,                  // replace frequency - default 1
                                        false,              // exclude original word from spin
                                        10,                  // max replacements/synonyms for each word, default 10
                                        true,               // Spin within existing spin
                                        0,                  // max spin depth. 0 for unlimited. default 0
                                        0,                  // instant unique level. -1 = off (default), 0 = best, 1,2,3 lower quality
                                        protectedTerms,     // protected terms (null for none)
                                        ""                  // Tag protect. Project anything in between tags
                );
            if (chimpResult.status != "success")
            {
                Console.WriteLine("Chimp Rewrite Failed :(");
                return;
            }

            //*******************
            // Create Spin
            //*******************
            var chimpSpinResult = ChimpRewriterAPI.ChimpRewriterAPI.CreateSpin(email, apikey, chimpResult.output);
            if (chimpSpinResult.status != "success")
            {
                Console.WriteLine("Create Spin Failed :(");
                return;
            }

            //*******************
            // Statistics
            //*******************
            var statistics = ChimpRewriterAPI.ChimpRewriterAPI.Statistics(email, apikey);
            if (!string.IsNullOrEmpty(statistics.error))
                Console.WriteLine("Getting statistics failed because of: " + statistics.error);
            else 
                Console.WriteLine("Remaining queries this month: " + 
                    statistics.remainingthismonth.ToString(CultureInfo.InvariantCulture));

           
            
        }
    }
}
