using BookingSystem.Shared.DTOs;
using System.Net.Http.Json;

public class UserService {
    private readonly HttpClient _http;

    public UserService( HttpClient http ) {
        _http = http;
    }

    public async Task<List<UserResponse>> GetAllUsersAsync() {
        return await _http.GetFromJsonAsync<List<UserResponse>>( "/api/users" )
               ?? new List<UserResponse>();
    }

    public async Task UpdateUserAsync( int id, UpdateUserDto dto ) {
        await _http.PutAsJsonAsync( $"/api/users/{id}", dto );
    }

    public async Task DeleteUserAsync( int id ) {
        await _http.DeleteAsync( $"/api/users/{id}" );
    }
}
