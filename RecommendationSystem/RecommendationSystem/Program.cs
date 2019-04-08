using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CallRequestResponseService
{

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    class Program
    {
        static string resultJson = string.Empty;

        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();

            InvokeLogicApps().Wait();
        }

        public static string EscapeStringValue(string value)
        {
            const char BACK_SLASH = '\\';
            const char SLASH = '/';
            const char DBL_QUOTE = '"';

            var output = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                switch (c)
                {
                    case SLASH:
                        output.AppendFormat("{0}{1}", BACK_SLASH, SLASH);
                        break;

                    case BACK_SLASH:
                        output.AppendFormat("{0}{0}", BACK_SLASH);
                        break;

                    case DBL_QUOTE:
                        output.AppendFormat("{0}{1}", BACK_SLASH, DBL_QUOTE);
                        break;

                    default:
                        output.Append(c);
                        break;
                }
            }

            return output.ToString();
        }

        static async Task InvokeLogicApps()
        {
            if (resultJson != string.Empty)
            {
                LogicAppInput[] array = new LogicAppInput[1];
                LogicAppInput app = new LogicAppInput();
                Rootobject rootobj = JsonConvert.DeserializeObject<Rootobject>(resultJson);
                for(int i=0;i<=5;i++)
                {
                    if(i==0)
                        app.User = rootobj.Results.output1.value.Values[0].GetValue(i).ToString();

                    if(i==1)
                        app.Item1 = rootobj.Results.output1.value.Values[0].GetValue(i).ToString();

                    if(i==2)
                        app.Item2 = rootobj.Results.output1.value.Values[0].GetValue(i).ToString();

                    if(i==3)
                        app.Item3 = rootobj.Results.output1.value.Values[0].GetValue(i).ToString();

                    if(i==4)
                        app.Item4 = rootobj.Results.output1.value.Values[0].GetValue(i).ToString();

                    if(i==5)
                        app.Item5 = rootobj.Results.output1.value.Values[0].GetValue(i).ToString();
                }

                array[0] = app;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://prod-14.centralus.logic.azure.com:443/workflows/aa02102791064545826333f733301572/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=EMNnMCAHfQTiyWgqmlmh40a87VVref6azmjcAIeqwCk");                    
                    HttpResponseMessage response = await client.PostAsJsonAsync<Array>("", array);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Result: {0}", result);
                    }
                    else
                    {
                        Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                        // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                        Console.WriteLine(response.Headers.ToString());

                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                    }
                }
            }
        }

        static async Task InvokeRequestResponseService()
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"User ID", "Product ID", "Rating"},
                                Values = new string[,] {  { "U002", "", "" }  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "1KU6cMEyzX7YKNrDgz2L1uHlP7JWCn5reCqnVQyOip9ceH8JwEMwDZBHZwKO34ISj3xJU/8t3T+1JRnlKQsMXw=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/42f97fe2c9c94a20a8d1846f20632b69/services/2935080116f04d2697bad9dc56c11435/execute?api-version=2.0&details=true");
                
                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Result: {0}", result);
                    resultJson = result;
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}
