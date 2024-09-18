using System.Text.Json;
using RepositoryContracts;
using Entities;

namespace FileRepositories;

using RepositoryContracts;
using Entities;
using System.Text.Json;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }
    
    private async Task<List<User>> LoadUsersAsync()
    {
        try
        {
            string usersAsJson = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>();
        }
        catch (JsonException e)
        {
            throw new FileLoadException("Could not deserialize users.json.", e);
        }
        catch (Exception e)
        {
            throw new FileLoadException("An error occured while loading users.", e);
        }

    }
    
    private async Task SaveUsersAsync(List<User> users)
    {
        try
        {
            string usersAsJson = JsonSerializer.Serialize(users);
            await File.WriteAllTextAsync(filePath, usersAsJson);
        }
        catch (Exception e)
        {
            throw new FileLoadException("An error occured while saving users.", e);
        }
    }

    public async Task<User> AddAsync(User user)
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson) !;
        int maxId = users.Count > 0 ? users.Max(x => x.Id) : 1;
        user.Id = maxId + 1;
        users.Add(user);
        usersAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, usersAsJson);
        return user;
    }

    public async Task DeleteAsync(int userId)
    {
        var users = await LoadUsersAsync();
        var user = users.SingleOrDefault(u => u.Id == userId);
        
        if (user != null)
        {
            users.Remove(user);
            await SaveUsersAsync(users);
        }
        else
        {
            throw new InvalidOperationException($"User with id {userId} was not found");
        }
    }

    public async Task<User?> GetSingleAsync(int userId)
    {
        var users = await LoadUsersAsync();
        var user = users.FirstOrDefault(u => u.Id == userId);

        if (user != null)
        {
            return user;
        }
        else
        {
            throw new InvalidOperationException($"User with id {userId} was not found");
        }
    }

    public IQueryable<User> GetManyAsync()
    {
        try
        {
            string usersAsJson = File.ReadAllTextAsync(filePath).Result;
            List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson) !;
            return users.AsQueryable();
        }
        catch (JsonException e)
        {
            throw new FileLoadException("Could not deserialize users.json.", e);
        }
        catch (Exception e)
        {
            throw new FileLoadException("An error occured while loading users.", e);
        }
    }

    public async Task UpdateAsync(User user)
    {
        var users = await LoadUsersAsync();
        var existingUser = users.SingleOrDefault(u => u.Id == user.Id);
        
        if (existingUser != null)
        {
            existingUser.Username = user.Username;
            existingUser.Password = user.Password;
            
            await SaveUsersAsync(users);
        }
        else
        {
            throw new InvalidOperationException("User not found.");
        }
    }
}
