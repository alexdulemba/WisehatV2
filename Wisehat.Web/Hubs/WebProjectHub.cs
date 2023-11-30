using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Wisehat.Web.Hubs;

public class WebProjectHub : Hub<WebProjectCommands>
{
  public async Task JoinWebProjectGroup(Guid projectId)
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, projectId.ToString());
    Debug.WriteLine($"new connection {Context.ConnectionId} added to group {projectId}");
  }

  public async Task LeaveWebProjectGroup(Guid projectId)
  {
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId.ToString());
    Debug.WriteLine($"removed connection from group");
  }

  public async Task UpdateWebProjectTitle(Guid projectId, string newProjectTitle)
  {
    Debug.WriteLine($@"project title updated to ""{newProjectTitle}"", sending new info to other open editors...");
    await Clients.OthersInGroup(projectId.ToString())
      .UpdateWebProjectTitle(projectId, newProjectTitle);
  }

  public void ServerReceiveWidgets(object[] widgets)
  {
    Debug.WriteLine(widgets[0]); 
  }
}

public interface WebProjectCommands
{
  Task UpdateWebProjectTitle(Guid id, string message);
}
