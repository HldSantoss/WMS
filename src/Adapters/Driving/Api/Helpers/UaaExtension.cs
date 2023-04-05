using Microsoft.IdentityModel.Tokens;

namespace Api.Helpers
{
    public static class UaaExtension
    {
        public static WebApplicationBuilder AddUaa(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = configuration.GetSection("UAA:Authority").Value;
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", configuration.GetSection("UAA:Scope").Value);
                });
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("View",
                    policy => policy.RequireClaim("Permission", "Read"));
                options.AddPolicy("Write",
                    policy => policy.RequireClaim("Permission", "Write"));
            });

            return builder;
        }
    }
}
