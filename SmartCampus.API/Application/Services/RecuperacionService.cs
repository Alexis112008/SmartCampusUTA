using SmartCampus.API.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class RecuperacionService
{
    private readonly SmartCampusDbContext _db;
    private readonly EmailService _emailService;
    // cambio prueba git
    public RecuperacionService(SmartCampusDbContext db, EmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    // PASO A: Solicitar recuperación → genera token y envía email
    public async Task<bool> SolicitarRecuperacion(string email)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email && u.Activo);

        if (usuario == null) return false; // No revelar si existe o no (seguridad)

        // Genera token de 6 dígitos numéricos (fácil de escribir)
        var random = new Random();
        var token = random.Next(100000, 999999).ToString();

        // Invalida tokens anteriores del mismo usuario
        var tokensAnteriores = _db.TokensRecuperacion
            .Where(t => t.IdUsuario == usuario.IdUsuario && !t.Usado);
        foreach (var t in tokensAnteriores) t.Usado = true;

        // Guarda el nuevo token
        _db.TokensRecuperacion.Add(new TokenRecuperacion
        {
            IdUsuario = usuario.IdUsuario,
            Token = token,
            FechaExpira = DateTime.Now.AddMinutes(15),
            Usado = false
        });

        await _db.SaveChangesAsync();

        // Envía el email
        await _emailService.EnviarTokenRecuperacion(email, token);
        return true;
    }

    // PASO B: Verificar que el token es válido
    public async Task<bool> VerificarToken(string token)
    {
        return await _db.TokensRecuperacion.AnyAsync(t =>
            t.Token == token &&
            !t.Usado &&
            t.FechaExpira > DateTime.Now);
    }

    // PASO C: Cambiar contraseña usando el token
    public async Task<bool> CambiarPassword(string token, string nuevaPassword)
    {
        var registro = await _db.TokensRecuperacion
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t =>
                t.Token == token &&
                !t.Usado &&
                t.FechaExpira > DateTime.Now);

        if (registro == null) return false;

        // Hashear nueva contraseña con SHA256 (igual que en tu registro)
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(nuevaPassword));
        var hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        registro.Usuario.PasswordHash = hash;
        registro.Usado = true; // Invalida el token

        await _db.SaveChangesAsync();
        return true;
    }
}