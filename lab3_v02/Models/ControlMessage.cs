using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace lab3_v02.Models {
    /// <summary>
    /// Mensagem de controle.
    /// É usada para responder o cliente com ACK ou NACK.
    /// </summary>
    public class ControlMessage {

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ControlMessageCode {
            ACK,
            NACK
        }

        public ControlMessageCode code { get; set; }

        public ControlMessage(ControlMessageCode code) {
            this.code = code;
        }
    }
}