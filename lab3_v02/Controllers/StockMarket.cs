using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using lab3_v02.Models;
using System.Text.Encodings.Web;
using System.IO;
using System.Net.Http;
using System.Net;

namespace lab3_v02.Controllers {
    public class StockMarket : Controller {
        private static List<Stock> listOfStocks = new List<Stock>();
        private static List<Bid> listOfSellers = new List<Bid>();
        private static List<Bid> listOfBuyers = new List<Bid>();

        // Exemplos de como usar os métodos REST
        // GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        /// <summary>
        /// Construtor. Carrega lista de ações a partir de um arquivo de texto.
        /// O valor inicial das ações é gerado aleatoriamente.
        /// </summary>
        static StockMarket() {
            using (var reader = new StreamReader("listaDeAcoes.txt")) {
                String line = "";
                while ((line = reader.ReadLine()) != null) {
                    listOfStocks.Add(new Stock(line));
                }
            }
            System.Console.WriteLine("Carregando ações:");
            foreach (Stock s in listOfStocks) {
                System.Console.WriteLine("Carregada acao {0} R$ {1}", s.name, s.price);
            }
        }

        ///
        /// [GET] /StockMarket/
        /// "Página" inicial.
        /// 
        public string Index() => "Opções disponíveis: /Sell /Buy /Poll /ListAll";

        /// <summary>
        /// [POST]
        /// Registra um ordem de compra ou venda no servidor.
        /// </summary>
        /// <param name="bid">Lance</param>
        /// <returns>Mensagem de controle (ACK ou NACK)</returns>
        [HttpPost]
        public IActionResult Post([FromBody] Bid bid) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            System.Console.WriteLine("Cliente {0} registando ordem de compra da acao {1} por R$ {2} e quantidade {3}", bid.clientId, bid.stockName, bid.negotiatedPrice, bid.quantity);
            Bid presentBid = null;
            List<Bid> list;
            if (bid.type == Bid.BidType.BUY) {
                list = listOfBuyers;
            } else if (bid.type == Bid.BidType.SELL) {
                list = listOfSellers;
            } else {
                return Nack();
            }
            foreach (Bid b in list) {
                if (b.stockName.Equals(bid.stockName)) {
                    presentBid = b;
                    presentBid.quantity = bid.quantity;
                    presentBid.negotiatedPrice = bid.negotiatedPrice;
                    ComputeBids();
                    return Ack();
                }
            }
            if (presentBid == null) {
                presentBid = new Bid(bid.stockName, bid.quantity, bid.negotiatedPrice, bid.type, bid.clientId);
                list.Add(presentBid);
            }
            ComputeBids();
            return Ack();
        }

        /// <summary>
        /// [GET] /StockMarket/Poll/
        /// Usado para fazer polling no servidor perguntando o estado de um lance.
        /// </summary>
        /// <param name="stockName">Ação que se deseja saber o estado.</param>
        /// <param name="clientId">Id do cliente perguntando.</param>
        /// <returns>Um objeto Bid com o estado atual do lance no servidor. Ou null, se não encontrou o lance.</returns>
        public Bid Poll(string stockName, int clientId) {
            ComputeBids();
            RemoveAlreadyNotifiedBids();
            foreach (Bid b in listOfSellers) {
                if (b.stockName.Equals(stockName) && (b.clientId == clientId) && (b.status == Bid.BidStatus.DONE)) {
                    b.status = Bid.BidStatus.SENT_TO_CLIENT;
                    return b;
                }
            }
            foreach (Bid b in listOfBuyers) {
                if (b.stockName.Equals(stockName) && (b.clientId == clientId) && (b.status == Bid.BidStatus.DONE)) {
                    b.status = Bid.BidStatus.SENT_TO_CLIENT;
                    return b;
                }
            }
            return null;
        }

        /// <summary>
        /// Percorre as lista de compra e venda de ações e trata as negociações.
        /// </summary>
        private void ComputeBids() {
            foreach (Bid buyer in listOfBuyers) {
                foreach (Bid seller in listOfSellers) {
                    if ((buyer.status == Bid.BidStatus.PENDING) && (seller.status == Bid.BidStatus.PENDING) && (buyer.stockName.Equals(seller.stockName)) &&
                        (buyer.clientId != seller.clientId) && (seller.quantity > 0) && (buyer.quantity > 0)) {
                        DoTransaction(buyer, seller);
                    }
                }
            }
        }

        /// <summary>
        /// Processa a transação em si
        /// </summary>
        private void DoTransaction(Bid buyer, Bid seller) {
            double newPrice = (seller.negotiatedPrice + buyer.negotiatedPrice) / 2;
            double transactionedQuantity = 0;
            if (buyer.quantity > seller.quantity) {
                transactionedQuantity = seller.quantity;
            } else {
                transactionedQuantity = buyer.quantity;
            }

            buyer.quantity = transactionedQuantity;
            seller.quantity = transactionedQuantity;

            buyer.negotiatedPrice = newPrice;
            seller.negotiatedPrice = newPrice;

            UpdatePriceOfStock(buyer.stockName, newPrice);

            System.Console.WriteLine("");
            System.Console.WriteLine("Nova transação efetuada:");
            System.Console.WriteLine("Cliente " + seller.clientId + " vendeu  " + transactionedQuantity + " ações " + seller.stockName + " por R$" + newPrice);
            System.Console.WriteLine("Cliente " + buyer.clientId + " comprou " + transactionedQuantity + " ações " + buyer.stockName + " por R$" + newPrice);
            System.Console.WriteLine("");

            buyer.status = Bid.BidStatus.DONE;
            seller.status = Bid.BidStatus.DONE;
        }

        /// <summary>
        /// Atualiza o preço de uma ação no servidor.
        /// </summary>
        /// <param name="stockName">Nome da ação.</param>
        /// <param name="newPrice">Novo preço da ação.</param>
        private void UpdatePriceOfStock(String stockName, double newPrice) {
            foreach (Stock s in listOfStocks) {
                if (s.name.Equals(stockName)) {
                    s.price = newPrice;
                }
            }
        }

        /// <summary>
        /// Remove da lista de compradores ou vendedores os lances que já foram devidamente concluídos e cujos clientes foram notificados.
        /// </summary>
        private void RemoveAlreadyNotifiedBids() {
            foreach (Bid b in listOfBuyers.ToList()) {
                if (b.status == Bid.BidStatus.SENT_TO_CLIENT) {
                    listOfBuyers.Remove(b);
                }
            }
            foreach (Bid b in listOfSellers.ToList()) {
                if (b.status == Bid.BidStatus.SENT_TO_CLIENT) {
                    listOfSellers.Remove(b);
                }
            }
        }

        /// <summary>
        /// [GET] /StockMarket/ListAll
        /// Lista todas as ações presentes no servidor.
        /// </summary>
        /// <returns>Lista de todas as ações presentes no servidor</returns>
        public IEnumerable<Stock> ListAll() => listOfStocks;

        /// <summary>
        /// [GET] /StockMarket/GetPrice
        /// Consulta o preço de uma ação.
        /// </summary>
        /// <returns>O preço atual da ação stockName</returns>
        public Stock GetPrice(string stockName) {
            System.Console.WriteLine("GetPrice: stock name = " + stockName);
            foreach (Stock s in listOfStocks) {
                if (s.name.Equals(stockName)) {
                    return s;
                }
            }
            return new Stock("");
        }

        /// <summary>
        /// Retorna um mensagem de ACK.
        /// </summary>
        /// <returns></returns>
        private IActionResult Ack() => Created("", new ControlMessage(ControlMessage.ControlMessageCode.ACK));

        /// <summary>
        /// Retorna uma mensagem de NACK.
        /// </summary>
        /// <returns></returns>
        private IActionResult Nack() => Created("", new ControlMessage(ControlMessage.ControlMessageCode.NACK));

        /// <summary>
        /// "Página" para debug acessível via /StockMarket/Debug
        /// </summary>
        /// <returns></returns>
        public string Debug() {
            string m = "Lista de ações do servidor:\r";
            foreach (Stock s in listOfStocks) {
                m += s.ToString() + "\r";
            }
            m += "\r\rLista de lances de compra:\r";
            foreach (Bid b in listOfBuyers) {
                m += b.ToString() + "\r";
            }
            m += "\r\rLista de lances de venda:\r";
            foreach (Bid b in listOfSellers) {
                m += b.ToString() + "\r";
            }
            return m;
        }

    }
}
