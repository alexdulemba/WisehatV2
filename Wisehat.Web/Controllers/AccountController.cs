// Author: Alexander Dulemba
// Copyright 2023

// The code in this file has been adapted to Wisehat's context.
// It is based on Andrea Chiarelli's blog post: https://auth0.com/blog/what-is-blazor-tutorial-on-building-webapp-with-authentication/

using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Wisehat.Web.Controllers;

public class AccountController : ControllerBase
{
  [AllowAnonymous]
  [Route("/Login")]
  public async Task Login([FromQuery] string redirectUrl = "/home")
  {
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(redirectUrl)
        .Build();

    await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
  }

  [Authorize]
  [Route("/Logout")]
  public async Task Logout()
  {
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
        .WithRedirectUri("/")
        .Build();

    await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
  }
}
