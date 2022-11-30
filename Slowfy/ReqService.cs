namespace App2;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.System;
using Model;
using User = Model.User;
using System;
using System.Text.Json;
using ABI.System;

public class ReqService
{
    public HttpClient client;
    public ReqService()
    {
        client = new HttpClient();
    }
    public async Task<string> Post(string url, Dictionary<string, string> values, string? bearer = null)
    {
        if (bearer != null) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        String res = await response.Content.ReadAsStringAsync();
        res = res.TrimEnd('"');
        res = res.TrimStart('"');
        return res;
    }

    public async Task<string> Get(string url, string? bearer = null)
    {
        if (bearer != null) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

        var response = await client.GetAsync(url);
       
        String res = await response.Content.ReadAsStringAsync();
        res = res.TrimEnd('"');
        res = res.TrimStart('"');
        return res;
    }



    public async Task<List<Track>> GetTracks()
    {
        //String res = await Get("https://localhost:7148/track");


        //var result = JsonNode.Parse(res);

        //Track[] bsObj = JsonConvert.DeserializeObject<Track[]>(res);
        string result2 = await new ReqService().Get("https://localhost:7148/tracks%22");

        List<Track> rec =
                JsonSerializer.Deserialize<List<Track>>(result2);
        return rec;
    }


}