﻿using System.Net;
using System.Security.Authentication;
using ArtHub.Attributes;
using ArtHub.Services;

namespace ArtHub.Controllers;

public class TestController(AuthService authService)
{
    [Authorize("user")]
    [Route("/test", "GET")]
    public async Task TestAuth(HttpListenerContext context, CancellationToken token)
    {
        try
        {
            await WebHelper.WriteJsonAsync("OK", context, token);
        }
        catch (AuthenticationException ex)
        {
            await WebHelper.ShowError(401, ex.Message, context, token);
        }
    }
}