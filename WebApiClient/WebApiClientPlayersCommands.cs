using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

using ChessAvalonia.Models;
using ChessAvalonia.ViewModels;
using System.Net.Http.Json;
using static ChessAvalonia.Services.MessengerService;
using ChessAvalonia.ViewModels.Pages.Main;

namespace ChessAvalonia.WebApiClient;
internal static class WebApiClientPlayersCommands
{
    internal static async Task<ObservableCollection<Player>> GetAllPlayersAsync()
    {
        ObservableCollection<Player> playerList = new();

        HttpResponseMessage response = await MessageMainPageViewModel.ClientInstance.GetAsync(
            "api/players");

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            playerList = JsonConvert.DeserializeObject<ObservableCollection<Player>>(jsonString);
        }

        return playerList;
    }

    internal static async Task<Player> CreatePlayerAsync(Player player)
    {
        Player responsePlayer;

        var response = await MessageMainPageViewModel.ClientInstance.PostAsJsonAsync(
            "api/players", player);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            responsePlayer = JsonConvert.DeserializeObject<Player>(jsonString);
        }
        else
        {
            responsePlayer = new Player();
        }

        return responsePlayer;
    }

    internal static async Task ResetInactiveCounterAsync(int localPlayerId)
    {
        await MessageMainPageViewModel.ClientInstance.PutAsJsonAsync(
            $"api/players/{localPlayerId}", localPlayerId);
    }

    internal static async Task DeletePlayerAsync(int localPlayerId)
    {
        await MessageMainPageViewModel.ClientInstance.DeleteAsync(
            $"api/players/{localPlayerId}");
    }
}
