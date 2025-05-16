using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bookings.Api.Controllers;
using Bookings.Api.Controllers.Request.Messages;
using Bookings.Api.Infrastructure.Messages;
using Bookings.Api.Infrastructure.Services.Users;
using Bookings.Domain.AggregatesModel.MessageAggregate;
using Bookings.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Bookings.Tests
{
    [TestFixture]
    public class MessageControllerTests
    {
        private Mock<ILogger<MessageController>> _loggerMock;
        private Mock<IMessageService> _messageServiceMock;
        private Mock<IUserService> _userServiceMock;
        private MessageController _controller;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<MessageController>>();
            _messageServiceMock = new Mock<IMessageService>();
            _userServiceMock = new Mock<IUserService>();
            _controller = new MessageController(_loggerMock.Object, _messageServiceMock.Object, _userServiceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "auth0|123456789"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@example.com")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Test]
        public async Task GetAllConversations_WhenUserExists_ReturnsOkResultWithConversations()
        {
            string auth0Id = "auth0|123456789";
            var user = new User { Id = 1, Auth0Id = auth0Id };
            var conversations = new List<Conversation>
            {
                new Conversation
                {
                    Id = "1-2",
                    OtherUserId = 2
                }
            };

            _userServiceMock
                .Setup(u => u.GetUserByAuth0IdAsync(auth0Id))
                .ReturnsAsync(user);

            _messageServiceMock
                .Setup(m => m.GetAllConversationsAsync(user.Id))
                .ReturnsAsync(conversations);

            var result = await _controller.GetAllConversations();

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(conversations, okResult.Value);
        }

        [Test]
        public async Task GetAllConversations_WhenUserDoesNotExist_ReturnsNotFoundResult()
        {
            string auth0Id = "auth0|123456789";

            _userServiceMock
                .Setup(u => u.GetUserByAuth0IdAsync(auth0Id))
                .ReturnsAsync((User)null);

            var result = await _controller.GetAllConversations();

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User not found", notFoundResult.Value);
        }

        [Test]
        public async Task GetConversation_WhenUserAndConversationExist_ReturnsOkResultWithMessages()
        {
            string auth0Id = "auth0|123456789";
            int otherUserId = 2;
            var user = new User { Id = 1, Auth0Id = auth0Id };
            var messages = new List<Message>
            {
                new Message
                {
                    Id = 1,
                    SenderId = 1,
                    ReceiverId = 2,
                    Content = "Hello"
                }
            };

            _userServiceMock
                .Setup(u => u.GetUserByAuth0IdAsync(auth0Id))
                .ReturnsAsync(user);

            _messageServiceMock
                .Setup(m => m.GetConversationAsync(user.Id, otherUserId))
                .ReturnsAsync(messages);

            var result = await _controller.GetConversation(otherUserId);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(messages, okResult.Value);

            _messageServiceMock.Verify(m => m.MarkConversationAsReadAsync(user.Id, otherUserId), Times.Once);
        }

        [Test]
        public async Task SendMessage_WhenUsersExist_ReturnsOkResultWithMessage()
        {
            string auth0Id = "auth0|123456789";
            var sender = new User { Id = 1, Auth0Id = auth0Id };
            var receiver = new User { Id = 2 };
            var request = new SendMessageRequest(
                ReceiverId: receiver.Id,
                Content: "Hello",
                PropertyId: 3
            );
            var message = new Message { Id = 1, Content = "Hello" };

            _userServiceMock
                .Setup(u => u.GetUserByAuth0IdAsync(auth0Id))
                .ReturnsAsync(sender);

            _userServiceMock
                .Setup(u => u.GetUserAsync(receiver.Id))
                .ReturnsAsync(receiver);

            _messageServiceMock
                .Setup(m => m.SendMessageAsync(sender.Id, receiver.Id, request.Content, request.PropertyId))
                .ReturnsAsync(message);

            var result = await _controller.SendMessage(request);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(message, okResult.Value);
        }

        [Test]
        public async Task SendMessage_WhenSenderDoesNotExist_ReturnsNotFoundResult()
        {
            string auth0Id = "auth0|123456789";
            var request = new SendMessageRequest(
                ReceiverId: 2,
                Content: "Hello",
                PropertyId: null
            );

            _userServiceMock
                .Setup(u => u.GetUserByAuth0IdAsync(auth0Id))
                .ReturnsAsync((User)null);

            var result = await _controller.SendMessage(request);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Sender user not found", notFoundResult.Value);
        }

        [Test]
        public async Task SendMessage_WhenReceiverDoesNotExist_ReturnsNotFoundResult()
        {
            string auth0Id = "auth0|123456789";
            var sender = new User { Id = 1, Auth0Id = auth0Id };
            var request = new SendMessageRequest(
                ReceiverId: 2,
                Content: "Hello",
                PropertyId: null
            );

            _userServiceMock
                .Setup(u => u.GetUserByAuth0IdAsync(auth0Id))
                .ReturnsAsync(sender);

            _userServiceMock
                .Setup(u => u.GetUserAsync(request.ReceiverId))
                .ReturnsAsync((User)null);

            var result = await _controller.SendMessage(request);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Receiver user not found", notFoundResult.Value);
        }

        [Test]
        public async Task MarkAsRead_WhenUserExists_ReturnsOkResult()
        {
            string auth0Id = "auth0|123456789";
            var user = new User { Id = 1, Auth0Id = auth0Id };
            string conversationId = "1-2"; // Format: {userId}-{otherUserId}
            int otherUserId = 2;

            _userServiceMock
                .Setup(u => u.GetUserByAuth0IdAsync(auth0Id))
                .ReturnsAsync(user);

            var result = await _controller.MarkAsRead(conversationId);

            Assert.IsInstanceOf<OkResult>(result);
            _messageServiceMock.Verify(m => m.MarkConversationAsReadAsync(user.Id, otherUserId), Times.Once);
        }

        [Test]
        public async Task MarkAsRead_WhenUserDoesNotExist_ReturnsNotFoundResult()
        {
            string auth0Id = "auth0|123456789";
            string conversationId = "1-2";

            _userServiceMock
                .Setup(u => u.GetUserByAuth0IdAsync(auth0Id))
                .ReturnsAsync((User)null);

            var result = await _controller.MarkAsRead(conversationId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User not found", notFoundResult.Value);
        }
    }
}