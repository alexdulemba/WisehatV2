// Author: Alexander Dulemba
// Copyright 2023

using FluentValidation;
using Wisehat.Domain.Entities;

namespace Wisehat.Domain.Validators;

public class WebProjectValidator : AbstractValidator<WebProject>
{
  public WebProjectValidator()
  {
    RuleFor(wp => wp.Id).NotEmpty();
    RuleFor(wp => wp.ProfileId).NotEmpty();
    RuleFor(wp => wp.Name).NotEmpty().MaximumLength(50);
    RuleFor(wp => wp.Description).MaximumLength(250);
  }
}
