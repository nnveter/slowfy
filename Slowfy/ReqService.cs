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

public class ReqService
{
    private HttpClient client;
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

    //public async Task<string> GetProfile(string token, string url)
    //{
    //    String res = await Get(url, token);
    //    User? result =
    //            JsonSerializer.Deserialize<User>(res);
    //    return result?.Name ?? "Error";
    //}
}