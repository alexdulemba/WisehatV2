﻿// Author: Alexander Dulemba
// Copyright 2023

using Wisehat.Domain.Entities;

namespace Wisehat.Domain.Infrastructure;

public interface IDatabaseService
{
  Task<IEnumerable<WebProject>> GetWebProjectsByProfileIdAsync(string profileId, CancellationToken token);

  Task<WebProject?> GetWebProjectAsync(Guid projectId, CancellationToken token);

  Task CreateWebProjectAsync(WebProject webProject, CancellationToken token);

  Task<bool> UpdateWebProjectAsync(WebProject webProject, CancellationToken token);

  Task<bool> UpdateWebProjectTitleAsync(Guid projectId, string newTitle, CancellationToken token);

  Task<bool> UpdateWebProjectWidgetsAsync(Guid projectId, List<Widget> widgets, CancellationToken token);

  Task<bool> DeleteWebProjectAsync(Guid projectId, CancellationToken token);
}
