using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ChessAvalonia.Models;
using static ChessAvalonia.Services.MessengerService;
using System.Diagnostics.CodeAnalysis;

namespace ChessAvalonia.WebApiClient;
internal static class WebApiClientInvitationsCommands
{
    internal static async Task<ObservableCollection<Player>> GetPlayerInvitationsAsync(int localPlayerId)
    {
        ObservableCollection<Player> invitations = new();

        HttpResponseMessage response = await MessageMainPageViewModel.ClientInstance.GetAsync(
            $"api/invitations/{localPlayerId}");

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            invitations = JsonConvert.DeserializeObject<ObservableCollection<Player>>(jsonString);
        }

        return invitations;
    }

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    internal static async Task<Player> InvitePlayerAsync(int invitedId, Player localPlayer)
    {
        Player playerJson;

        var response = await MessageMainPageViewModel.ClientInstance.PostAsJsonAsync(
            $"api/invitations/{invitedId}", localPlayer);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            playerJson = JsonConvert.DeserializeObject<Player>(jsonString);
        }
        else
        {
            playerJson = new Player();
        }

        return playerJson;
    }

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    internal static async Task CancelInvitationAsync(int invitedId, Player localPlayer)
    {
        await MessageMainPageViewModel.ClientInstance.PutAsJsonAsync(
            $"api/invitations/cancel/{invitedId}", localPlayer);
    }
}
