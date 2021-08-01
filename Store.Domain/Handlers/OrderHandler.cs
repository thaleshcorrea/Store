using Flunt.Notifications;
using Store.Domain.Commands;
using Store.Domain.Commands.Interfaces;
using Store.Domain.Entities;
using Store.Domain.Handlers.Interfaces;
using Store.Domain.Repositories;
using Store.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Handlers
{
    public class OrderHandler : Notifiable<Notification>, IHandler<CreateOrderCommand>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderHandler(ICustomerRepository customerRepository,
            IDeliveryFeeRepository deliveryFeeRepository,
            IDiscountRepository discountRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _deliveryFeeRepository = deliveryFeeRepository;
            _discountRepository = discountRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public ICommandResult Handle(CreateOrderCommand command)
        {
            // Fail fast validation
            command.Validate();
            if (!command.IsValid)
            {
                AddNotifications(command.Notifications);

                return new GenericCommandResult(false, "Pedido invalido", command.Notifications);
            }

            // 1 - Recuperar cliente
            Customer customer = _customerRepository.Get(command.Customer);

            // 2 - Calcular taxa de entrega
            decimal deliveryFee = _deliveryFeeRepository.Get(command.ZipCode);

            // 3 - Obtem o cupom de desconto
            Discount discount = _discountRepository.Get(command.PromoCode);

            // 4 - Gera o pedido
            var products = _productRepository.Get(ExtractGuids.Extract(command.Items));
            var order = new Order(customer, deliveryFee, discount);
            foreach(var item in command.Items)
            {
                var product = products.Where(x => x.Id == item.Product).FirstOrDefault();
                order.AddItem(product, item.Quantity);
            }

            // 5 - Agrupar notificacoes
            AddNotifications(order.Notifications);

            // 6 - Verifica se deu tudo certo
            if (IsValid)
                return new GenericCommandResult(false, "Falha ao gerar o pedido", Notifications);

            // 7 - Retorna o resultado
            _orderRepository.Save(order);
            return new GenericCommandResult(true, $"Pedido {order.Number} gerado com sucesso", order);
        }
    }
}
