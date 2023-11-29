using Microsoft.JSInterop;

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

  internal static ValueTask<object> GetChildrenOfElementAsync(this IJSRuntime jsRuntime, string parent)
  {
    return jsRuntime.InvokeAsync<object>("document.getElementById", parent);
  }
}
