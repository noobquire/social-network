using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using SocialNetworkApi.Services.Interfaces;
using Moq;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Implementations;
using SocialNetworkApi.Services.Models;
using AutoFixture;
using SocialNetworkApi.Services.Extensions;

namespace SocialNetworkApi.Services.Tests
{
    [TestFixture]
    public class MessagesServiceTests
    {
        private IMessagesService _messagesService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<UserManager<User>> _userManager;
        private Mock<IHttpContextAccessor> _httpContext;
        private Mock<IChatsService> _chatsService;
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _userManager = MockUserManager(new List<User>());
            _httpContext = new Mock<IHttpContextAccessor>();
            _chatsService = new Mock<IChatsService>();
            _fixture = new Fixture();

            var user = new User { Id = Guid.NewGuid() };
            _httpContext.Setup(c => c.HttpContext.User)
                .Returns(_fixture.Create<ClaimsPrincipal>());
            _userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _messagesService = new MessagesService(
                _unitOfWork.Object,
                _userManager.Object,
                _httpContext.Object,
                _chatsService.Object);
        }

        [Test]
        public void SendGroupMessageAsync_GroupDoesNotExists_ThrowsItemNotFoundException()
        {
            var chatId = Guid.NewGuid().ToString();
            var messageData = new MessageDataModel()
            {
                Text = "This is a test message!"
            };
            _unitOfWork.Setup(u => u.Chats.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Chat)null);

            _messagesService.Awaiting(s => s.SendGroupMessageAsync(chatId, messageData))
                .Should().Throw<ItemNotFoundException>();
        }

        [Test]
        public void SendGroupMessageAsync_NotGroupChat_ThrowsInvalidOperationException()
        {
            var chatId = Guid.NewGuid().ToString();
            var messageData = new MessageDataModel()
            {
                Text = "This is a test message!"
            };
            var chat = new Chat
            {
                Type = ChatType.Personal
            };
            _unitOfWork.Setup(u => u.Chats.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(chat);

            _messagesService.Awaiting(s => s.SendGroupMessageAsync(chatId, messageData))
                .Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void SendGroupMessageAsync_ReplyToUnknownMessage_ThrowsInvalidOperationException()
        {
            var chatId = Guid.NewGuid().ToString();
            var replyId = Guid.NewGuid();
            var messageData = new MessageDataModel()
            {
                Text = "This is a test message!",
                ReplyToId = replyId.ToString()
            };
            var replyMessage = new Message()
            {
                Id = replyId,
                Text = "This is a message in another chat",
                TimePublished = DateTime.UtcNow,
                ChatId = Guid.NewGuid()
            };
            var chat = new Chat
            {
                Type = ChatType.Group,
            };
            _unitOfWork.Setup(u => u.Chats.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(chat);
            _unitOfWork.Setup(u => u.Messages.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(replyMessage);

            _messagesService.Awaiting(s => s.SendGroupMessageAsync(chatId, messageData))
                .Should().Throw<ItemNotFoundException>();
        }

        [Test]
        public async Task SendGroupMessageAsync_Success_AddsMessageToRepository()
        {
            var chatId = Guid.NewGuid().ToString();
            var messageData = new MessageDataModel()
            {
                Text = "This is a test message!"
            };
            var chat = new Chat
            {
                Type = ChatType.Group
            };
            _unitOfWork.Setup(u => u.Chats.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(chat);
            _unitOfWork.Setup(u => u.Messages
                .CreateAsync(It.Is<Message>(m =>
                    m.Text == messageData.Text)));

            await _messagesService.SendGroupMessageAsync(chatId, messageData);

            _unitOfWork.VerifyAll();
        }

        [Test]
        public async Task SendGroupMessageAsync_Success_ReturnsExpectedMessage()
        {
            var chatId = Guid.NewGuid();
            var messageData = new MessageDataModel()
            {
                Text = "This is a test message!"
            };
            var chat = new Chat
            {
                Id = chatId,
                Type = ChatType.Group
            };
            _unitOfWork.Setup(u => u.Chats.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(chat);
            _unitOfWork.Setup(u => u.Messages
                .CreateAsync(It.Is<Message>(m =>
                    m.Text == messageData.Text)));

            var message = await _messagesService.SendGroupMessageAsync(chatId.ToString(), messageData);

            message.ChatId.Should().Be(chatId.ToString());
            message.Text.Should().Be(messageData.Text);
            message.TimePublished.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Test]
        public void SendPersonalMessageAsync_UserNotFound_ThrowsItemNotFoundException()
        {
            var userId = Guid.NewGuid().ToString();
            var messageData = new MessageDataModel()
            {
                Text = "This is a test message!"
            };
            _unitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            _messagesService.Awaiting(s => s.SendPersonalMessageAsync(userId, messageData))
                .Should().Throw<ItemNotFoundException>();
        }

        [Test]
        public async Task SendPersonalMessageAsync_Success_ReturnsExpectedMessage()
        {
            var userId = Guid.NewGuid();
            var messageData = new MessageDataModel()
            {
                Text = "This is a test message!"
            };
            var chat = new Chat
            {
                Type = ChatType.Personal
            };
            var otherUser = new User
            {
                Id = userId
            };
            _unitOfWork.Setup(u => u.Chats.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(chat);
            _unitOfWork.Setup(u => u.Messages
                .CreateAsync(It.Is<Message>(m =>
                    m.Text == messageData.Text)));
            _unitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(otherUser);
            _unitOfWork.Setup(u => u.Chats.QueryAsync(It.IsAny<Func<Chat, bool>>()))
                .ReturnsAsync(new[] { chat });

            var message = await _messagesService.SendPersonalMessageAsync(userId.ToString(), messageData);

            message.Text.Should().Be(messageData.Text);
            message.TimePublished.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Test]
        public async Task SendPersonalMessageAsync_ChatWithUserDoesNotExist_CreatesPersonalChat()
        {
            var userId = Guid.NewGuid();
            var messageData = new MessageDataModel()
            {
                Text = "This is a test message!"
            };
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Type = ChatType.Personal
            };
            var otherUser = new User
            {
                Id = userId
            };
            _unitOfWork.Setup(u => u.Chats.QueryAsync(It.IsAny<Func<Chat, bool>>()))
                .ReturnsAsync(new List<Chat>());
            _unitOfWork.Setup(u => u.Messages
                .CreateAsync(It.Is<Message>(m =>
                    m.Text == messageData.Text)));
            _unitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(otherUser);
            _chatsService.Setup(s => s.CreatePersonalAsync(It.IsAny<string>()))
                .ReturnsAsync(chat.ToDto());
            _unitOfWork.Setup(u => u.Chats.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(chat);

            await _messagesService.SendPersonalMessageAsync(userId.ToString(), messageData);

            _chatsService.VerifyAll();
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }
    }
}