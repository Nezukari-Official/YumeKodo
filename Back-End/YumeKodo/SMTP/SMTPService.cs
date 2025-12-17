using System.Net.Mail;
using System.Net;


namespace YumeKodo.SMTP;
public class SMTPService
{
    public void Verification(string ToAddress, string Subject, string VerificationCode)
    {
        string EmailSender = "thatonejouji@gmail.com";
        string SenderPassword = "zkiy cfhe tfie azvo";

        string Body = @"
<html>
  <head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
    <style>
      body {
        margin: 0;
        padding: 0;
        background-color: #1e1e1e;
        font-family: monospace;
      }
      @media only screen and (max-width: 600px) {
        .email-container {
          width: 100% !important;
          padding: 10px !important;
        }
        .email-header h1,
        .email-content h1 {
          font-size: 20px !important;
        }
        .email-footer {
          font-size: 11px !important;
        }
      }
    </style>
  </head>
  <body>
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"">
      <tr>
        <td align=""center"">
          <table
            class=""email-container""
            width=""600""
            cellpadding=""0""
            cellspacing=""0""
            border=""0""
            style=""max-width: 600px; width: 100%; background-color: #333; border: 1px solid #fff; border-radius: 8px; margin: 20px auto; padding: 20px;""
          >
            <tr>
              <td align=""center"" style=""background-color: #8d0000; color: white; border-radius: 8px 8px 8px 8px; padding: 10px;"">
                <h1 style=""margin: 0;"">Confirmation Code</h1>
              </td>
            </tr>
            <tr>
              <td align=""center"" style=""padding: 20px;"">
                <h1 style=""color: #fff; margin: 0;"">{VerificationCode}</h1>
              </td>
            </tr>
            <tr>
              <td align=""center"" style=""color: #a7a7a7; font-size: 12px; padding: 10px;"">
                <p style=""margin: 0;"">YumeKōdo Team</p>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>
        ";


        Body = Body.Replace("{VerificationCode}", VerificationCode);

        MailMessage Mail = new MailMessage();
        Mail.From = new MailAddress(EmailSender);
        Mail.To.Add(ToAddress);
        Mail.Subject = Subject;
        Mail.Body = Body;
        Mail.IsBodyHtml = true;


        var SmtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(EmailSender, SenderPassword)
        };
        SmtpClient.Send(Mail);
    }



    public void PasswordResetCode(string ToAddress, string Subject, string PasswordResetCode)
    {
        string EmailSender = "thatonejouji@gmail.com";
        string SenderPassword = "zkiy cfhe tfie azvo";

        string Body = @"
<html>
  <head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
    <style>
      body {
        margin: 0;
        padding: 0;
        background-color: #1e1e1e;
        font-family: monospace;
      }
      @media only screen and (max-width: 600px) {
        .email-container {
          width: 100% !important;
          padding: 10px !important;
        }
        .email-header h1,
        .email-content h1 {
          font-size: 20px !important;
        }
        .email-footer {
          font-size: 11px !important;
        }
      }
    </style>
  </head>
  <body>
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"">
      <tr>
        <td align=""center"">
          <table
            class=""email-container""
            width=""600""
            cellpadding=""0""
            cellspacing=""0""
            border=""0""
            style=""max-width: 600px; width: 100%; background-color: #333; border: 1px solid #fff; border-radius: 8px; margin: 20px auto; padding: 20px;""
          >
            <tr>
              <td align=""center"" style=""background-color: #8d0000; color: white; border-radius: 8px 8px 8px 8px; padding: 10px;"">
                <h1 style=""margin: 0;"">Password Reset Code</h1>
              </td>
            </tr>
            <tr>
              <td align=""center"" style=""padding: 20px;"">
                <h1 style=""color: #fff; margin: 0;"">{PasswordResetCode}</h1>
              </td>
            </tr>
            <tr>
              <td align=""center"" style=""color: #a7a7a7; font-size: 12px; padding: 10px;"">
                <p style=""margin: 0;"">YumeKōdo Team</p>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>
        ";


        Body = Body.Replace("{PasswordResetCode}", PasswordResetCode);

        MailMessage Mail = new MailMessage();
        Mail.From = new MailAddress(EmailSender);
        Mail.To.Add(ToAddress);
        Mail.Subject = Subject;
        Mail.Body = Body;
        Mail.IsBodyHtml = true;


        var SmtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(EmailSender, SenderPassword)
        };
        SmtpClient.Send(Mail);
    }
}
