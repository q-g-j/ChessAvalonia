using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

using ChessAvalonia.Models;
using ChessAvalonia.ViewModels;
using ChessAvalonia.WebApiClient;
using System.Net.Http.Json;
using static ChessAvalonia.Services.MessengerService;
using ChessAvalonia.ViewModels.Pages.Main;
using ChessAvalonia.Helpers;

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

    internal static async Task ResetWhiteInactiveCounterAsync(int gameId)
    {
        await MessageMainPageViewModel.ClientInstance.PutAsJsonAsync(
            $"api/games/current/counter/white/{gameId}", gameId);
    }

    internal static async Task ResetBlackInactiveCounterAsync(int gameId)
    {
        await MessageMainPageViewModel.ClientInstance.PutAsJsonAsync(
            $"api/games/current/counter/black/{gameId}", gameId);
    }
}
