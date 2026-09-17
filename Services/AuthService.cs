using System.Net;
using _2026RecetaFront.DTOs;

namespace _2026RecetaFront.Services
{
    public interface IAuthService
    {
        Task<RespuestaAutenticacion?> Login(CredencialesUsuario credencialesUsuario);
        Task<RespuestaAutenticacion?> Registrar(CredencialesUsuario credencialesUsuario);
        Task<RespuestaAutenticacion?> RenovarToken();
        Task Logout(); 
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient httpClient;
        private readonly ITokenService tokenService;
        private const string endpoint = "api/Cuentas";
        //TODO: Revisar el endpoint del backend

        public AuthService(HttpClient httpClient, ITokenService tokenService)
        {
            this.httpClient = httpClient;
            this.tokenService = tokenService;
        }

        public async Task<RespuestaAutenticacion?> Login(CredencialesUsuario credencialesUsuario)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{endpoint}/Login", credencialesUsuario);

                if (response.IsSuccessStatusCode)
                {
                    var respuesta =  await response.Content.ReadFromJsonAsync<RespuestaAutenticacion>();

                    if(respuesta != null)
                    {
                        await tokenService.GuardarToken(respuesta.Token, respuesta.Expiracion);
                        return respuesta;
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error en login: {error}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al hacer login: {ex.Message}");
                return null;
            }
        }

        public async Task Logout()
        {
            await tokenService.EliminarToken();
        }

        public async Task<RespuestaAutenticacion?> Registrar(CredencialesUsuario credenciales)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{endpoint}/registrar", credenciales);
                //TODO: Revisar el endpoint en el backen
                if (response.IsSuccessStatusCode)
                {
                    var respuesta = await response.Content.ReadFromJsonAsync<RespuestaAutenticacion>();
                    if(respuesta != null)
                    {
                        await tokenService.GuardarToken(respuesta.Token, respuesta.Expiracion);
                        return respuesta;
                    }
                }
                return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al registrar: {ex.Message}");
                return null;
            }
        } 

        public async Task<RespuestaAutenticacion?> RenovarToken()
        {
            try
            {
                var token = await tokenService.ObtenerToken();
                
                if(string.IsNullOrEmpty(token))
                    return null;

                //Agregar el token actual al header
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{endpoint}/RenovarToken");

                if (response.IsSuccessStatusCode)
                {
                    var respuesta = await response.Content.ReadFromJsonAsync<RespuestaAutenticacion>();

                    if(response != null)
                    {
                        await tokenService.GuardarToken(respuesta!.Token, respuesta.Expiracion);
                        return respuesta;
                    }
                }
                return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al renovar el token: {ex.Message}");
                return null;
            }
        }
    }
}