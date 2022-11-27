namespace App2;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> Get(string url, string? bearer = null)
    {
        if (bearer != null) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

        var response = await client.GetAsync(url);

        return await response.Content.ReadAsStringAsync();
    }
}