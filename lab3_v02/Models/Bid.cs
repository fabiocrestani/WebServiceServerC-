using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace lab3_v02.Models {

    /// <summary>
    /// Classe para guardar os dados de um lance de compra ou venda de ação.
    /// </summary>
    public class Bid {

        /// <summary>
        /// Status da transação
        /// </summary>
        public enum BidStatus {
            PENDING,                ///< Transação pendente (não encontrou comprador ou vendedor compatível)
            DONE,                   ///< Transação concluída (ainda não notificou o resultado para o cliente)
            SENT_TO_CLIENT          ///< Transação concluída e resultados enviados ao cliente (Será remova da lista em breve)
        }

        /// <summary>
        /// Tipo de transação. Compra ou venda.
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