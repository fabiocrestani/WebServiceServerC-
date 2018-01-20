using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace lab3_v02.Models {

    /// <summary>
    /// Classe para guardar os dados de um lance de compra ou venda de a��o.
    /// </summary>
    public class Bid {

        /// <summary>
        /// Status da transa��o
        /// </summary>
        public enum BidStatus {
            PENDING,                ///< Transa��o pendente (n�o encontrou comprador ou vendedor compat�vel)
            DONE,                   ///< Transa��o conclu�da (ainda n�o notificou o resultado para o cliente)
            SENT_TO_CLIENT          ///< Transa��o conclu�da e resultados enviados ao cliente (Ser� remova da lista em breve)
        }

        /// <summary>
        /// Tipo de transa��o. Compra ou venda.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum BidType {
            SELL,
            BUY
        }

        public string stockName { get; set; }
        public double quantity { get; set; }
        public double negotiatedPrice { get; set; }
        public BidType type { get; set; }
        public int clientId { get; set; }
        public BidStatus status { get; set; }

        public Bid(String stockName, double quantity, double price, BidType type, int clientId) {
            this.type = type;
            this.stockName = stockName;
            this.quantity = quantity;
            this.negotiatedPrice = price;
            this.clientId = clientId;
            this.status = BidStatus.PENDING;
        }

        public string statusAsString() {
            if (status == BidStatus.PENDING) {
                return "PENDENTE";
            } else if (status == BidStatus.DONE) {
                return "PRONTO";
            } else if (status == BidStatus.SENT_TO_CLIENT) {
                return "ENVIADO AO CLIENTE";
            } else {
                return "desconhecido";
            }
        }

        override public string ToString() => stockName + " R$" + negotiatedPrice.ToString() + " qntd: " + quantity.ToString() + " client: " +
            clientId.ToString() + " status: " + statusAsString();
    }
}