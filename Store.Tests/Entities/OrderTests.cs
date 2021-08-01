using Microsoft.VisualStudio.TestTools.UnitTesting;
using Store.Domain.Entities;
using Store.Domain.Enums;
using System;

namespace Store.Tests.Entities
{
    [TestClass]
    public class OrderTests
    {
        private readonly Customer _customer = new("Cliente", "cliente@email.com");
        private readonly Product _product = new("Produto 1", 10, true);
        private readonly Discount _discount = new(10, DateTime.Now.AddMonths(1));

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_novo_pedido_valido_ele_deve_gerar_um_numero_com_8_caracteres()
        {
            Order order = new(_customer, 0, null);
            Assert.AreEqual(8, order.Number.Length);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_novo_pedido_seu_status_deve_ser_aguardando_pagamento()
        {
            Order order = new(_customer, 0, null);
            Assert.AreEqual(EOrderStatus.WaitingPayment, order.Status);
        }
        
        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_pagamento_do_pedido_seu_status_deve_ser_aguardando_entrega()
        {
            Order order = new(_customer, 0, null);
            order.AddItem(_product, 1);
            order.Pay(10);

            Assert.AreEqual(EOrderStatus.WaitingDelivey, order.Status);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_pedido_cancelado_seu_status_deve_ser_cancelado()
        {
            Order order = new(_customer, 0, null);
            order.Cancel();

            Assert.AreEqual(EOrderStatus.Canceled, order.Status);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_novo_item_sem_produto_o_mesmo_nao_deve_ser_adicionado()
        {
            Order order = new(_customer, 0, null);
            order.AddItem(null, 0);

            Assert.AreEqual(0, order.Items.Count);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_novo_item_com_quantidade_zero_ou_menor_o_mesmo_nao_deve_ser_adicionado()
        {
            Order order = new(_customer, 0, null);
            order.AddItem(_product, 0);
            order.AddItem(_product, -1);

            Assert.AreEqual(0, order.Items.Count);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_novo_pedido_valido_seu_total_deve_ser_50()
        {
            Order order = new(_customer, 0, null);
            order.AddItem(_product, 5);

            Assert.AreEqual(50, order.Total());
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_desconto_expirado_o_valor_do_pedido_deve_ser_60()
        {
            Discount expiredDiscount = new(10, DateTime.Now.AddDays(-1));
            Order order = new(_customer, 10, expiredDiscount);
            order.AddItem(_product, 5);

            Assert.AreEqual(60, order.Total());
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_desconto_invalido_o_valor_do_pedido_deve_ser_60()
        {
            Order order = new(_customer, 10, null);
            order.AddItem(_product, 5);

            Assert.AreEqual(60, order.Total());
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_desconto_de_10_o_valor_do_pedido_deve_ser_50()
        {
            Discount discount = new(10, DateTime.Now.AddDays(10));
            Order order = new(_customer, 10, discount);
            order.AddItem(_product, 5);

            Assert.AreEqual(50, order.Total());
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_uma_taxa_de_entrega_de_10_o_valor_do_pedido_dever_ser_60()
        {
            Order order = new(_customer, 10, null);
            order.AddItem(_product, 5);

            Assert.AreEqual(60, order.Total());
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void Dado_um_pedido_sem_cliente_o_mesmo_deve_ser_invalido()
        {
            Order order = new(null, 10, null);

            Assert.IsFalse(order.IsValid);
        }
    }
}