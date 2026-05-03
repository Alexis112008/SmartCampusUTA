using SmartCampus.API.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class RecuperacionService
{
    private readonly SmartCampusDbContext _db;
    private readonly EmailService _emailService;

    public RecuperacionService(SmartCampusDbContext db, EmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<bool> SolicitarRecuperacion(string email)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email && u.Activo);

        if (usuario == null) return false;

        var random = new Random();
        var token = random.Next(100000, 999999).ToString();

        var tokensAnteriores = _db.TokensRecuperacion
            .Where(t => t.IdUsuario == usuario.IdUsuario && !t.Usado);
        foreach (var t in tokensAnteriores) t.Usado = true;

        _db.TokensRecuperacion.Add(new TokenRecuperacion
        {
            IdUsuario = usuario.IdUsuario,
            Token = token,
            FechaExpira = DateTime.Now.AddMinutes(15),
            Usado = false
        });

        await _db.SaveChangesAsync();

        await _emailService.EnviarTokenRecuperacion(email, token);
        return true;
    }

    public async Task<bool> VerificarToken(string token)
    {
        return await _db.TokensRecuperacion.AnyAsync(t =>
            t.Token == token &&
            !t.Usado &&
            t.FechaExpira > DateTime.Now);
    }

    public async Task<bool> CambiarPassword(string token, string nuevaPassword)
    {
        var registro = await _db.TokensRecuperacion
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t =>
                t.Token == token &&
                !t.Usado &&
                t.FechaExpira > DateTime.Now);

        if (registro == null) return false;

        // El frontend ya manda el hash SHA256, no hasheamos de nuevo
        registro.Usuario.PasswordHash = nuevaPassword;
        registro.Usado = true;

        await _db.SaveChangesAsync();
        return true;
    }
}