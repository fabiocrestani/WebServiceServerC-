using System;

namespace lab3_v02.Models {

    /// <summary>
    /// Guarda os dados de uma a��o.
    /// </summary>
    public class Stock {
        public string name { get; set; }
        public double price { get; set; }

        public Stock() {}

        /// <summary>
        /// Construtor. Seta o nome da a��o com o par�metro name e gera um pre�o aleat�rio.
        /// </summary>
        /// <param name="name">Nome da a��o</param>
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
        /// <returns>Retorna o nome e o pre�o da a��o em uma string.</returns>
        override public string ToString() => name + " R$ " + price.ToString();
    }
}