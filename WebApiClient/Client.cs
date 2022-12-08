using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ChessAvalonia.WebApiClient;

internal class Client : HttpClient
{
    public Client(string address)
    {
        BaseAddress = new Uri(address);

        DefaultRequestHeaders.Accept.Clear();
        DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
