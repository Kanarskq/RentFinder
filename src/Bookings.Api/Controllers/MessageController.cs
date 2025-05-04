using Bookings.Api.Controllers.Request.Messages;
using Bookings.Api.Infrastructure.Messages;
using Bookings.Api.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookings.Api.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly ILogger<MessageController> _logger;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;

    public MessageController(
        ILogger<MessageController> logger,
        IMessageService messageService,
        IUserService userService)
    {
        _logger = logger;
        _messageService = messageService;
        _userService = userService;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetAllConversations()
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(auth0Id))
        {
            return Unauthorized("User not authenticated");
        }

        var user = await _userService.GetUserByAuth0IdAsync(auth0Id);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var conversations = await _messageService.GetAllConversationsAsync(user.Id);
        return Ok(conversations);
    }

    [HttpGet("conversation/{otherUserId}")]
    public async Task<IActionResult> GetConversation(int otherUserId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(auth0Id))
        {
            return Unauthorized("User not authenticated");
        }

        var user = await _userService.GetUserByAuth0IdAsync(auth0Id);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var messages = await _messageService.GetConversationAsync(user.Id, otherUserId);

        await _messageService.MarkConversationAsReadAsync(user.Id, otherUserId);

        return Ok(messages);
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(auth0Id))
        {
            return Unauthorized("User not authenticated");
        }

        var sender = await _userService.GetUserByAuth0IdAsync(auth0Id);
        if (sender == null)
        {
            return NotFound("Sender user not found");
        }

        var receiver = await _userService.GetUserAsync(request.ReceiverId);
        if (receiver == null)
        {
            return NotFound("Receiver user not found");
        }

        var message = await _messageService.SendMessageAsync(
            sender.Id,
            request.ReceiverId,
            request.Content,
            request.PropertyId);

        return Ok(message);
    }

    [HttpPut("read/{conversationId}")]
    public async Task<IActionResult> MarkAsRead(string conversationId)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(auth0Id))
        {
            return Unauthorized("User not authenticated");
        }

        var user = await _userService.GetUserByAuth0IdAsync(auth0Id);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Parse the conversation ID to get the other user's ID
        var otherUserId = int.Parse(conversationId.Split('-')[1]);
        await _messageService.MarkConversationAsReadAsync(user.Id, otherUserId);

        return Ok();
    }
}
