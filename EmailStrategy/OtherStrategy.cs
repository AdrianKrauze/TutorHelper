using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.EmailStrategy
{
    public class OtherStrategy : IEmailStrategy
    {
        public string MakeBodyContent(CreateEmailDto dto)
        {
            return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    color: #333;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                .email-container {{
                    width: 100%;
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                    background-color: #fff;
                    border-radius: 8px;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                }}
                h2 {{
                    color: #4CAF50;
                    font-size: 24px;
                    text-align: center;
                }}
                .email-body {{
                    font-size: 16px;
                    line-height: 1.5;
                    color: #555;
                }}
                .email-body p {{
                    margin: 10px 0;
                }}
                .email-body strong {{
                    color: #333;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 12px;
                    text-align: center;
                    color: #aaa;
                }}
                .footer a {{
                    color: #4CAF50;
                    text-decoration: none;
                }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <h2>Kontakt - Inne Zapytanie</h2> 
                <div class='email-body'>
                    <p><strong>Imię i nazwisko:</strong> {dto.Name} {dto.LastName}</p>
                    <p><strong>E-mail:</strong> {dto.Email}</p>
                    <p><strong>Telefon:</strong> {dto.PhoneNumber}</p>
                    <p><strong>Treść wiadomości:</strong></p>
                    <p>{dto.Content}</p>
                </div>
                <div class='footer'>
                    <p>Dziękujemy za kontakt z nami! <br/> Jeśli masz jakiekolwiek pytania, prosimy o kontakt.</p>
                    <p>Zapraszamy ponownie!</p>
                </div>
            </div>
        </body>
        </html>
    ";
        }


    }
}
