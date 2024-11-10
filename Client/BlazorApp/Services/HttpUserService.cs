using System.Text.Json;
using APIContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("Users", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        
        return JsonSerializer.Deserialize<UserDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"Users/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
       var response = await client.GetFromJsonAsync<UserDto>($"Users/{id}");
       return response ?? new UserDto
       {
           UserName = "Unknown"
       };
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync() 
    { 
        HttpResponseMessage httpResponse = await client.GetAsync("Users"); 
        string response = await httpResponse.Content.ReadAsStringAsync(); 

        if (!httpResponse.IsSuccessStatusCode) 
        { 
            throw new Exception(response); 
        } 

        return JsonSerializer.Deserialize<IEnumerable<UserDto>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"Users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
}