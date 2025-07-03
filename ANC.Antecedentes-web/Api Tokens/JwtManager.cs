
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtManager
{
    private readonly string _secretKey;

    public JwtManager(string secretKey)
    {
        _secretKey = secretKey;
    }

    public (string token, double expiresInMinutes) GenerateTokenUserID(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("userId", userId) }),
            Expires = DateTime.UtcNow.AddMinutes(15), // Definir tiempo de expiración del token
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var expiresInMinutes = tokenDescriptor.Expires.Value.Subtract(DateTime.UtcNow).TotalMinutes;

        return (tokenHandler.WriteToken(token), expiresInMinutes);
    }

    public double ValidateTokenUserID(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        // Configurar los parámetros de validación del token
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true // Validar la vigencia del token
        };

        try
        {
            // Validar el token y obtener el principal
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Obtener la fecha de expiración del token
            var exp = validatedToken.ValidTo;

            // Calcular el tiempo restante en minutos
            var tiempoRestante = exp.Subtract(DateTime.UtcNow).TotalMinutes;

            // Devolver el tiempo restante en minutos
            return tiempoRestante;
        }
        catch (Exception ex)
        {
            return 0; // Devolver 0 si hay un error
        }
    }

}