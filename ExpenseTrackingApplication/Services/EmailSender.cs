using ExpenseTrackingApplication.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ExpenseTrackingApplication.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    // TODO: Add sendgrid configuration (here and website), check if everything works.
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SendGridClient(_configuration["SendGrid:ApiKey"]);
        var from = new EmailAddress("no-reply@yourapp.com", "Expense Tracker");
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
        var response = await client.SendEmailAsync(msg);
    }
}