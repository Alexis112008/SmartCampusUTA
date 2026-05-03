using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task EnviarTokenRecuperacion(string emailDestino, string token)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress("SmartCampus UTA", _config["Email:From"]));
        mensaje.To.Add(MailboxAddress.Parse(emailDestino));
        mensaje.Subject = "Recuperación de contraseña";

        mensaje.Body = new TextPart("html")
        {
            Text = $@"
                <h2>SmartCampus UTA</h2>
                <p>Tu código de recuperación es:</p>
                <h1 style='letter-spacing:8px; color:#1a56db'>{token}</h1>
                <p>Este código expira en <strong>15 minutos</strong>.</p>
                <p>Si no solicitaste esto, ignora este mensaje.</p>
            "
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["Email:Host"], int.Parse(_config["Email:Port"]), false);
        await smtp.AuthenticateAsync(_config["Email:User"], _config["Email:Password"]);
        await smtp.SendAsync(mensaje);
        await smtp.DisconnectAsync(true);
    }
}