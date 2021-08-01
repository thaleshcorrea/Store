using Microsoft.VisualStudio.TestTools.UnitTesting;
using Store.Domain.Commands;
using Store.Domain.Handlers;
using Store.Domain.Repositories;
using Store.Tests.Repositories;
using System;
using System.Collections.Generic;

namespace Store.Tests.Handlers
{
    [TestClass]
    public class OrderHandlerTests
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        private readonly OrderHandler _handler;

        private readonly IList<CreateOrderItemCommand> _items;

        public OrderHandlerTests()
        {
            _customerRepository = new FakeCustomerRepository();
            _deliveryFeeRepository = new FakeDeliveryFeeRepository();
            _discountRepository = new FakeDiscountRepository();
            _productRepository = new FakeProductRepository();
            _orderRepository = new FakeOrderRepository();

            _handler = new OrderHandler(_customerRepository, _deliveryFeeRepository, _discountRepository, _productRepository, _orderRepository);

            _items = new List<CreateOrderItemCommand>
            {
                new CreateOrderItemCommand {Product = Guid.NewGuid(), Quantity = 1},
                new CreateOrderItemCommand {Product = Guid.NewGuid(), Quantity = 1},
            };
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void Dado_um_cliente_inexistente_o_pedido_nao_deve_ser_gerado()
        {
            var command = new CreateOrderCommand
            {
                Customer = "11111111111",
                PromoCode = "12345678",
                ZipCode = "12345678",
                Items = _items
            };

            _handler.Handle(command);
            Assert.AreEqual(false, _handler.IsValid);
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void Dado_um_cep_invalido_o_pedido_deve_ser_gerado_normalmente()
        {
            var command = new CreateOrderCommand
            {
                Customer = "12345678911",
                PromoCode = "12345678",
                ZipCode = "11111111",
                Items = _items
            };

            _handler.Handle(command);
            Assert.AreEqual(true, _handler.IsValid);
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void Dado_um_promocode_inexistente_o_pedido_deve_ser_gerado_normalmente()
        {
            var command = new CreateOrderCommand
            {
                Customer = "12345678911",
                PromoCode = "00000000",
                ZipCode = "12345678",
                Items = _items
            };

            _handler.Handle(command);
            Assert.AreEqual(true, _handler.IsValid);
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void Dado_um_pedido_sem_items_o_mesmo_nao_deve_ser_gerado()
        {
            var command = new CreateOrderCommand
            {
                Customer = "12345678911",
                PromoCode = "12345678",
                ZipCode = "12345678",
            };

            _handler.Handle(command);
            Assert.AreEqual(false, _handler.IsValid);
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void Dado_um_comando_invalido_o_pedido_nao_deve_ser_gerado()
        {
            var command = new CreateOrderCommand
            {
                Customer = "1234567891",
                PromoCode = "12345678",
                ZipCode = "12345678",
            };

            _handler.Handle(command);
            Assert.AreEqual(false, _handler.IsValid);
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void Dado_um_comando_valido_o_pedido_deve_ser_gerado()
        {
            var command = new CreateOrderCommand
            {
                Customer = "12345678911",
                ZipCode = "12345678",
                PromoCode = "12345678",
                Items = _items
            };

            _handler.Handle(command);
            Assert.AreEqual(true, _handler.IsValid);
        }
    }
}
