using Application.Commands;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests
{
    public class CreateOrderCommandTests
    {
        private readonly Mock<IOrderRepository> _mockRepo = new();
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandTests()
        {
            _handler = new CreateOrderCommandHandler(
                _mockRepo.Object,
                _mediatorMock.Object,
                Mock.Of<ILogger<CreateOrderCommandHandler>>());
        }

        [Fact]
        public async Task Handle_Should_CreateOrder_AndPublishEvent()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<Application.Commands.OrderItemDto>
        {
            new(Guid.NewGuid(), 10m, 2)
        };

            // Act
            var result = await _handler.Handle(new CreateOrderCommand(customerId, items), default);

            // Assert
            Assert.NotEqual(Guid.Empty, result);

            _mediatorMock.Verify(m =>
                m.Publish(
                    It.Is<OrderCreatedEvent>(e =>
                        e.OrderId == result &&
                        e.CustomerId == customerId &&
                        e.TotalAmount == 20m),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}