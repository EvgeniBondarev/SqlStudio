namespace SlqStudio.Extensions;

public static class MiddlewareExtensions
{
    public static void UseCustomStatusCodePages(this IApplicationBuilder app)
    {
        app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;
            var request = context.HttpContext.Request;

            if (response.StatusCode == 401)
            {
                var returnUrl = request.Path;
                response.Redirect($"/Auth/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
            else if (response.StatusCode == 403)
            {
                response.Redirect("/Home/AccessDenied");
            }
        });
    }
}