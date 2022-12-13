using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ChessAvalonia.Models;
using static ChessAvalonia.Services.MessengerService;
using System.Diagnostics.CodeAnalysis;

namespace ChessAvalonia.WebApiClient;
internal static class WebApiClientGamesCommands
{
    internal static async Task<OnlineGame> GetNewGame(int invitingId)
    {
        OnlineGame newOnlineGame = new();

        var response = await MessageMainPageViewModel.ClientInstance.GetAsync(
            $"api/games/{invitingId}");
        
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            newOnlineGame = JsonConvert.DeserializeObject<OnlineGame>(jsonString);
        }
        

        return newOnlineGame;
    }

    internal static async Task<OnlineGame> GetCurrentGame(int gameId)
    {
        OnlineGame currentOnlineGame = new();

        var response = await MessageMainPageViewModel.ClientInstance.GetAsync(
            $"api/games/current/{gameId}");

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            currentOnlineGame = JsonConvert.DeserializeObject<OnlineGame>(jsonString);
        }

        return currentOnlineGame;
    }

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    internal static async Task<OnlineGame> StartNewOnlineGameAsync(OnlineGame newOnlineGame)
    {
        OnlineGame responseNewOnlineGame = new();

        var response = await MessageMainPageViewModel.ClientInstance.PostAsJsonAsync(
            $"api/games", newOnlineGame);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            responseNewOnlineGame = JsonConvert.DeserializeObject<OnlineGame>(jsonString);
        }

        return responseNewOnlineGame;
    }

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    internal static async Task<OnlineGame> PutCurrentOnlineGame(int gameId, OnlineGame currentOnlineGame)
    {
        OnlineGame currentMove = new();

        var response = await MessageMainPageViewModel.ClientInstance.PutAsJsonAsync(
            $"api/games/current/{gameId}", currentOnlineGame);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            currentMove = JsonConvert.DeserializeObject<OnlineGame>(jsonString);
        }

        return currentMove;
    }

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    internal static async Task ResetWhiteInactiveCounterAsync(int gameId)
    {
        await MessageMainPageViewModel.ClientInstance.PutAsJsonAsync(
            $"api/games/current/counter/white/{gameId}", gameId);
    }

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    internal static async Task ResetBlackInactiveCounterAsync(int gameId)
    {
        await MessageMainPageViewModel.ClientInstance.PutAsJsonAsync(
            $"api/games/current/counter/black/{gameId}", gameId);
    }
}
