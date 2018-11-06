namespace Stripe.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Stripe;

    internal class StripeListItemConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var incoming = JObject.FromObject(value);
            incoming.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var incoming = JObject.Load(reader);

                List<JsonConverter> converters = new List<JsonConverter>();
                if (objectType.GetTypeInfo().IsInterface)
                {
                    // Polymorphic type, need to call specialized deserializer
                    if (objectType == typeof(IBalanceTransactionSource))
                    {
                        converters.Add(new BalanceTransactionSourceConverter());
                    }
                    else if (objectType == typeof(IExternalAccount))
                    {
                        converters.Add(new ExternalAccountConverter());
                    }
                    else if (objectType == typeof(IPaymentSource))
                    {
                        converters.Add(new PaymentSourceConverter());
                    }
                }

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Converters = converters,
                };

                var item = JsonConvert.DeserializeObject(incoming.ToString(), objectType, settings);

                return item;
            }

            return null;
        }
    }
}
