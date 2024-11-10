using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TutorHelper;
using TutorHelper.Entities;
using TutorHelper.Entities.DbContext;
using TutorHelper.Middlewares;
using TutorHelper.Models.ConfigureClasses;
using TutorHelper.Models.DtoModels.CreateModels;
using TutorHelper.Models.DtoModels.UpdateModels;
using TutorHelper.Models.IdentityModels;
using TutorHelper.Services;
using TutorHelper.Validators.AccountModelsValidators;
using TutorHelper.Validators.LessonValidators;
using TutorHelper.Validators.StudentValidator;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja autoryzacji

// Dodawanie usług
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TutorHelperDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyTutorHelperConnectionString"))
);
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // Potwierdzenie konta
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.AllowedForNewUsers = true;

    //PasswordSettings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<TutorHelperDb>();
// Konfiguracja ustawień autoryzacji JWT
var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);

var googleAuthSetting = new GoogleAuthSettings();
builder.Configuration.GetSection("Authentication:Google").Bind(googleAuthSetting);
builder.Services.AddSingleton(googleAuthSetting);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin() // Zezwala na wszystkie domeny
                  .AllowAnyHeader() // Zezwala na wszystkie nagłówki
                  .AllowAnyMethod(); // Zezwala na wszystkie metody HTTP
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false; // Ustawienie na true w środowisku produkcyjnym
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
    };
})
.AddGoogle(options =>
{
    options.ClientId = googleAuthSetting.ClientId;
    options.ClientSecret = googleAuthSetting.ClientSecret;
});

builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>(); 
builder.Services.AddScoped<ICalendarAppService, CalendarAppService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IValidator<CreateStudentDto>, CreateStudentDtoValidator>();
builder.Services.AddScoped<IValidator<CreateLessonDtoWithStudent>, CreateLessonDtoWithStudentValidator>();
builder.Services.AddScoped<IValidator<CreateLessonDtoWoStudent>, CreateLessonDtoWoStudentValidator>();
builder.Services.AddScoped<IValidator<UpdateLessonWithoutStudentDto>, UpdateLessonWithoutStudentValidator>();
builder.Services.AddScoped<IValidator<UpdateLessonWithStudentDto>, UpdateLessonWithStudentValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordModel>, ChangePasswordValidator>();
builder.Services.AddScoped<IValidator<ForgotPasswordModel>, ForgotPasswordValidator>();
builder.Services.AddScoped<IValidator<LoginModel>, LoginModelValidator>();
builder.Services.AddScoped<IValidator<RegisterModel>, RegisterModelValidator>();
builder.Services.AddScoped<IValidator<ResetPasswordModel>, ResetPasswordValidator>();
builder.Services.AddScoped<IValidator<CreateNoteDto>, CreateNoteValidator>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IGoogleCalendarApi, GoogleCalendarApi>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ISummaryServices, SummaryServices>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
  
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); 

app.Run();