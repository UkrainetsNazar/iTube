using Shared.Api.Extensions;
using AuthService.Application.Commands.ChangePassword;
using AuthService.Application.Commands.ConfirmEmail;
using AuthService.Application.Commands.ForgotPassword;
using AuthService.Application.Commands.Login;
using AuthService.Application.Commands.Logout;
using AuthService.Application.Commands.RefreshToken;
using AuthService.Application.Commands.Register;
using AuthService.Application.Commands.ResetPassword;
using AuthService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] AuthDto dto,
        CancellationToken ct)
    {
        var result = await sender.Send(new RegisterCommand(dto), ct);
        return result.ToActionResult();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailDto dto,
        CancellationToken ct)
    {
        var result = await sender.Send(new ConfirmEmailCommand(dto.Email, dto.Token), ct);
        return result.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] AuthDto dto,
        CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(dto), ct);
        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenDto dto,
        CancellationToken ct)
    {
        var result = await sender.Send(new RefreshTokenCommand(dto.RefreshToken), ct);
        return result.ToActionResult();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenDto dto,
        CancellationToken ct)
    {
        var result = await sender.Send(new LogoutCommand(dto.RefreshToken), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto dto,
        CancellationToken ct)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value!;
        var result = await sender.Send(new ChangePasswordCommand(email, dto.OldPassword, dto.NewPassword), ct);
        return result.ToActionResult();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDto dto,
        CancellationToken ct)
    {
        var result = await sender.Send(new ForgotPasswordCommand(dto.Email), ct);
        return result.ToActionResult();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto dto,
        CancellationToken ct)
    {
        var result = await sender.Send(new ResetPasswordCommand(dto.Email, dto.Token, dto.NewPassword), ct);
        return result.ToActionResult();
    }
}