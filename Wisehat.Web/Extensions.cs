﻿using Microsoft.JSInterop;

namespace Wisehat.Web;

public static class Extensions
{
  internal static ValueTask AlertAsync(this IJSRuntime jsRuntime, string message)
  {
    return jsRuntime.InvokeVoidAsync("alert", message);
  }

  internal static ValueTask OpenInNewTabAsync(this IJSRuntime jsRuntime, string url)
  {
    return jsRuntime.InvokeVoidAsync("open", url, "_blank");
  }

  internal static ValueTask<bool> ConfirmAsync(this IJSRuntime jsRuntime, string message)
  {
    return jsRuntime.InvokeAsync<bool>("confirm", message);
  }

  internal static ValueTask<IJSObjectReference> ImportModuleAsync(this IJSRuntime jsRuntime, string modulePath)
  {
    return jsRuntime.InvokeAsync<IJSObjectReference>("import", modulePath);
  }
}
