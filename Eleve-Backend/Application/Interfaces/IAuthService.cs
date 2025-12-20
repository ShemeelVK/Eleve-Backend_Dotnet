using Eleve_Backend.Application.DTOs.Auth;
using Eleve_Backend.Domain.Entities;

namespace Eleve_Backend.Application.Interfaces
{
    //This is an interface that says "I need a way to Authenticate" bur it doesn't care if it's JWt or Cookie.
    public interface IAuthService
    {
        //this returns a string (the token) if successful,oir null if failed
        Task<String> Login(LoginRequestDto request);
        Task<String> Register(RegisterRequestDto request);
    }
}
