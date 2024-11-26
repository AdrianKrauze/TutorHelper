using TutorHelper.EmailStrategy;
using TutorHelper.Models.DtoModels.CreateModels;

public static class EmailTemplateHelper
{
    public static string GenerateHtmlTemplateForAdmin(string title, CreateEmailDto dto)
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
                <h2>Kontakt w sprawie {title}</h2>
                <div class='email-body'>
                   <p><strong>Imię i nazwisko:</strong> {dto.Name} {dto.LastName}</p>
            <p><strong>E-mail:</strong> {dto.Email}</p>
            <p><strong>Telefon:</strong> {dto.PhoneNumber}</p>
            <p><strong>Treść wiadomości:</strong></p>
            <p>{dto.Content}</p>
                </div>
                <div class='footer'>
                    <p>Email został wysłany {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}.</p>
                  
                </div>
            </div>
        </body>
        </html>";
    }
    public static string GenerateHtmlTemplateResponse(CreateEmailDto dto)
    {
        return $@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                color: #333;
                background-color: #f9f9f9;
                margin: 0;
                padding: 0;
            }}
            .email-container {{
                width: 100%;
                max-width: 600px;
                margin: 20px auto;
                padding: 20px;
                background-color: #ffffff;
                border-radius: 10px;
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            }}
            h2 {{
                color: #2196F3;
                font-size: 24px;
                text-align: center;
                margin-bottom: 20px;
            }}
            .email-body {{
                font-size: 16px;
                line-height: 1.6;
                color: #555;
            }}
            .email-body p {{
                margin: 10px 0;
            }}
            .email-body strong {{
                color: #333;
            }}
            .email-body .greeting {{
                font-size: 18px;
                color: #4CAF50;
                margin-bottom: 20px;
                font-weight: bold;
            }}
            .footer {{
                margin-top: 30px;
                font-size: 12px;
                text-align: center;
                color: #999;
                border-top: 1px solid #ddd;
                padding-top: 15px;
            }}
            .footer a {{
                color: #2196F3;
                text-decoration: none;
            }}
            .btn {{
                display: inline-block;
                margin: 20px 0;
                padding: 10px 20px;
                background-color: #4CAF50;
                color: #ffffff;
                text-decoration: none;
                border-radius: 5px;
                font-size: 16px;
                text-align: center;
            }}
            .btn:hover {{
                background-color: #45a049;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <h2>Dziękujemy za kontakt!</h2>
            <div class='email-body'>
                <p class='greeting'>Twoja wiadomość została pomyślnie wysłana!</p>
                <p>Drogi/a <strong>{dto.Name} {dto.LastName}</strong>,</p>
                <p>Dziękujemy za wiadomość dotyczącą. Nasz zespół przetworzy Twoje zgłoszenie najszybciej, jak to możliwe.</p>
                <p>Oto podsumowanie Twojej wiadomości:</p>
                <p><strong>E-mail:</strong> {dto.Email}</p>
                <p><strong>Telefon:</strong> {dto.PhoneNumber}</p>
                <p><strong>Treść wiadomości:</strong></p>
                <p>{dto.Content}</p>
                <p>Jeśli potrzebujesz pilnej pomocy, skontaktuj się z nami bezpośrednio pod naszym numerem telefonu lub adresem e-mail.</p>
            </div>
            <div class='footer'>
                <p>Ten e-mail został wygenerowany automatycznie {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}.</p>
                <p>W razie dodatkowych pytań odwiedź naszą <a href='#'>stronę kontaktową</a>.</p>
                <p>&copy; {DateTime.Now.Year} Twoja Firma. Wszystkie prawa zastrzeżone.</p>
            </div>
        </div>
    </body>
    </html>";
    }


    public static string ReturnSubject(CreateEmailDto dto) => dto.EmailCase switch
    {
        EmailCase.PageError => $"Strona zawiera błąd od: {dto.Name} {dto.LastName}",
        EmailCase.ProblemWithLessons => $"Problem z lekcjami od: {dto.Name} {dto.LastName}",
        EmailCase.ProblemWithPayments => $"Problem z płatnościami od: {dto.Name} {dto.LastName}" ,
        EmailCase.ProblemWithStudents => $"Problem ze studentami od: {dto.Name} {dto.LastName}",
        EmailCase.Subscribe => $"Subskrypcja od: {dto.Name} {dto.LastName}",
        _ => $"Inny temat od: {dto.Name} {dto.LastName}"
    };

    

}
