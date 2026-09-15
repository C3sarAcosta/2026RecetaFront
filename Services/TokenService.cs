using Microsoft.JSInterop;

namespace _2026RecetaFront.Services
{
    public interface ITokenService
    {
        Task GuardarToken(string token, DateTime expiracion);
        Task<String?> ObtenerToken();
        Task<DateTime?> ObtenerExpiracion();
        Task<bool> EstaAutenticado();
        Task EliminarToken();
    }

    public class tokenService : ITokenService
    {
        private readonly IJSRuntime jsRuntime;
        private const string TOKEN_KEY = "authToken";
        private const string EXPIRACION_KEY = "tokenExpiracion";

        public tokenService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task EliminarToken()
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", EXPIRACION_KEY);
        }

        public async Task<bool> EstaAutenticado()
        {
            var token = await ObtenerToken();
            return !string.IsNullOrEmpty(token);
        }

        public async Task GuardarToken(string token, DateTime expiracion)
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, token);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", EXPIRACION_KEY, expiracion.ToString("o"));
            //Formato ISO 8601 (2024-12-15T10:30:00Z)
        }

        public async Task<DateTime?> ObtenerExpiracion()
        {
            try
            {
                var expiracionStr = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", EXPIRACION_KEY);
                if(string.IsNullOrEmpty(expiracionStr))
                    return null;
                
                if(DateTime.TryParse(expiracionStr, out var expiracion))
                    return expiracion;
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> ObtenerToken()
        {
            try
            {
                //1 leer el token del localstorage
                var token = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", TOKEN_KEY);

                //2 Si no tenemos token, retornamos null
                if(string.IsNullOrEmpty(token))
                    return null;

                //3 Verificar si el token expiro
                var expiracion = await ObtenerExpiracion();
                if(expiracion.HasValue && expiracion.Value < DateTime.UtcNow)
                {
                    await EliminarToken();
                    return null;
                }

                return token;
            }
            catch
            {
                return null;
            }
        }
    }
}