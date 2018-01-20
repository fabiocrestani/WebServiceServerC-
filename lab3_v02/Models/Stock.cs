using System;

namespace lab3_v02.Models {

    /// <summary>
    /// Guarda os dados de uma ação.
    /// </summary>
    public class Stock {
        public string name { get; set; }
        public double price { get; set; }

        public Stock() {}

        /// <summary>
        /// Construtor. Seta o nome da ação com o parâmetro name e gera um preço aleatório.
        /// </summary>
        /// <param name="name">Nome da ação</param>
        public Stock(string name) {
            this.name = name;
            Random random = new Random();
            int minimumPrice = 1;
            int maximumPrice = 10;
            this.price = Math.Round(random.NextDouble() * (maximumPrice - minimumPrice) + minimumPrice, 2);
        }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns>Retorna o nome e o preço da ação em uma string.</returns>
        override public string ToString() => name + " R$ " + price.ToString();
    }
}